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
        #region ���� �ݹ�
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
            Logger.Log("���� ����");
        }

        private void OnOnServerStoppedCallback(bool obj)
        {
            Logger.Log("���� ����");
        }

        private void OnClientStartedCallback()
        {
            Logger.Log("Ŭ���̾�Ʈ ����");
        }


        #endregion

        #region ��ư �ݹ�
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

