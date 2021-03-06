using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;
using System.Linq;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

namespace MultiplayerMirror {
    public class NetworkManagerLobby : NetworkManager {
        [SerializeField] private int minPlayers = 2;
        [Scene] [SerializeField] private string menuScene = string.Empty;

        [Header("Room")]
        [SerializeField] private NetworkRoomPlayerLobby roomPlayerPrefab = null;
        public List<NetworkRoomPlayerLobby> RoomPlayers { get; } = new List<NetworkRoomPlayerLobby>();

        [Header("Game")]
        [SerializeField] private NetworkGamePlayer networkGamePlayerPrefab = null;
        [SerializeField] private GameObject playerSpawnSystem = null;
        public List<NetworkGamePlayer> GamePlayers { get; } = new List<NetworkGamePlayer>();

        public static Action OnClientConnected;
        public static Action OnClientDisconnected;
        public static Action<NetworkConnection> OnServerReadied;
        public string serverRoomName = "nullPointer";

#region Server
        public override void OnStartServer() {
            spawnPrefabs = Resources.LoadAll<GameObject>("SpawnablePrefabsRTS").ToList();
        }
        
        public override void OnServerConnect(NetworkConnection conn) {
            if (numPlayers >= maxConnections) {
                conn.Disconnect();
                return;
            }
            if (SceneManager.GetActiveScene().path != menuScene) {
                conn.Disconnect();
                return;
            }
        }
        
        public override void OnServerAddPlayer(NetworkConnection conn) {
            if (SceneManager.GetActiveScene().path == menuScene) {
                Debug.Log("OnServerAddPlayerRoom");
                bool isLeader = RoomPlayers.Count == 0;

                NetworkRoomPlayerLobby roomPlayerInstance = Instantiate(roomPlayerPrefab);

                roomPlayerInstance.IsLeader = isLeader;

                NetworkServer.AddPlayerForConnection(conn, roomPlayerInstance.gameObject);
            }
        }
        public override void OnServerDisconnect(NetworkConnection conn) {
            if (conn.identity != null) {
                var player = conn.identity.GetComponent<NetworkRoomPlayerLobby>();

                RoomPlayers.Remove(player);

                NotifyPlayersOfReadyState();
            }

            base.OnServerDisconnect(conn);
        }

        public override void OnStopServer() {
            RoomPlayers.Clear();
            GamePlayers.Clear();
            StartCoroutine(deleteRoomFromFirebase());
        }

        IEnumerator deleteRoomFromFirebase(){
            var request = new UnityWebRequest("https://tangrandeyenparojam-default-rtdb.firebaseio.com/Lobbies/" + serverRoomName + ".json", "DELETE");
            request.SetRequestHeader("Content-Type", "application/json");
            yield return request.SendWebRequest();
        }
#endregion

#region Client

        public override void OnStartClient() {
            var spawnPrefabs = Resources.LoadAll<GameObject>("SpawnablePrefabsRTS");

            foreach (var prefab in spawnPrefabs) {
                NetworkClient.RegisterPrefab(prefab);
            }
        }
        public override void OnClientConnect(NetworkConnection conn) {
            base.OnClientConnect(conn);
            OnClientConnected?.Invoke();
        }

        public override void OnClientDisconnect(NetworkConnection conn) {
            base.OnClientDisconnect(conn);
            OnClientDisconnected?.Invoke();
        }


        public void NotifyPlayersOfReadyState() {
            foreach (var player in RoomPlayers) {
                player.HandleReadyToStart(IsReadyToStart());
            }
        }
#endregion 
        private bool IsReadyToStart() {
            if (numPlayers < minPlayers)
                return false;

            foreach (var player in RoomPlayers) {
                if (!player.IsReady)
                    return false;
            }

            return true;
        }



        #region Gameplay

        public void StartGame() {
            if (SceneManager.GetActiveScene().path == menuScene) {
                if (!IsReadyToStart())
                    return;

                ServerChangeScene("Main");
            }
        }

        public override void ServerChangeScene(string newSceneName) {
            if (SceneManager.GetActiveScene().path == menuScene && newSceneName.StartsWith("Main")) {
                for (int i = RoomPlayers.Count-1; i >= 0; i--) {
                    var conn = RoomPlayers[i].connectionToClient;
                    var gameplayerInstance = Instantiate(networkGamePlayerPrefab);
                    gameplayerInstance.SetDisplayName(RoomPlayers[i].DisplayName);

                    NetworkServer.Destroy(conn.identity.gameObject);

                    NetworkServer.ReplacePlayerForConnection(conn, gameplayerInstance.gameObject);
                }
            }
            base.ServerChangeScene(newSceneName);
        }

        public override void OnServerSceneChanged(string sceneName) {
            if(sceneName.StartsWith("Main")){
                GameObject playerSpawnSystemInstance = Instantiate(playerSpawnSystem);
                NetworkServer.Spawn(playerSpawnSystemInstance);
            } 
        }

        public override void OnServerReady(NetworkConnection conn){
            base.OnServerReady(conn);
            
            OnServerReadied?.Invoke(conn);
        }
        #endregion

    }
}
