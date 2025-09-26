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

        #region 유니티 이벤트
        private async void Awake()
        {
            await UnityServices.InitializeAsync();
            await AuthenticationService.Instance.SignInAnonymouslyAsync();

            Logger.Log($"익명 로그인 성공: Player Id : {AuthenticationService.Instance.PlayerId}");
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

        #region 로비 관련 메소드
        // 로비 생성
        // # 1. Relay 서버 할당
        private async void CreateLobbyAsync(string lobbyName, int maxPlayers)
        {
            try
            {
                // #1 릴레이 서버 할당
                var relayAlloc = await RelayService.Instance.CreateAllocationAsync(maxPlayers);
                // #2 Join Code 생성
                var relayJoinCode = await RelayService.Instance.GetJoinCodeAsync(relayAlloc.AllocationId);
                Logger.Log($"Relay 할당 성공: {relayJoinCode}");
                // #3. 통신 방식 설정
                var relayServerData = relayAlloc.ToRelayServerData("dtls"); // 보안 프로토콜 : UDP 베이스
                var utp = NetworkManager.Singleton.GetComponent<UnityTransport>();
                utp.SetRelayServerData(relayServerData);
                // #4 로비 옵션 생성
                var lobbyOptions = new CreateLobbyOptions
                {
                    Data = new Dictionary<string, DataObject>
                    {
                        [KEY_RELAY_JOIN_CODE] = new DataObject(
                            DataObject.VisibilityOptions.Member, relayJoinCode
                            )
                    }
                };

                // #5 로비 생성 With Options
                CurrentLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, lobbyOptions);
                DisplayCurrenLobby();
                BindingLobbyCallbacks();

                // 로비를 생성한 유저가 Host
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

        //로비 조인
        private async void JoinLobbyAsync(string lobbyCode)
        {
            try
            {
                CurrentLobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode);
                DisplayCurrenLobby();

                // Relay 설정
                await RelaySetup(CurrentLobby);                
                // 클라이어트 가동
                NetworkManager.Singleton.StartClient();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
            }
        }

        // 퀵조인
        private async void QuitJoinLobbyAsync()
        {
            try
            {
                CurrentLobby = await LobbyService.Instance.QuickJoinLobbyAsync();
                DisplayCurrenLobby();

                // Relay 설정
                await RelaySetup(CurrentLobby);
                // 클라이어트 가동
                NetworkManager.Singleton.StartClient();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
            }

        }

        // 로비 나가기
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

            Logger.Log($"로비 접속정보 : {CurrentLobby.Name}/{CurrentLobby.LobbyCode}");
        }
        private void ClearCurrentLobby()
        {
            lobbyNameInput.text = "";
            lobbyCodeInput.text = "";

        }
        #endregion

        #region 로비 유지 관련 메소드

        // Heatbeat 전송 로직(30초 미만으로 호출)
        private async void SendHeartbeatAsync()
        {
            if (CurrentLobby == null)
                return;

            await LobbyService.Instance.SendHeartbeatPingAsync(CurrentLobby.Id);
            Logger.Log("Heartbeat 전송 성공");
        }

        // 로비 정보 갱신
        private async void PollingLobbyAsync()
        {
            if (CurrentLobby == null)
                return;

            CurrentLobby = await LobbyService.Instance.GetLobbyAsync(CurrentLobby.Id);
            Logger.Log($"로비 정보 갱신: {CurrentLobby.Name}. 접속자 수:{CurrentLobby.Players.Count}");
        }
        #endregion

        #region 로비 콜백
        private void BindingLobbyCallbacks()
        {
            // 로비 콜백에 연결한 이벤트를 선언
            LobbyEventCallbacks callbacks = new LobbyEventCallbacks();
            callbacks.PlayerJoined += OnPlayerJoined;
            callbacks.PlayerLeft += OnPlayerLeft;

            // 로비 콜백 이벤트 연결
            LobbyService.Instance.SubscribeToLobbyEventsAsync(CurrentLobby.Id, callbacks);
        }

        private void OnPlayerLeft(List<int> playerIds)
        {
            foreach (var playerId in playerIds)
            {
                Logger.Log($"플레이어 떠남: {playerId}");
            }
        }

        private void OnPlayerJoined(List<LobbyPlayerJoined> players)
        {
            foreach (var player in players)
            {
                Logger.Log($"플레이어 접속: {player.Player.Id}");
            }
        }
        #endregion

        #region Relay 관련 메소드
        private async Task RelaySetup(Lobby currentLobby)
        {
            try
            {
                // #1 - Relay JoinCode
                string joinCode = currentLobby.Data[KEY_RELAY_JOIN_CODE].Value;

                // #2 - Relay Allocation 
                JoinAllocation joinAlloc = await RelayService.Instance.JoinAllocationAsync(joinCode);

                // #3 - 통신 방식 설정
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
