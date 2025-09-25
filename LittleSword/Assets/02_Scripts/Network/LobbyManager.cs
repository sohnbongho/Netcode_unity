using System;
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

        #region ����Ƽ �̺�Ʈ
        private async void Awake()
        {
            await UnityServices.InitializeAsync();
            await AuthenticationService.Instance.SignInAnonymouslyAsync();

            Logger.Log($"�͸� �α��� ����: Player Id : {AuthenticationService.Instance.PlayerId}");

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
        }

        #endregion

        #region �κ� ���� �޼ҵ�
        //�κ� ����
        private async void CreateLobbyAsync(string lobbyName, int maxPlayers)
        {
            try
            {
                CurrentLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers);
                DisplayCurrenLobby();
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
    }

}
