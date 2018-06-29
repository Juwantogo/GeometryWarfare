using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Server : NetworkDiscovery
{

    public void Start()
    {

        Initialize();
        //StartAsClient();
        //Debug.Log("Client? - " + running);

    }

    public void startServer()
    {
        StartAsServer();
    }

/*
    public override void OnReceivedBroadcast(string fromAddress, string data)
    {
        base.OnReceivedBroadcast(fromAddress, data);
        Debug.Log("blah" + fromAddress);
        NetworkManager.singleton.networkAddress = fromAddress;
        NetworkManager.singleton.StartClient();
    }

    public void OnServerInitialized()
    {
        Debug.Log("Server Init");
        //return
    }
    */
}
