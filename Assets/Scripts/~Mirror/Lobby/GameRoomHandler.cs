using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.UI;

namespace MultiplayerMirror
{
    public class GameRoomHandler : MonoBehaviour
    {
        [SerializeField] private NetworkManagerLobby networkManager = null;
        private const string URLFIREBASE = "https://tangrandeyenparojam-default-rtdb.firebaseio.com/Lobbies/";
        List<string[]> lobbies = new List<string[]>();
        List<string> lobbieHeader = new List<string>();
        [SerializeField]private GameObject gameRoomPrefab;
        [SerializeField] private GameObject landingPagePanel = null;
        private CanvasGroup canvaGroup;

        private void Awake() {
            canvaGroup = GetComponentInParent<Canvas>().rootCanvas.GetComponent<CanvasGroup>();
        }
        private void OnEnable() {
            
            NetworkManagerLobby.OnClientConnected +=  HandleClientConnected;
            NetworkManagerLobby.OnClientDisconnected +=  HandleClientDisconnected;

            GetLobbies();
        }
        private void OnDisable(){
            NetworkManagerLobby.OnClientConnected -=  HandleClientConnected;
            NetworkManagerLobby.OnClientDisconnected -=  HandleClientDisconnected;
        }

        private void HandleClientConnected() {
            canvaGroup.interactable = true;

            gameObject.SetActive(false);
            landingPagePanel.SetActive(false);
        }

        private void HandleClientDisconnected() {
            canvaGroup.interactable = true;
        }

        
        public void GetLobbies() {
            StartCoroutine(UnityGETLobbies());
        }

        public void OnFinishGettingLobbies(){
            int i = 0;
            foreach (string[] lobby in lobbies)
            {
                GameObject gameRoom = Instantiate(gameRoomPrefab,this.transform);
                gameRoom.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = lobby[0];
                Button button = gameRoom.transform.GetChild(0).GetComponent<Button>();
                button.onClick.AddListener(()=>{
                    
                    string ipAddress = lobby[1];
                    if(string.IsNullOrEmpty(ipAddress))
                        ipAddress = "localhost";
                    networkManager.networkAddress = ipAddress;
                    networkManager.serverRoomName = lobbieHeader[i];
                    networkManager.StartClient();

                   canvaGroup.interactable = false;
                });
                i++;
            }
        }

        IEnumerator UnityGETLobbies() {
            //Petition
            using (UnityWebRequest webRequest = UnityWebRequest.Get(URLFIREBASE + ".json")) {
                yield return webRequest.SendWebRequest();
                if (webRequest.isNetworkError) {
                    Debug.LogError("Error: " + webRequest.error);
                } else {
                    JSONNode data = JSON.Parse(webRequest.downloadHandler.text);
                    
                    foreach (JSONNode lobby in data) {
                        string[] lobbie = new string[3];
                        lobbie[0] = lobby["name"];
                        lobbie[1] = lobby["ipAddress"];
                        lobbie[2] = lobby["password"];
                        Debug.Log($"Address: {lobby["ipAddress"]}");
                        Debug.Log($"password: {lobby["password"]}");
                        lobbies.Add(lobbie);
                    }
                    OnFinishGettingLobbies();
                }
            }
        }
    }
}
