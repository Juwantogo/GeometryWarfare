using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class networkPlayer : NetworkBehaviour {
    public GameObject prefab;
    public bool spawned = false;
    public MyNetworkManager manager;
    [SyncVar]
    public int playerCount;
    [SyncVar]
    public GameObject enemy;
    [SyncVar]
    public int points;
    [SyncVar]
    public string playerName;
    [SyncVar]
    public string teamColor;
    [SyncVar]
    public string enemyColor;
    //[SyncVar]
    public Color thisCol;
    public MyNetworkManager nm;
    public Vector3 spawnPosition;
    public Vector3 unitPosition;
    public Canvas canvasA;
    GameObject myCommand;


    // Use this for initialization
    void Start () {
        //canvasA = GameObject.Find("HUD").GetComponent<Canvas>();
        //canvasA.enabled = false;
        //playerList = FindObjectsOfType<networkPlayer>();
        //if (isServer)
        //{
            manager = GameObject.Find("Network Manager").GetComponent<MyNetworkManager>();
    }

    public override void OnStartClient()
    {
    }

    private IEnumerator slowUpdate()
    {
        while (connectionToClient.isReady)
            yield return 1f;

    }

    // Update is called once per frame
    void Update () {
        //Debug.Log(teamColor);
        if (isLocalPlayer)
        {
            if (teamColor != "")
            {
                this.gameObject.name = teamColor;
                if(manager == null)
                {
                    manager = GameObject.Find("Network Manager").GetComponent<MyNetworkManager>();
                }
                manager.transform.GetChild(3).gameObject.SetActive(true);
                manager.transform.GetChild(3).GetChild(3).GetComponent<Text>().text = teamColor;
                //canvasA.transform.GetChild(0).GetComponent<Text>().text = teamColor;
            }
        }
        this.gameObject.name = teamColor;
    }

    [Command]
    void CmdrequestAuthority()
    {
    }

    [ClientRpc]
    void Rpc_setTeam(string team)
    {
        teamColor = team;
    }
}
