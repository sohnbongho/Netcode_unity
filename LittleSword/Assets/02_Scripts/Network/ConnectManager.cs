using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using Logger = LittelSword.Common.Logger;

namespace LittleSword.Network
{
    public class ConnectManager : MonoBehaviour
    {
        [SerializeField] private Button serverButton;
        [SerializeField] private Button hostButton;
        [SerializeField] private Button clientButton;
        [SerializeField] private Button closeButton;

        private void Start()
        {
            serverButton.onClick.AddListener(OnClickServer);
            hostButton.onClick.AddListener(OnClickHost);
            clientButton.onClick.AddListener(OnClickClient);
            closeButton.onClick.AddListener(OnClickClose);

            BindServerCallbacks();
        }
        private void OnDisable()
        {
            serverButton.onClick.RemoveListener(OnClickServer);
            hostButton.onClick.RemoveListener(OnClickHost);
            clientButton.onClick.RemoveListener(OnClickClient);
            closeButton.onClick.RemoveListener(OnClickClose);

            UnBindServerCallbacks();
        }
        #region 서버 콜백
        private void BindServerCallbacks()
        {
            NetworkManager.Singleton.OnServerStarted += OnServerStartedCallback;
            NetworkManager.Singleton.OnServerStopped += OnOnServerStoppedCallback;
            NetworkManager.Singleton.OnClientStarted += OnClientStartedCallback;

        }

        private void UnBindServerCallbacks()
        {
            if (NetworkManager.Singleton == null)
                return;

            NetworkManager.Singleton.OnServerStarted -= OnServerStartedCallback;
            NetworkManager.Singleton.OnServerStopped -= OnOnServerStoppedCallback;
            NetworkManager.Singleton.OnClientStarted -= OnClientStartedCallback;

        }
        private void OnServerStartedCallback()
        {
            Logger.Log("서버 시작");
        }

        private void OnOnServerStoppedCallback(bool obj)
        {
            Logger.Log("서버 종료");
        }

        private void OnClientStartedCallback()
        {
            Logger.Log("클라이언트 시작");
        }


        #endregion

        #region 버튼 콜백
        private void OnClickServer()
        {
            NetworkManager.Singleton.StartServer();
        }

        private void OnClickHost()
        {
            NetworkManager.Singleton.StartHost();
        }

        private void OnClickClient()
        {
            NetworkManager.Singleton.StartClient();
        }

        private void OnClickClose()
        {
            NetworkManager.Singleton.Shutdown();
        }
        #endregion
    }
}

