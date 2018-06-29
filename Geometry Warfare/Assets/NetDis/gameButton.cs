using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class gameButton : MonoBehaviour {
    public string address;
    public string data;
    public MyNetworkDiscovery originalNetDis;
    //public string myName;
    // Use this for initialization
    void Start() {
        originalNetDis = transform.root.GetComponent<MyNetworkDiscovery>();
    }

    // Update is called once per frame
    void Update() {

    }

    public void connectToGame(){
        originalNetDis.LobbyCanvas.SetActive(true);
        originalNetDis.StartCanvas.SetActive(false);
        NetworkLobbyManager.singleton.networkAddress = address;
        Debug.Log("#:  " + NetworkLobbyManager.singleton.numPlayers);
        NetworkLobbyManager.singleton.StartClient();
    }
}
