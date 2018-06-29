using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class MyNetworkManager : NetworkLobbyManager {
    public string playerName;
    public string teamName;
    public string enemyName;
    public Color thisColor;
    public int connections = 0;
    public Vector3 spawnSpot;
    public Vector3 mySpawnSpot;
    public GameObject RedCC, BlueCC, RedSpawn, BlueSpawn;
    public GameObject lobbyPlayer;
    //team, enemy, and color saved here
    // Use this for initialization
    void Start() {
        //  manager.teamName;
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetKey(KeyCode.Escape))
        {
            GetComponent<MyNetworkDiscovery>().endBroadcast();
        }
    }

    private Dictionary<int, structForServer.playerInfo> playerObjs = new Dictionary<int, structForServer.playerInfo>();

    public override void OnLobbyStartServer()
    {
        //called on the server when the server is started
        //numOfPlayers++;
        Debug.Log("onserverstart: " + networkAddress);
        Debug.Log("MatchNAME: " + matchName);

    }

    public void endGame()
    {
        GetComponent<MyNetworkDiscovery>().endBroadcast();
    }

    public override void OnLobbyClientAddPlayerFailed()
    {
        Debug.Log("Fail Max Players 2/2" + connections);
        GetComponent<MyNetworkDiscovery>().restartClient();

    }

    public void OnStartClient()
    {
        //called on client when disconnnected from server
        //restartClient end clients connection
        //restartClient calls StopBroadcast which ends a broadcast if it's running
        Debug.Log("start client");

        //NetworkDiscovery.StopBroadcast();
    }

    public override void OnStopClient()
    {
        //playerObjs.Remove(.connectionId);
        //called on client when disconnnected from server
        //restartClient end clients connection
        //restartClient calls StopBroadcast which ends a broadcast if it's running
        Debug.Log("stopped client");
        GetComponent<MyNetworkDiscovery>().restartClient();
        //NetworkDiscovery.StopBroadcast();
    }

    public override void OnStopServer()
    {
        Debug.Log("stopped server");
        //transform.GetComponent<MyNetworkDiscovery>().endBroadcast();
        //NetworkDiscovery.StopBroadcast();
    }

    public void OnLobbyClientConnect()
    {
        //Called on client when client connects to server
        //numOfPlayers++;

        //Debug.Log("lobMan: " + lobbyPlayerPrefab.GetComponent<lobbyPlayer>().playerName + ":" + lobbyPlayerPrefab.GetComponent<lobbyPlayer>().matchName);
        Debug.Log("onclientconnect: " + playerName);
        Debug.Log("onclientconnect: " + networkAddress);

    }

    public override GameObject OnLobbyServerCreateLobbyPlayer(NetworkConnection conn, short playerControllerId)
    {
        Debug.Log("lobbycreateplay numPlayers: " + numPlayers);
        //sets lobbyPlayer name to playername and matchname to matchname
        //variable changes here only changes the currently spawning object variables these spawning object won't have older objects names etc.
        //must call the functions in onstartclient on the prefab to run(which runs on all objects in server
        //play name will be whatever it was changed to on server
        //so we have to the netManager playerName value from local player OnStartLocalPlayer
        lobbyPlayer = (GameObject)Instantiate(lobbyPlayerPrefab.gameObject);
        lobbyPlayer.GetComponent<lobbyPlayer>().playerName = this.playerName;
        lobbyPlayer.GetComponent<lobbyPlayer>().matchName = this.matchName;
        //numPlayers controls color on server
        if (numPlayers == 0)
        {
            thisColor = lobbyPlayer.GetComponent<lobbyPlayer>().myColor = Color.red;
            teamName = "Red";
            enemyName = "Blue";
            mySpawnSpot = RedCC.transform.position;
            spawnSpot = RedSpawn.transform.position;
        }
        else if (numPlayers == 1)
        {
            thisColor = lobbyPlayer.GetComponent<lobbyPlayer>().myColor = Color.blue;
            teamName = "Blue";
            enemyName = "Red";
            mySpawnSpot = BlueCC.transform.position;
            spawnSpot = BlueSpawn.transform.position;
        }
        else
        {
            thisColor = lobbyPlayer.GetComponent<lobbyPlayer>().myColor = Color.green;
        }

        //-----------Add GameObjects to List for the given connection--------------
        //mySpawnSpot.y -= 55f;
        structForServer.playerInfo pI = new structForServer.playerInfo(playerName, teamName, enemyName, tag, thisColor, mySpawnSpot, spawnSpot);
        playerObjs.Add(conn.connectionId, pI);
        Debug.Log("servercreatelobbyplayer" + connections);
        //foreach (var i in lobbySlots)
        //{
        //    i.GetComponent<lobbyPlayer>().updateAll();
        //}
        return lobbyPlayer;
    }

    public override void OnLobbyServerPlayerRemoved(NetworkConnection conn, short playerControllerId)
    {
        //Destroy(playerObjs[conn.connectionId]);
        playerObjs.Remove(conn.connectionId);
        Debug.Log("RemoveTheting");
    }

    public override void OnLobbyServerDisconnect(NetworkConnection conn)
    {
        playerObjs.Remove(conn.connectionId);
        Debug.Log("disconnected");
    }

    public override void OnLobbyClientSceneChanged(NetworkConnection conn)
    {
        GetComponent<MyNetworkDiscovery>().LobbyCanvas.SetActive(false);
        //transform.GetChild(3).gameObject.SetActive(transform.GetChild(3).gameObject.activeInH);
        conn.isReady = true;
    }

    public override GameObject OnLobbyServerCreateGamePlayer(NetworkConnection conn, short playerControllerId)
    {
        structForServer.playerInfo thisPlayerInfo = playerObjs[conn.connectionId];
        GameObject myCommand = GameObject.Instantiate(gamePlayerPrefab, thisPlayerInfo.mySpawnSpot, Quaternion.identity);
        myCommand.GetComponent<networkPlayer>().playerName = thisPlayerInfo.playerName;
        myCommand.GetComponent<networkPlayer>().teamColor = thisPlayerInfo.teamName;
        //this.name = teamColor + " Team";
        myCommand.GetComponent<networkPlayer>().enemyColor = thisPlayerInfo.enemyName;
        myCommand.GetComponent<networkPlayer>().thisCol = thisPlayerInfo.thisColor;
        myCommand.GetComponent<networkPlayer>().spawnPosition = thisPlayerInfo.mySpawnSpot;
        myCommand.GetComponent<networkPlayer>().unitPosition = thisPlayerInfo.spawnSpot;
        //myCommand.GetComponent<networkPlayer>().nm = this;
        myCommand.GetComponent<Renderer>().material.color = thisPlayerInfo.thisColor;
        myCommand.name = thisPlayerInfo.teamName + "CC";
        myCommand.GetComponent<CommandClick>().team = thisPlayerInfo.teamName;
        myCommand.GetComponent<CommandClick>().enemy = thisPlayerInfo.enemyName;
        myCommand.GetComponent<CommandClick>().myCol = thisPlayerInfo.thisColor;
        myCommand.GetComponent<CommandClick>().gold = 4000;
        myCommand.GetComponent<CommandClick>().food = 100;
        myCommand.GetComponent<CommandClick>().health = 1000f;
        myCommand.GetComponent<CommandClick>().setHealth = 1000f;


        //myCommand.GetComponent<CommandClick>().nm = this;
        myCommand.layer = LayerMask.NameToLayer(thisPlayerInfo.teamName);
        if (thisPlayerInfo.teamName == "Red")
        {
            myCommand.tag = ("Red Command");
        }
        else
        {
            myCommand.tag = ("Blue Command");
        }
        //myCommand.GetComponent<NetworkIdentity>().AssignClientAuthority(conn);
        myCommand.GetComponent<CommandClick>().myTag = myCommand.tag;


        Debug.Log(teamName);
        return myCommand;
    }


}

class structForServer
{
    public struct playerInfo
    {
        public string playerName, teamName, enemyName, tag;
        public Color thisColor;
        public Vector3 mySpawnSpot, spawnSpot;

        public playerInfo(string playerName, string teamName, string enemyName, string tag, Color thisColor, Vector3 mySpawnSpot, Vector3 spawnSpot)
        {
            this.playerName = playerName;
            this.teamName = teamName;
            this.enemyName = enemyName;
            this.tag = tag;
            this.thisColor = thisColor;
            this.mySpawnSpot = mySpawnSpot;
            this.spawnSpot = spawnSpot;
        }
    }
}