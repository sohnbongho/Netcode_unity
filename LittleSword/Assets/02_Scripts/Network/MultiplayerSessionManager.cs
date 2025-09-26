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
        // Multiplayer : Lobby, Relay, Matchmking 통합관리하는 SDK
        // ISession : 세션의 모든 이벤트, 속성
        private ISession ActiveSession { get; set; }

        private const string BATTLE_SCENE_NAME = "Level01";
        private const string LOBBY_SCENE_NAME = "Lobby";

        #region 유니티 이벤트
        private async void Start()
        {
            try
            {
                await UnityServices.InitializeAsync();
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
                Logger.Log($"익명 로그인 성공");
            }
            catch (Exception ex)
            {
                Logger.Log(ex.Message);
            }
        }
        #endregion

        #region Session 관련 메소드
        public async void CreateSessionAsync(string sessionName)
        {
            try
            {
                // 세션 옵션 설정
                var options = new SessionOptions
                {
                    Name = sessionName,
                    IsPrivate = false,
                    IsLocked = false,
                    MaxPlayers = 4
                }.WithRelayNetwork();

                // 세션 생성 및 호스트 시작
                ActiveSession = await MultiplayerService.Instance.CreateSessionAsync(options);
                Logger.Log($"세션 생성 완료:{ActiveSession.Name} - {ActiveSession.Code}");
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
                    CreateSession = true, // 5초가 넘으면 내가 session을 만들겠다.
                };

                // Session Options
                var sessionOptions = new SessionOptions
                {
                    IsPrivate = false,
                    IsLocked = false,
                    MaxPlayers = 4
                }.WithRelayNetwork();

                // 퀵 조인 시도
                ActiveSession = await MultiplayerService.Instance.MatchmakeSessionAsync(joinOptions,
                    sessionOptions);
                Logger.Log($"세션 조인 완료:{ActiveSession.Name} - {ActiveSession.Code}");
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

