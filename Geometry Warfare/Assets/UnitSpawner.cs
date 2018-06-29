using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class UnitSpawner : CommandClick {

    public GameObject farmPrefab;
    public GameObject heavyPrefab;
    public GameObject siegePrefab;
    public GameObject lightPrefab;
    public CommandClick parentScript;
    public Color teamColor;
    public NetworkConnection myConn;
    //public string team;
    // Use this for initialization
    void Start () {
        GameObject.Find("SpawnSpot");
	}
	
	// Update is called once per frame
	void Update () {

    }

    public void spawnFarm()
    {
        //GameObject newFarm = GameObject.Instantiate(farmPrefab, spawnPosition, Quaternion.identity);
        //newFarm.GetComponent<FarmScript>().commandLoc = spawnPosition;
        //newFarm.GetComponent<FarmScript>().targetPoint = pointOnCircumference;
        //newFarm.GetComponent<FarmScript>().drawnCircle = LineDrawer;
       // newFarm.GetComponent<FarmScript>().distance = distance;
       // newFarm.tag = team + " Farm";
        //NetworkServer.SpawnWithClientAuthority(newFarm, myConn);
        //NetworkServer.Spawn(newFarm);
        parentScript.makeMenu(3);
        //   parentScript.makeMenu(newFarm);
        //moveObject = ture;
        thisMenu.SetActive(false);
        //LineDrawer.enabled = false;

    }

    public void spawnHeavy()
    {
        //Vector3 higherSpawn = spawnPosition;
        //higherSpawn.y = 52.5f;
        /*
        GameObject newHeavy = GameObject.Instantiate(heavyPrefab, spawnPosition, Quaternion.identity);
        newHeavy.GetComponent<UnitManager>().team = team;
        newHeavy.GetComponent<UnitManager>().enemy = enemy;
        newHeavy.tag = team;
        foreach (var i in newHeavy.GetComponentsInChildren<Renderer>())
        {
            i.material.color = myCol;
        }*/
        // newHeavy.
        //NetworkServer.Spawn(newHeavy);
        // if(GameObject.Find(team) != null)
        // {
        //NetworkServer.SpawnWithClientAuthority(newHeavy, myConn);
        parentScript.makeMenu(0);

        //  parentScript.makeMenu(newHeavy);
        //}
        //newHeavy.GetComponent<FarmScript>().commandLoc = spawnPosition;
        //moveObject = ture;
        thisMenu.SetActive(false);
        LineDrawer.enabled = false;
    }

    public void spawnSiege()
    {
        //Vector3 higherSpawn = spawnPosition;
        //higherSpawn.y = 52.5f;
    /*
        GameObject newSiege = GameObject.Instantiate(siegePrefab, spawnPosition, Quaternion.identity);
        newSiege.GetComponent<UnitManager>().team = team;
        newSiege.GetComponent<UnitManager>().enemy = enemy;
        newSiege.tag = team;
        foreach (var i in newSiege.GetComponentsInChildren<Renderer>())
        {
            i.material.color = myCol;
        }
        */
        // if (GameObject.Find(team) != null)
        // {
        Debug.Log(myConn);
        //myConn.
        //NetworkServer.SpawnWithClientAuthority(newSiege, myConn);
        //NetworkServer.Spawn(newSiege);
        // parentScript.makeMenu(newSiege);
        //CmdspawnObj(newSiege);
        //}
        parentScript.makeMenu(1);

        //newHeavy.GetComponent<FarmScript>().commandLoc = spawnPosition;
        //moveObject = ture;
        thisMenu.SetActive(false);
        LineDrawer.enabled = false;
    }


    public void spawnLight()
    {
        //Vector3 higherSpawn = spawnPosition;
        //higherSpawn.y = 52.5f;
/*
        GameObject newLight = GameObject.Instantiate(lightPrefab, spawnPosition, Quaternion.identity);
        newLight.GetComponent<UnitManager>().team = team;
        newLight.GetComponent<UnitManager>().enemy = enemy;
        newLight.tag = team;
        foreach (var i in newLight.GetComponentsInChildren<Renderer>())
        {
            i.material.color = myCol;
        }
        */
        //NetworkServer.Spawn(newLight);
        //if (GameObject.Find(team) != null)
        // {
        ///  NetworkServer.SpawnWithClientAuthority(newLight, myConn);
        parentScript.makeMenu(2);

        //parentScript.makeMenu(newLight);
        //}
        //newHeavy.GetComponent<FarmScript>().commandLoc = spawnPosition;
        //moveObject = ture;
        thisMenu.SetActive(false);
        LineDrawer.enabled = false;
    }
    //0
    //0 -heavy
     // 1-  siege
      // 2- light
       //3- farm
    [Command]
    void CmdspawnObj(GameObject unit)
    {
        NetworkServer.SpawnWithClientAuthority(unit, GetComponent<NetworkIdentity>().connectionToClient);
    }
}
