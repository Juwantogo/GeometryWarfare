using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class OverrideNetDis : NetworkDiscovery {
 


    private void Awake()
    {
        base.Initialize();
        //base.StartAsClient();
        //StartCoroutine(CleanupExpired());
    }

    public void StartBroadcast()
    {
        StopBroadcast();
        base.Initialize();
        base.StartAsServer();
    }

    public override void OnReceivedBroadcast(string fromAddress, string data)
    {
        base.OnReceivedBroadcast(fromAddress, data);
        NetworkManager.singleton.networkAddress = fromAddress;
        NetworkManager.singleton.StartClient();
        foreach(KeyValuePair<string, NetworkBroadcastResult> i in broadcastsReceived)
        {
            Debug.Log(i.Key);
        }
        
    }

}
