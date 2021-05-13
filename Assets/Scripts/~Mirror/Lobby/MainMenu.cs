using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using SimpleJSON;
using UnityEngine;
using UnityEngine.Networking;

namespace MultiplayerMirror {
    public class MainMenu : MonoBehaviour {
        [SerializeField] private NetworkManagerLobby networkManager = null;

        [Header("UI")]
        [SerializeField] private GameObject landingPagePanel = null;

        private string URLFIREBASE = "https://tangrandeyenparojam-default-rtdb.firebaseio.com/Lobbies/";
        public void HostLobby() {
            networkManager.StartHost();
            CreateLobby(LocalIPAddress(), "123456");
        }
 

        private string LocalIPAddress() {
            IPHostEntry host;
            string localIP = "0.0.0.0";
            host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList) {
                if (ip.AddressFamily == AddressFamily.InterNetwork) {
                    localIP = ip.ToString();
                    break;
                }
            }
            return localIP;
        }


        public void CreateLobby(string ipAddress, string password) {
            StartCoroutine(UnityRequestSingUp(ipAddress,password));
        }

        IEnumerator UnityRequestSingUp(string ipAddress, string password) {

            string dq = ('"' + "");
            string bodyJsonString = "{" + dq + "name" + dq + ":" + dq + ("newRoom" + Random.Range(0,10)) + dq + ","+ dq + "ipAddress" + dq + ":" + dq + (ipAddress) + dq + "," + dq + "password" + dq + ":" + dq + (password) + dq + "}";

            var request = new UnityWebRequest(URLFIREBASE + ".json" , "POST");
            byte[] bodyRaw = Encoding.UTF8.GetBytes(bodyJsonString);
            request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            yield return request.SendWebRequest();
            StartCoroutine(UnityGETLobbies());
            
        }

        IEnumerator UnityGETLobbies() {
            //Petition
            using (UnityWebRequest webRequest = UnityWebRequest.Get(URLFIREBASE + ".json")) {
                yield return webRequest.SendWebRequest();
                if (webRequest.isNetworkError) {
                    Debug.LogError("Error: " + webRequest.error);
                } else {
                    GetRoomHeaders(webRequest.downloadHandler.text);
                }
            }
        }

        private void GetRoomHeaders(string pureJson){
            if(pureJson.IndexOf("-M")!= -1){
                string header = pureJson.Substring(pureJson.IndexOf("-M"), 20);
                
                pureJson = pureJson.Substring(pureJson.IndexOf("-M")+20);
                if(pureJson.IndexOf("-M")!= -1){
                    GetRoomHeaders(pureJson);
                }
                else{
                    networkManager.serverRoomName = header;
                    landingPagePanel.SetActive(false);
                }
            }
        }
    }
}
