using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class lobbyPlayer : NetworkLobbyPlayer {
    //HOOK IS A FUNCTION USED THAT IS CALLED ON ALL THE SAME OBJECTS ON SERVER AND CLIENT WHERE THE SYNCVAR CHANGES
    [SyncVar(hook = "OnMyName")]
    public string playerName;
    [SyncVar]
    public string matchName;
    [SyncVar]
    public bool readySync;
    public GameObject playerNameHolder;
    public GameObject lobbyHeader;
    public GameObject nameInputField;
    public GameObject kickButton;
    public GameObject readyButton;
    public GameObject readyLight;
    public Image teamIMGcolor;
   [SyncVar(hook = "OnMyColor")]
    public Color myColor;

    public Transform contentHolder;
    public Text textObjHeader;
    private bool setup = false;

    // Use this for initialization
    void Start () {
        //LOBBY HEADER FOR GAME NAME (GET, FIND TEXT, SET TEXT)
        lobbyHeader = GameObject.Find("LobbyHeader");
        textObjHeader = lobbyHeader.transform.GetChild(0).GetComponent<Text>();
        textObjHeader.text = matchName;
        //HOLDER TO PLACE LOBBY PLAYER IN FOR SCROLLVIEW
        contentHolder = GameObject.Find("PlayersContent").transform;
        transform.SetParent(contentHolder);
        //gets name from netManager of the client ++ COULD ADD ANOTHER LINE TOO IS COLOR IS HANDLED BY PLAYER
        playerName = transform.root.GetComponent<MyNetworkManager>().playerName;
        //USED TO CHECK IF GAMEOBJECT NAMES ARE SET TO GET USER PRESET VALUES
        setup = true;
    }


    //LOCAL FUNCTION FOR LOCAL PLAYER TO START COROUTINE AND SETUP BUTTONS ----------------------------------------------------------------------------------------------------------------------------------
    public override void OnStartLocalPlayer()
    {
      //Only Local
        StartCoroutine(getInfo());
        SetupLocalPlayer();
    }

    //FUNCTION FOR ALL LobbyPLAYERS WHEN STARTED ON CLIENT ----------------------------------------------------------------------------------------------------------------------------------
    public override void OnStartClient()
    {
        //runs when client is started before start
        //runs on all these objects on server to get all objects info
        base.OnStartClient();
        //Called again to get old/Already spawned objects names/colors
        OnMyName(playerName);
        OnMyColor(myColor);
        // if (isLocalPlayer)
        //{
        //    Debug.Log("localPlayer" + readyToBegin);
        //CmdChangeColor(readyToBegin);
        // }
        OnClientReady(readySync);
    }


    //COROUTINE FOR LOCAL HOST TO SEND NAME TO SERVER ---------------------------------------------------------------------------------------------------------
    private IEnumerator getInfo()
    {
        //localplayer coroutine
        //runs after start has been done - so that playername is set etc.
        while (!setup)
        {
            yield return null;
            // if()
        }
        //waits til start is done to get the netMan playerName and sends to server so it knows the name 
        //w/o this the server will make this it's name(the host's name)
        CmdNameChanged(playerName);
        //Dont have to call this because server controls when started
        //CmdChangeColor(myColor);
    }

    private IEnumerator kickPlayer()
    {
        nameInputField.SetActive(false);
        readyButton.SetActive(false);
        SendNotReadyToBeginMessage();
        yield return new WaitForSeconds(5f);
        RemovePlayer();
        //Destroy(this.gameObject);
        //transform.root.GetComponent<MyNetworkDiscovery>().restartClient();
        NetworkLobbyManager.singleton.StopClient();
    }




    //SETTING UP LOBBY PLAYER BUTTONS ----------------------------------------------------------------------------------------------------------------------------------
    public override void OnClientEnterLobby()
    {
        //called on all clients when a new lobyplayer enters
        //
        base.OnClientEnterLobby();
        if (isLocalPlayer)
        {
            //this is localplayer remove kick button
            SetupLocalPlayer();
           // isReady();
        }
        else if(isServer)
        {
            //this is server? remove ready and name input
            SetupOtherPlayer();
        }
        else
        {
            //this isn't server or local host? remove ready, name input, and kick button
            kickButton.SetActive(false);
            SetupOtherPlayer();
        }
        //OnClientReady(readyToBegin);
    }

    void SetupOtherPlayer()
    {
        nameInputField.SetActive(false);
        readyButton.SetActive(false);
        //OnClientReady(false);
        //isReady();
    }

    void SetupLocalPlayer()
    {
        nameInputField.SetActive(true);
        readyButton.SetActive(true);
        kickButton.SetActive(false);
        //isReady();
    }




    //USER CONTROLED INPUTS ----------------------------------------------------------------------------------------------------------------------------------
    //Called if you want user to changename
    [Command]
    public void CmdNameChanged(string name)
    {
        playerName = name;
    }

    //Called if you want user to control color
    [Command]
    public void CmdChangeColor()
    {
        RpcsendServerReady();

        //myColor = myColor;
    }

    [TargetRpc]
    public void TargetSendRemove(NetworkConnection target)
    {
        StartCoroutine(kickPlayer());
        Debug.Log("remove");
    }

    [ClientRpc]
    public void RpcsendServerReady()
    {
        if (isLocalPlayer)
        {

            Debug.Log("ready" + readyToBegin);
        }
    }

    // Update is called once per frame
    void Update () {
        if (!isLocalPlayer)
        {
            //Debug.Log("iS Server Ready?" + readySync);
        }
    }

    //SendReadyToBeginMessage();


    //HOOK FUNCTIONS FROM CMD CALLS ----------------------------------------------------------------------------------------------------------------------------------
    //Hook functions that gets called on server object but also the correct client object with the same id
    //this is called in onstartclient which runs on all player ocjects containing this script on the server
    // the value is sync var which is why it changes everywhere where this player object script is
    //gets already spawned objects values
    public void OnMyName(string newName)
    {
        playerName = newName;
        //transform.root.GetComponent<MyNetworkManager>().playerName = newName;
        playerNameHolder.GetComponent<Text>().text = playerName;
    }

    public void OnMyColor(Color newColor)
    {
        myColor = newColor;
        teamIMGcolor.GetComponent<Image>().color = myColor;
    }

    //Interaction Buttons(Name Change, Kick, Ready)
    public void OnNameChanged(string str)
    {
        //Debug.Log("check name" + str);
        CmdNameChanged(str);
    }

    public void OnKick()
    {
        //Debug.Log("check name" + str);
        OnMyName("YOU HAVE BEEN KICKED");
        //playerNameHolder.GetComponent<Text>().text = "YOU HAVE BEEN KICKED";
        //System.Threading.Thread.Sleep(3000); // 3 sec.
        TargetSendRemove(connectionToClient);
        //transform.root.GetComponent<MyNetworkDiscovery>().restartClient();
        // NetworkLobbyManager.singleton.KickPlayer(connectionToClient);
    }

    public override void OnClientReady(bool readyState)
    {
        Debug.Log("iS Server Ready?" + readySync);

        if (readyState)
        {
            readyButton.transform.GetChild(0).GetComponent<Text>().text = "Unready";
            readyLight.GetComponent<Image>().color = Color.green;
            readySync = true;

        }
        else
        {
            readyButton.transform.GetChild(0).GetComponent<Text>().text = "Ready";
            readyLight.GetComponent<Image>().color = Color.red;
            readySync = false;
        }
    }

    //function to change ready
    public void OnReady()
    {
        if (readyToBegin)
        {
            SendNotReadyToBeginMessage();
        }
        else
        {
            SendReadyToBeginMessage();
        }
    }

    //function to check ready and send status


}

