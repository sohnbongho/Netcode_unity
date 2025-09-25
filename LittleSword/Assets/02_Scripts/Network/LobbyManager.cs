using System;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;
using Logger = LittelSword.Common.Logger;

namespace LittleSword.Network.LobbyUI
{
    public class LobbyManager : MonoBehaviour
    {
        [SerializeField] private TMP_InputField lobbyNameInput;
        [SerializeField] private TMP_InputField lobbyCodeInput;
        [SerializeField] private Button createLobbyButton;
        [SerializeField] private Button joinLobbyButton;
        [SerializeField] private Button quitJoinLobbyButton;
        [SerializeField] private Button leaveLobbyButton;

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
        }

        private void OnDisable()
        {
            createLobbyButton.onClick.RemoveAllListeners();
            joinLobbyButton.onClick.RemoveAllListeners();
            quitJoinLobbyButton.onClick.RemoveAllListeners();
            leaveLobbyButton.onClick.RemoveAllListeners();

            CancelInvoke(nameof(SendHeartbeatAsync));
            CancelInvoke(nameof(PollingLobbyAsync));
        }

        #endregion

        #region 로비 관련 메소드
        //로비 생성
        private async void CreateLobbyAsync(string lobbyName, int maxPlayers)
        {
            try
            {
                CurrentLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers);
                DisplayCurrenLobby();
                BindingLobbyCallbacks();

                if (IsHost)
                {
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
    }

}
