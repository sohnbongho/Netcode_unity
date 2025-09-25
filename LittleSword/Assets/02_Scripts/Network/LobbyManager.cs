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
        }

        private void OnDisable()
        {
            createLobbyButton.onClick.RemoveAllListeners();
        }

        #endregion

        #region �κ� ���� �޼ҵ�
        //�κ� ����
        private async void CreateLobbyAsync(string lobbyName, int maxPlayers)
        {
            try
            {
                CurrentLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers);
                lobbyNameInput.text = CurrentLobby.Name;
                lobbyCodeInput.text = CurrentLobby.LobbyCode;

                Logger.Log($"�κ� ���� �Ϸ�: {CurrentLobby.Name}, {CurrentLobby.LobbyCode}");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
            }
        }


        //�κ� ����

        // ������

        // �κ� ������

        #endregion
    }

}
