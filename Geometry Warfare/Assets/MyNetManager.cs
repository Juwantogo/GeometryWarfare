using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class MyNetManager : NetworkManager {
    [SerializeField]
    public string playerName;
    [SerializeField]
    public string team;
    [SerializeField]
    public string enemy;


    //public GameObject netObj;
    public int numOfPlayers = 0;
    public string gName;
    //public Material col;
    public Color col;

    // Use this for initialization
    void Start() { }



    // Update is called once per frame
    void Update() { }

    public void setIP(string adr)
    {
        this.networkAddress = adr;
    }
    public void setPort(string port)
    {
        this.networkPort = System.Int32.Parse(port);
    }

    public void setName(string n)
    {
        playerName = n;
    }
    public void setTeam()
    {
        Debug.Log("#" + numOfPlayers);
        team = "Blue Team";
        switch (numOfPlayers)
        {
            case 1:
                team = "Blue Team";
                enemy = "Red Team";
                break;
            case 2:
                team = "Red Team";
                enemy = "Blue Team";
                break;
            default:

                break;
        }
    }
    public void startHost()
    {
        toggleUI();
        StartHost();
    }
    public void startClient()
    {
        toggleUI();
        StartClient();
        //numOfPlayers++;
        //setTeam();
       
    }

    public override void OnStopClient()
    {
        toggleUI();
        numOfPlayers--;
    }

    public void startServer()
    {
        toggleUI();
        StartServer();
    }

    public void toggleUI()
    {
        this.transform.GetChild(0).gameObject.SetActive(!this.transform.GetChild(0).gameObject.activeSelf);
    }

    public string getName()
    {
        return playerName;
    }

    public string getTeam()
    {
        return team;
    }

    public string getEnemy()
    {
        return enemy;
    }









    // called when a new player is added for a client
    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {
    //Transform startPos = GetStartPosition();
     numOfPlayers++;
        setTeam();
        //gName = getName();
    //col = getColor().color;
    //playerPrefab.GetComponent<Renderer>().sharedMaterial.color = col;
    GameObject player = (GameObject)GameObject.Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
        player.name = getTeam();
    NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);
     }

    void OnPlayerDisconnected(NetworkPlayer player)
    {
       numOfPlayers--;
        Debug.Log("Clean up after player " + player);
        Network.RemoveRPCs(player);
        Network.DestroyPlayerObjects(player);
    }




    public override void OnServerConnect(NetworkConnection conn)
    {
        //numOfPlayers++;
        //Debug.Log("number" + numOfPlayers);
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        numOfPlayers--;
        NetworkServer.DestroyPlayersForConnection(conn);
    }
}
