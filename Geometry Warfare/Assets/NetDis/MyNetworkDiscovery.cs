using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MyNetworkDiscovery : NetworkDiscovery {
    public GameListController glControl;
    public GameObject serverInput,serverB,clientB,endB, LoadCanvas, playerInput, LobbyCanvas, StartCanvas, thisNetworkDis;
    public Text serverNameInput, playerNameInput;
    public MyNetworkManager netmanager;
    public int connections;
    private float timeout = 5f;

    private Dictionary<ConnectionInfo, float> addresses = new Dictionary<ConnectionInfo, float>();


    public void Start()
    {
    }

    public void Awake()
    {
        Initialize();
        glControl = this.transform.GetComponent<GameListController>();
        playerInput = GameObject.Find("PlayerName");
        serverInput = GameObject.Find("ServerName");
        serverB = GameObject.Find("Server Broadcast");
        clientB = GameObject.Find("Client Broadcast");
        endB = GameObject.Find("End Broadcast");
        LoadCanvas = GameObject.Find("LoadCanvas");
        LobbyCanvas = GameObject.Find("LobbyCanvas");
        StartCanvas = GameObject.Find("StartCanvas");

        transform.GetChild(3).gameObject.SetActive(false);
        netmanager = GetComponent<MyNetworkManager>();
        thisNetworkDis = this.gameObject;
        endB.SetActive(false);
        LoadCanvas.SetActive(false);
        LobbyCanvas.SetActive(false);
        serverNameInput = serverInput.transform.GetChild(1).GetComponent<Text>();
        playerNameInput = playerInput.transform.GetChild(1).GetComponent<Text>();
    }

    private IEnumerator CleanUp()
    {
        while (true)
        {
            bool changed = false;

            var keys = addresses.Keys.ToList();
            foreach(var key in keys)
            {
                if (addresses[key] <= Time.time)
                {
                    addresses.Remove(key);
                    changed = true;
                }
                //Debug.Log(addresses[key]);
            }
            if (changed)
            {
                
                UpdateInfo();
            }
            yield return new WaitForSeconds(timeout);
        }
    }

    public void startClient()
    {
        StopAllCoroutines();
        StartCoroutine(CleanUp());
        StartCanvas.SetActive(true);
        LoadCanvas.SetActive(false);
        LobbyCanvas.SetActive(false);
        playerInput.SetActive(false);
        serverInput.SetActive(false);
        serverB.SetActive(false);
        clientB.SetActive(false);
        endB.SetActive(true);
        Initialize();
        netmanager.playerName = playerNameInput.text;
        StartAsClient();
    }

    public void restartClient()
    {

        //stop corouts,broadcast, start main screen, clear address so main screen is empty call update info to portray the empty field
        //set other canvases to false, and set original inputs and server/client buttons to true
        //      StopAllCoroutines();
        //StopBroadcast();
        //       addresses.Clear();
        //glControl.RemoveGames();
        StopBroadcast();
        Destroy(thisNetworkDis);
        SceneManager.LoadScene("offline");
    }

    public void startServer()
    {
        //gamename:playername:numon server:
        if(serverNameInput.text.Length < 1 || playerNameInput.text.Length < 1 || serverNameInput.text.Contains(":") || playerNameInput.text.Contains(":"))
        {
            Debug.Log("Server and Player Name must be filled and can't contain the symbol ':'");
        }
        else
        {
            playerInput.SetActive(false);
            serverInput.SetActive(false);
            serverB.SetActive(false);
            clientB.SetActive(false);
            endB.SetActive(true);
            LoadCanvas.SetActive(false);
            LobbyCanvas.SetActive(true);
            StartCanvas.SetActive(false);
            Initialize();
            broadcastData = serverNameInput.text + ":" + playerNameInput.text + ":" + 0;
            StartAsServer();
            NetworkLobbyManager.singleton.matchName = serverNameInput.text;
            netmanager.playerName = playerNameInput.text;
            NetworkLobbyManager.singleton.StartHost();
        }

    }
    //m_msgOutBuffer needs to be set to null on broadcast stop
    public void endBroadcast()
    {
        //StopClient();
        playerInput.SetActive(true);
        LoadCanvas.SetActive(true);
        serverInput.SetActive(true);
        serverB.SetActive(true);
        clientB.SetActive(true);
        endB.SetActive(true);
        LobbyCanvas.SetActive(true);
        StartCanvas.SetActive(true);
        //StopBroadcast();
        try
            {
            Debug.Log("stophost");
            NetworkLobbyManager.singleton.StopHost();
        }
        catch
        {
            Debug.Log("No Server In Lobby");
        }

            try
            {
                Debug.Log("stop client");
                NetworkLobbyManager.singleton.StopClient();
            }
            catch
            {
                Debug.Log("No Client In Lobby");

            }


        SceneManager.LoadScene("offline");
    }

    public override void OnReceivedBroadcast(string fromAddress, string data)
    {
        base.OnReceivedBroadcast(fromAddress, data);
        //NetworkServerSimple.messageBuffer;
        byte[] asciiBytes = Encoding.ASCII.GetBytes(data);
        // look for the 1st zero
        int endOfByte = 0;
        foreach (var item in asciiBytes)
        {
            if (item == 0)
            {
                break;
            }
            endOfByte++;
        }
        data = System.Text.Encoding.ASCII.GetString(asciiBytes, 0, endOfByte);
        broadcastData = data;

        ConnectionInfo info = new ConnectionInfo(fromAddress, broadcastData);
        if(addresses.ContainsKey(info) == false)
        {
            addresses.Add(info, Time.time + timeout);
            UpdateInfo();
        }
        else
        {
            //info = time
            addresses[info] = Time.time + timeout;
        }

        //from address = ::ffff:192.168.0.96
    }

    void UpdateInfo()
    {
        Debug.Log("Remove");
            glControl.AddGames(addresses.Keys.ToList());
    }

}

