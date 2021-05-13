using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEngine;

namespace MultiplayerMirror
{
    public class PlayerSpawnSystem : NetworkBehaviour
    {
        [SerializeField] private GameObject[] playerPrefabs = null;

        private static List<Transform> spawnPoints = new List<Transform>();

        private int nextIndex = 0;

        public static void AddSpawnPoint(Transform transform){
            spawnPoints.Add(transform);

            spawnPoints = spawnPoints.OrderBy(x => x.GetSiblingIndex()).ToList();
        }

        public static void RemoveSpawnPoint(Transform transform){
            spawnPoints.Remove(transform);
        }

        public override void OnStartServer(){
            NetworkManagerLobby.OnServerReadied += SpawnPlayer;
        }

        [ServerCallback]
        private void OnDestroy() {
            NetworkManagerLobby.OnServerReadied -= SpawnPlayer;
        }

        [Server]
        private void SpawnPlayer(NetworkConnection conn) {
            Transform spawnPoint = spawnPoints.ElementAtOrDefault(nextIndex);

            if(spawnPoint == null){
                Debug.LogError($"Missing spawn point for player {nextIndex}");
                return;
            }

            FactionType currentFactionType = FactionType.faction_1;
            foreach (GameObject playerEntity in playerPrefabs)
            {
                GameObject playerInstance = Instantiate(playerEntity ,playerEntity.transform.position + spawnPoints[nextIndex].position,spawnPoints[nextIndex].rotation);
                playerInstance.GetComponent<RtsEntity>().faction = currentFactionType;
                NetworkServer.Spawn(playerInstance,conn);
            }


            nextIndex++;
            currentFactionType = FactionType.faction_2;
        }
    }
}
