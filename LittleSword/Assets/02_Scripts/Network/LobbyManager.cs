using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Logger = LittelSword.Common.Logger;

namespace LittleSword.Network.LobbyUI
{
    public class LobbyManager : MonoBehaviour
    {
        private const string KEY_RELAY_JOIN_CODE = "RelayJoinCode";
        private const string BATTLE_SCENE_NAME = "Level01";

        [SerializeField] private TMP_InputField lobbyNameInput;
        [SerializeField] private TMP_InputField lobbyCodeInput;
        [SerializeField] private Button createLobbyButton;
        [SerializeField] private Button joinLobbyButton;
        [SerializeField] private Button quitJoinLobbyButton;
        [SerializeField] private Button leaveLobbyButton;
        [SerializeField] private Button startGameButton;

        private Lobby CurrentLobby;
        private bool IsHost => CurrentLobby != null &&
            CurrentLobby.HostId == AuthenticationService.Instance.PlayerId;

        #region ����Ƽ �̺�Ʈ
        private async void Awake()
        {
            await UnityServices.InitializeAsync();
            await AuthenticationService.Instance.SignInAnonymouslyAsync();

            Logger.Log($"�͸� �α��� ����: Player Id : {AuthenticationService.Instance.PlayerId}");
        }

        private void Start()
        {
            startGameButton.interactable = false;
        }

        private void OnEnable()
        {
            createLobbyButton.onClick.AddListener(
                () => CreateLobbyAsync(lobbyNameInput.text, 4));

            joinLobbyButton.onClick.AddListener(
                () => JoinLobbyAsync(lobbyCodeInput.text));

            quitJoinLobbyButton.onClick.AddListener(
                () => QuitJoinLobbyAsync());

            leaveLobbyButton.onClick.AddListener(
                () => LeaveLobbyAsync());

            startGameButton.onClick.AddListener(
                () => GameStart());
        }

        private void OnDisable()
        {
            createLobbyButton.onClick.RemoveAllListeners();
            joinLobbyButton.onClick.RemoveAllListeners();
            quitJoinLobbyButton.onClick.RemoveAllListeners();
            leaveLobbyButton.onClick.RemoveAllListeners();
            startGameButton.onClick.RemoveAllListeners();

            CancelInvoke(nameof(SendHeartbeatAsync));
            CancelInvoke(nameof(PollingLobbyAsync));
        }

        #endregion

        #region �κ� ���� �޼ҵ�
        // �κ� ����
        // # 1. Relay ���� �Ҵ�
        private async void CreateLobbyAsync(string lobbyName, int maxPlayers)
        {
            try
            {
                // #1 ������ ���� �Ҵ�
                var relayAlloc = await RelayService.Instance.CreateAllocationAsync(maxPlayers);
                // #2 Join Code ����
                var relayJoinCode = await RelayService.Instance.GetJoinCodeAsync(relayAlloc.AllocationId);
                Logger.Log($"Relay �Ҵ� ����: {relayJoinCode}");
                // #3. ��� ��� ����
                var relayServerData = relayAlloc.ToRelayServerData("dtls"); // ���� �������� : UDP ���̽�
                var utp = NetworkManager.Singleton.GetComponent<UnityTransport>();
                utp.SetRelayServerData(relayServerData);
                // #4 �κ� �ɼ� ����
                var lobbyOptions = new CreateLobbyOptions
                {
                    Data = new Dictionary<string, DataObject>
                    {
                        [KEY_RELAY_JOIN_CODE] = new DataObject(
                            DataObject.VisibilityOptions.Member, relayJoinCode
                            )
                    }
                };

                // #5 �κ� ���� With Options
                CurrentLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, lobbyOptions);
                DisplayCurrenLobby();
                BindingLobbyCallbacks();

                // �κ� ������ ������ Host
                NetworkManager.Singleton.StartHost();

                if (IsHost)
                {
                    startGameButton.interactable = true;
                    InvokeRepeating(nameof(SendHeartbeatAsync), 5f, 5f);
                    InvokeRepeating(nameof(PollingLobbyAsync), 3f, 3f);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
            }
        }

