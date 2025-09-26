using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using Logger = LittelSword.Common.Logger;
using Random = UnityEngine.Random;

namespace LittelSword.Network
{
    public class NetworkGameManager : MonoBehaviour
    {
        [SerializeField] private GameObject enemyPrefab;
        [SerializeField] private Transform[] spawnPoints;
        [SerializeField] private GameObject playerPrefab;
        [SerializeField] private Button leaveSessionButton;

        private void Start()
        {
            if (NetworkManager.Singleton.IsServer)
            {
                SpawnEnemies();
                SpawnPlayers();

                // 새로 연결되는 클라이언트 콜백
                NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
            }

            //NetworkManager.Singleton.OnServerStarted += SpawnEnemies;
        }
        private void OnEnable()
        {
            leaveSessionButton.onClick.AddListener(LeaveSession);
        }
        

        private void OnDisable()
        {
            leaveSessionButton.onClick.RemoveAllListeners();
        }
        private void LeaveSession()
        {
            MultiplayerSessionManager.Instance.LeaveSession();
        }

        private void OnDestroy()
        {
            if (NetworkManager.Singleton == null)
                return;

            if (NetworkManager.Singleton.IsServer)
            {
                NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
            }
        }
        private void OnClientConnected(ulong playerId)
        {
            Logger.Log($"클라이언트 접속:{playerId}");
            SpawnPlayer(playerId);
        }

        private void SpawnPlayers()
        {
            Logger.Log($"접속 플레이어 수 : {NetworkManager.Singleton.ConnectedClients.Count}");

            foreach (var clientId in NetworkManager.Singleton.ConnectedClientsIds)
            {
                var spawnPosition = new Vector3(
                    Random.Range(-2f, 2f),
                    Random.Range(-2f, 2f),
                    0);
                var player = Instantiate(playerPrefab, spawnPosition, Quaternion.identity);

                player.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId);
            }
        }
        private void SpawnPlayer(ulong playerId)
        {
            Logger.Log($"접속 플레이어 수 : {NetworkManager.Singleton.ConnectedClients.Count}");

            var spawnPosition = new Vector3(
                    Random.Range(-2f, 2f),
                    Random.Range(-2f, 2f),
                    0);
            var player = Instantiate(playerPrefab, spawnPosition, Quaternion.identity);

            player.GetComponent<NetworkObject>().SpawnAsPlayerObject(playerId);
        }

        private void SpawnEnemies()
        {

            if (!NetworkManager.Singleton.IsServer)
                return;

            foreach (var point in spawnPoints)
            {
                var enemy = Instantiate(enemyPrefab, point.position, point.rotation);

                // 네트워크 객체로 등록
                enemy.GetComponent<NetworkObject>().Spawn();
            }
        }
    }

}
