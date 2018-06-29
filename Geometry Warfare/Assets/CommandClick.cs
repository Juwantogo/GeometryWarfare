using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class CommandClick : NetworkBehaviour {
    [SyncVar]
    public GameObject myCommand;
    public GameObject menuPrefab;
    public Vector3 spawnPosition, pointOnCircumference;
    public float distance;
    public GameObject thisMenu, red, blue;
    //public MyNetManager manager;
    public MyNetworkManager nm;
    public LineRenderer LineDrawer;
    float radius = 5f;
    public bool CCspawnedR = false;
    public bool CCspawned = false;
    [SyncVar]
    public int gold;
    [SyncVar]
    public int food;
    [SyncVar]
    public string team, enemy, teamName;
    [SyncVar]
    public Color myCol;
    [SyncVar]
    public float health = 1000f;
    [SyncVar]
    public float setHealth = 1000f;
    [SyncVar]
    public string myTag;
    public Image bar;


    //set

    // Use this for initialization
    void Start () {
        //if (isLocalPlayer)
        //{
        tag = myTag;
        nm = GameObject.Find("Network Manager").GetComponent<MyNetworkManager>();
        //manager = GameObject.Find("Network Manager").GetComponent<MyNetManager>();
        myCommand = this.gameObject;
        this.transform.GetComponent<Renderer>().material.color = myCol;
        spawnPosition = this.transform.position;
            distance = radius;
            LineDrawer = this.GetComponent<LineRenderer>();
            thisMenu = GameObject.Instantiate(menuPrefab, transform.position, Camera.main.transform.rotation);
            thisMenu.GetComponent<Canvas>().worldCamera = Camera.main;
            thisMenu.GetComponent<UnitSpawner>().thisMenu = thisMenu;
            thisMenu.GetComponent<UnitSpawner>().LineDrawer = LineDrawer;
            thisMenu.GetComponent<UnitSpawner>().spawnPosition = spawnPosition;
            thisMenu.GetComponent<UnitSpawner>().distance = distance;
        thisMenu.GetComponent<UnitSpawner>().parentScript = this;
        thisMenu.GetComponent<UnitSpawner>().myCol = myCol;
        if (isServer)
        {
            //thisMenu.GetComponent<UnitSpawner>().myConn = GetComponent<NetworkIdentity>().connectionToClient;
            TargetsendConn(GetComponent<NetworkIdentity>().connectionToClient);
            TargetsendFood(connectionToClient, food);
            TargetsendPoints(connectionToClient, gold);

        }
        if (isLocalPlayer)
        {
            //thisMenu.GetComponent<UnitSpawner>().myConn = GetComponent<NetworkIdentity>().connectionToClient;
        }
        thisMenu.GetComponent<UnitSpawner>().pointOnCircumference = pointOnCircumference;
            thisMenu.SetActive(false);
            LineDrawer.enabled = false;
        //}
    }

    [TargetRpc]
    void TargetsendConn(NetworkConnection conn)
    {
        Debug.Log("conn" + conn);
        StartCoroutine(checkMenuMade(conn));
    }

    private IEnumerator checkMenuMade(NetworkConnection conn)
    {
        while (thisMenu == null)
        { yield return 1f; }
        thisMenu.GetComponent<UnitSpawner>().myConn = conn;
    }

    public void sendEndGameMsg(string msg)
    {
        TargetsendMsg(connectionToClient, msg);
    }

    // Update is called once per frame
    void Update () {
        if (health <= 0)
        {
            if (isServer)
            {
                GameObject eCC = GameObject.FindWithTag(enemy + " Command");
                eCC.GetComponent<CommandClick>().sendEndGameMsg("You Won");
                TargetsendMsg(connectionToClient, "You Lost...");
            }
            if (isLocalPlayer)
            {
                nm.endGame();
            }
            this.gameObject.GetComponent<Renderer>().enabled = false;
            StopAllCoroutines();
            //NetworkServer.Destroy(this.gameObject);
        }
        Debug.Log("Am i a team" + team);
/*  
        if(CCspawned == false)
        {
            if (isServer)
            {
                Debug.Log("ccspawned is false");
                GetComponent<NetworkIdentity>().AssignClientAuthority(GameObject.Find(team).GetComponent<NetworkIdentity>().connectionToClient);
                CCspawned = true;
            }    
        }*/
        //GameObject.Instantiate(menuPrefab, this.transform.position, Quaternion.identity);
        
    }
    //dont need
    void spawn()
    {
        if (this.gameObject.name == "BlueCC")
        {
            NetworkServer.SpawnWithClientAuthority(this.gameObject, GameObject.Find("Blue Team").GetComponent<NetworkIdentity>().connectionToClient);
        }
        else if (this.gameObject.name == "RedCC")
        {
            NetworkServer.SpawnWithClientAuthority(this.gameObject, GameObject.Find("Red Team").GetComponent<NetworkIdentity>().connectionToClient);
        }
    }

    void OnMouseDown()
    {
        Debug.Log("mosue");
        if (isLocalPlayer)
        {
            Debug.Log("mosue lp");
            RaycastHit theHit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out theHit, Mathf.Infinity))
            {
            Debug.Log("hit smtig");

            if (theHit.transform.gameObject == this.gameObject)
                {
                Debug.Log("hit go");

                // Vector3 newPos = new Vector3(transform.position.x, transform.position.y + 150f, transform.position.z);
                // thisMenu.transform.position = newPos;
                thisMenu.SetActive(!thisMenu.activeInHierarchy);
                    thisMenu.GetComponent<UnitSpawner>().team = team;
                    thisMenu.GetComponent<UnitSpawner>().enemy = enemy;
                    LineDrawer.enabled = !LineDrawer.enabled;
                    drawRadius();
                }

            }
            //Debug.Log(manager.getTeam());
        }



    }

    void takeDamage(float dam)
    {

        health -= dam;
        bar.fillAmount = health / setHealth;
        Debug.Log("Traget not null;" + bar.fillAmount);

    }

    public void sendDam(float dam)
    {
        RpctakeDamage(dam);
    }

    [ClientRpc]
    void RpctakeDamage(float dmg)
    {
        takeDamage(dmg);
    }

    void drawRadius()
    {

        float ThetaScale = 0.01f;
    int Size;
    float Theta = 0f;

        Theta = 0f;
        Size = (int)((1f / ThetaScale) + 1f);
        LineDrawer.SetVertexCount(Size);
        for (int i = 0; i < Size; i++)
        {
            Theta += (2.0f * Mathf.PI * ThetaScale);
            float x = radius * Mathf.Cos(Theta);
            float y = radius * Mathf.Sin(Theta);
            LineDrawer.SetPosition(i, new Vector3(x, 0, y));
            //LineDrawer.GetPosition(i);
            //Debug.Log("DistanceX - " + x + " - Y - " + y);

            pointOnCircumference = transform.position + new Vector3(x, 0, y);
            Debug.Log("DistanceaPoint - " + pointOnCircumference + " - Y - ");
            Debug.Log("Distance Frome 1 2 - " + Vector3.Distance(transform.position, pointOnCircumference) + " - Y - ");
           // Vector3.Distance(transform.position, pointOnCircumference);
        }
    }

    public void makeMenu(int unit)
    {
        if (hasAuthority)
        {
            //thisMenu.SetActive(false);
            Debug.Log("sent cmd " + unit);
            CmdspawnObj(unit);
        }
    }

    //only use nm for spawnpositions as these are the same for everyone
    [Command]
    void CmdspawnObj(int unit)
    {

        Debug.Log(unit);
        GameObject prefab = nm.spawnPrefabs[unit];
        GameObject gameO = Instantiate(prefab, GetComponent<networkPlayer>().unitPosition, Quaternion.identity);
        if(unit == 3)
        {
            if (food < 5 || gold < 800)
            {
                TargetsendMsg(connectionToClient,"Not Enough Resources");
                Destroy(gameO);
                return;
            }
            food = food - 5;
            gold = gold - 800;
            TargetsendFood(connectionToClient, food);
            TargetsendPoints(connectionToClient, gold);
            //thisMenu.SetActive(false);
            gameO.GetComponent<FarmScript>().commandLoc = spawnPosition;
            gameO.GetComponent<FarmScript>().targetPoint = pointOnCircumference;
            gameO.GetComponent<FarmScript>().drawnCircle = LineDrawer;
            gameO.GetComponent<FarmScript>().distance = distance;
            gameO.GetComponent<FarmScript>().farmCol = myCol;
            gameO.GetComponent<FarmScript>().health = 200f;
            gameO.GetComponent<FarmScript>().setHealth = 200f;
            gameO.GetComponent<FarmScript>().myCommandScript = this.gameObject;
            gameO.tag = team + " Farm";
            gameO.GetComponent<FarmScript>().myTag = gameO.tag;

        }
        else if(unit == 2)
        {
            if (food < 2)
            {
                TargetsendMsg(connectionToClient, "Not Enough Resources");
                Destroy(gameO);
                return;
            }
            food = food - 2;
            TargetsendFood(connectionToClient, food);
            //thisMenu.SetActive(false);
            gameO.GetComponent<UnitManager>().team = team;
            gameO.GetComponent<UnitManager>().enemy = enemy;
            gameO.GetComponent<UnitManager>().myCol = myCol;
            gameO.GetComponent<UnitManager>().fireRate = 1;
            gameO.GetComponent<UnitManager>().pointRate = 4;
            gameO.GetComponent<UnitManager>().canShoot = true;
            gameO.GetComponent<UnitManager>().punishment = 10;
            gameO.GetComponent<UnitManager>().range = 25f;
            gameO.GetComponent<UnitManager>().myCommandScript = this.gameObject;
            gameO.tag = team;
            gameO.GetComponent<UnitManager>().myTag = gameO.tag;

        }
        else if (unit == 1)//siege
        {
            if (food < 4)
            {
                TargetsendMsg(connectionToClient, "Not Enough Resources");
                Destroy(gameO);
                return;
            }
            food = food - 4;
            TargetsendFood(connectionToClient, food);
            //thisMenu.SetActive(false);
            gameO.GetComponent<UnitManager>().team = team;
            gameO.GetComponent<UnitManager>().enemy = enemy;
            gameO.GetComponent<UnitManager>().myCol = myCol;
            gameO.GetComponent<UnitManager>().fireRate = 1;
            gameO.GetComponent<UnitManager>().pointRate = 2;
            gameO.GetComponent<UnitManager>().canShoot = true;
            gameO.GetComponent<UnitManager>().punishment = 20;
            gameO.GetComponent<UnitManager>().range = 25f;
            gameO.GetComponent<UnitManager>().myCommandScript = this.gameObject;

            gameO.tag = team;
            gameO.GetComponent<UnitManager>().myTag = gameO.tag;

        }
        else if (unit == 0)//heavy
        {
            if(food < 5)
            {
                TargetsendMsg(connectionToClient, "Not Enough Resources");
                Destroy(gameO);
                return;
            }
            food = food - 5;

            TargetsendFood(connectionToClient, food);
            //thisMenu.SetActive(false);
            gameO.GetComponent<UnitManager>().team = team;
            gameO.GetComponent<UnitManager>().enemy = enemy;
            gameO.GetComponent<UnitManager>().myCol = myCol;
            gameO.GetComponent<UnitManager>().fireRate = 1;
            gameO.GetComponent<UnitManager>().pointRate = 1;
            gameO.GetComponent<UnitManager>().canShoot = true;
            gameO.GetComponent<UnitManager>().range = 25f;
            gameO.GetComponent<UnitManager>().punishment = 30;
            gameO.GetComponent<UnitManager>().myCommandScript = this.gameObject;

            gameO.tag = team;
            gameO.GetComponent<UnitManager>().myTag = gameO.tag;

        }

        // foreach (var i in gameO.GetComponentsInChildren<Renderer>())
        //  {
        //      i.material.color = myCol;
        //  }
        Debug.Log("sent cmd " + unit);
        NetworkServer.SpawnWithClientAuthority(gameO, GetComponent<NetworkIdentity>().connectionToClient);
    }

    public override void OnStartClient()
    {
        if (isLocalPlayer)
        {
           // CmdgetInfo();
        }
    }

    public void acceptPoints(int pts)
    {
        gold += pts;
        TargetsendPoints(connectionToClient, gold);
    }

    public void acceptFood(int pts)
    {
        food += pts;
        TargetsendFood(connectionToClient, food);
    }

    [TargetRpc]
    void TargetsendMsg(NetworkConnection conn, string msg)
    {
        Debug.Log("MSG TRPC");
        nm.transform.GetChild(3).GetChild(0).GetComponent<Text>().text = msg;
        if(msg == "You Won!" || msg == "You Lost...")
        {
            StartCoroutine(endGameTimer());
        }
        else
        {
            StartCoroutine(msgTimer());
        }
    }

    [TargetRpc]
    void TargetsendPoints(NetworkConnection conn, int pts)
    {
        if(nm == null)
        {
            nm = GameObject.Find("Network Manager").GetComponent<MyNetworkManager>();
        }
        Debug.Log("POINTS TRPC");
        nm.transform.GetChild(3).GetChild(1).GetComponent<Text>().text = "Gold: " + pts;
    }

    [TargetRpc]
    void TargetsendFood(NetworkConnection conn, int fd)
    {
        if(nm == null)
        {
            nm = GameObject.Find("Network Manager").GetComponent<MyNetworkManager>();
        }
        Debug.Log("FOOD TRPC");
        nm.transform.GetChild(3).GetChild(2).GetComponent<Text>().text = "Food: " + fd;
    }

    [Command]
    void CmdgetInfo()
    {
        RpcsendInfo();
    }

    [ClientRpc]
    void RpcsendInfo()
    {
        this.transform.GetComponent<Renderer>().material.color = myCol;
    }

    public IEnumerator msgTimer()
    {
        yield return new WaitForSeconds(2);
        nm.transform.GetChild(3).GetChild(0).GetComponent<Text>().text = "";
    }

    public IEnumerator endGameTimer()
    {
        yield return new WaitForSeconds(7);
        nm.endGame();
    }
    /*
    [Command]
    void CmdspawnObj(GameObject unit)
    {
        NetworkServer.SpawnWithClientAuthority(unit, GetComponent<NetworkIdentity>().connectionToClient);
    }*/
}