        //�κ� ����
        private async void JoinLobbyAsync(string lobbyCode)
        {
            try
            {
                CurrentLobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode);
                DisplayCurrenLobby();

                // Relay ����
                await RelaySetup(CurrentLobby);                
                // Ŭ���̾�Ʈ ����
                NetworkManager.Singleton.StartClient();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
            }
        }

        // ������
        private async void QuitJoinLobbyAsync()
        {
            try
            {
                CurrentLobby = await LobbyService.Instance.QuickJoinLobbyAsync();
                DisplayCurrenLobby();

                // Relay ����
                await RelaySetup(CurrentLobby);
                // Ŭ���̾�Ʈ ����
                NetworkManager.Singleton.StartClient();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
            }

        }

        // �κ� ������
        private async void LeaveLobbyAsync()
        {
            try
            {
                await LobbyService.Instance.RemovePlayerAsync(CurrentLobby.Id,
                    AuthenticationService.Instance.PlayerId);
                ClearCurrentLobby();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
            }
        }

        private void DisplayCurrenLobby()
        {
            if (CurrentLobby == null)
                return;

            lobbyNameInput.text = CurrentLobby.Name;
            lobbyCodeInput.text = CurrentLobby.LobbyCode;

            Logger.Log($"�κ� �������� : {CurrentLobby.Name}/{CurrentLobby.LobbyCode}");
        }
        private void ClearCurrentLobby()
        {
            lobbyNameInput.text = "";
            lobbyCodeInput.text = "";

        }
        #endregion

        #region �κ� ���� ���� �޼ҵ�

        // Heatbeat ���� ����(30�� �̸����� ȣ��)
        private async void SendHeartbeatAsync()
        {
            if (CurrentLobby == null)
                return;

            await LobbyService.Instance.SendHeartbeatPingAsync(CurrentLobby.Id);
            Logger.Log("Heartbeat ���� ����");
        }

        // �κ� ���� ����
        private async void PollingLobbyAsync()
        {
            if (CurrentLobby == null)
                return;

            CurrentLobby = await LobbyService.Instance.GetLobbyAsync(CurrentLobby.Id);
            Logger.Log($"�κ� ���� ����: {CurrentLobby.Name}. ������ ��:{CurrentLobby.Players.Count}");
        }
        #endregion

        #region �κ� �ݹ�
        private void BindingLobbyCallbacks()
        {
            // �κ� �ݹ鿡 ������ �̺�Ʈ�� ����
            LobbyEventCallbacks callbacks = new LobbyEventCallbacks();
            callbacks.PlayerJoined += OnPlayerJoined;
            callbacks.PlayerLeft += OnPlayerLeft;

            // �κ� �ݹ� �̺�Ʈ ����
            LobbyService.Instance.SubscribeToLobbyEventsAsync(CurrentLobby.Id, callbacks);
        }

        private void OnPlayerLeft(List<int> playerIds)
        {
            foreach (var playerId in playerIds)
            {
                Logger.Log($"�÷��̾� ����: {playerId}");
            }
        }

        private void OnPlayerJoined(List<LobbyPlayerJoined> players)
        {
            foreach (var player in players)
            {
                Logger.Log($"�÷��̾� ����: {player.Player.Id}");
            }
        }
        #endregion

        #region Relay ���� �޼ҵ�
        private async Task RelaySetup(Lobby currentLobby)
        {
            try
            {
                // #1 - Relay JoinCode
                string joinCode = currentLobby.Data[KEY_RELAY_JOIN_CODE].Value;

                // #2 - Relay Allocation 
                JoinAllocation joinAlloc = await RelayService.Instance.JoinAllocationAsync(joinCode);

                // #3 - ��� ��� ����
                var relayServerData = joinAlloc.ToRelayServerData("dtls");
                var utp = NetworkManager.Singleton.GetComponent<UnityTransport>();
                utp.SetRelayServerData(relayServerData);

            }
            catch (Exception ex)
            {
                Logger.Log(ex.Message);
            }
        }

        private async void GameStart()
        {
            try
            {
                NetworkManager.Singleton.SceneManager.LoadScene(BATTLE_SCENE_NAME,
                    LoadSceneMode.Single);

            }
            catch (Exception ex)
            {
                Logger.Log(ex.Message);
            }
        }

        #endregion
    }

}
