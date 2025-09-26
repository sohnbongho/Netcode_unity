using LittelSword.Common;
using System;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Multiplayer;
using UnityEngine.SceneManagement;
using Logger = LittelSword.Common.Logger;

namespace LittelSword.Network
{
    public class MultiplayerSessionManager : Singleton<MultiplayerSessionManager>
    {
        // Multiplayer : Lobby, Relay, Matchmking ���հ����ϴ� SDK
        // ISession : ������ ��� �̺�Ʈ, �Ӽ�
        private ISession ActiveSession { get; set; }

        private const string BATTLE_SCENE_NAME = "Level01";
        private const string LOBBY_SCENE_NAME = "Lobby";

        #region ����Ƽ �̺�Ʈ
        private async void Start()
        {
            try
            {
                await UnityServices.InitializeAsync();
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
                Logger.Log($"�͸� �α��� ����");
            }
            catch (Exception ex)
            {
                Logger.Log(ex.Message);
            }
        }
        #endregion

        #region Session ���� �޼ҵ�
        public async void CreateSessionAsync(string sessionName)
        {
            try
            {
                // ���� �ɼ� ����
                var options = new SessionOptions
                {
                    Name = sessionName,
                    IsPrivate = false,
                    IsLocked = false,
                    MaxPlayers = 4
                }.WithRelayNetwork();

                // ���� ���� �� ȣ��Ʈ ����
                ActiveSession = await MultiplayerService.Instance.CreateSessionAsync(options);
                Logger.Log($"���� ���� �Ϸ�:{ActiveSession.Name} - {ActiveSession.Code}");
            }
            catch (Exception ex)
            {
                Logger.Log(ex.Message);
            }
        }

        public async void QuickJoinSessionAsync()
        {
            try
            {
                // Join Options
                var joinOptions = new QuickJoinOptions
                {
                    Filters = new List<FilterOption>
                {
                    new (FilterField.AvailableSlots, "1", FilterOperation.GreaterOrEqual),
                    //new (FilterField.Name, "Red", FilterOperation.Contains),
                },
                    Timeout = TimeSpan.FromSeconds(5),
                    CreateSession = true, // 5�ʰ� ������ ���� session�� ����ڴ�.
                };

                // Session Options
                var sessionOptions = new SessionOptions
                {
                    IsPrivate = false,
                    IsLocked = false,
                    MaxPlayers = 4
                }.WithRelayNetwork();

                // �� ���� �õ�
                ActiveSession = await MultiplayerService.Instance.MatchmakeSessionAsync(joinOptions,
                    sessionOptions);
                Logger.Log($"���� ���� �Ϸ�:{ActiveSession.Name} - {ActiveSession.Code}");
            }
            catch (Exception ex)
            {
                Logger.Log(ex.Message);
            }
        }
        public void StartSession()
        {
            NetworkManager.Singleton.SceneManager.LoadScene(BATTLE_SCENE_NAME, LoadSceneMode.Single);
        }

        #endregion
    }

}

