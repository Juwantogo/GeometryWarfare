using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.Networking;

public class UnitManager : UnitSpawner {
    public GameObject Target;
    [SyncVar]
    public GameObject buildingTarget;
    public GameObject bulletPrefab;
    [SyncVar]
    public float health = 100;
    [SyncVar]
    public GameObject thisOBJ;
    [SyncVar]
    public int fireRate = 2;
    [SyncVar]
    public int pointRate = 10;
    public int speed = 5;
    [SyncVar]
    public float punishment = 10f;
    [SyncVar]
    public GameObject myCommandScript;
    [SyncVar]
    public string myTag;
    [SyncVar]
    public string enemy;
    [SyncVar]
    public float range = 25f;
    public Renderer rend;
    public NavMeshAgent myAgent;
    public bool movable = true;
    public bool capturing = false;
    [SyncVar]
    public bool canShoot = true;
    public bool getPoints = true;
    public string enemyName;
    public Image bar;
    //[SyncVar]
   // public Color myCol;
    // Use this for initialization
    void Start () {
        //this.transform.GetComponent<Renderer>().material.color = myCol;
        foreach (var i in this.transform.GetComponentsInChildren<Renderer>())
        {
            i.material.color = myCol;
        }
        if (this.gameObject.name == "Siege(Clone)")
        {
            punishment = 20f;
        }
        else if (this.gameObject.name == "Light(Clone)")
        {
            punishment = 5f;
        }
        else if (this.gameObject.name == "Heavy(Clone)")
        {
            punishment = 30f;
        }
        myAgent = this.GetComponent<NavMeshAgent>();
        rend = GetComponent<Renderer>();
        if (isServer)
        {
            //GameObject Temp = Instantiate(prefab);
            //NetworkServer.Spawn(Temp);
            //myPiece = this.gameObject;
            //myPiece.GetComponent<Renderer>().enabled = false;
            //manager = GameObject.Find("Network Manager").GetComponent<myNetworkManager>();
            //enemy = GameObject.Find("enemy1");
            thisOBJ = this.gameObject;
            //manager = GameObject.Find("Network Manager").GetComponent<MyNetManager>();
            //playerName = manager.getName();
            //teamColor = manager.getTeam();
            Debug.Log(team);
            ////Rpc_setTeam(teamColor);
            //thisCol = manager.col;
            //myPiece.GetComponent<Renderer>().material.color = thisCol;
            //RpcupdateColor();
            //spawnPoints = FindObjectsOfType<NetworkStartPosition>();
            //Debug.Log(isServer + " Color " + manager.getColor().name);
            //points = 0;
        }
        tag = myTag;

        StartCoroutine(slowUpdate());
        //myAgent.SetDestination(this.transform.position);
        //myAgent.destination = this.transform.position;
    }
	
	// Update is called once per frame
	void Update () {
        if(health <= 0)
        {
            StopAllCoroutines();
            NetworkServer.Destroy(this.gameObject);
        }
       // if (capturing)
       // {
            CheckCapture();

        //}
        if (hasAuthority)
        {
            if (Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(0))
            {
                RaycastHit myCheck;
                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out myCheck, Mathf.Infinity, ~LayerMask.GetMask("Capture Points")))
                {
                    Debug.Log("insiderayupdate");
                    //have to change player later
                    /*
                    if (myCheck.transform.gameObject.tag == "Player")
                    {
                        movable = false;
                        rend.material.shader = Shader.Find("Diffuse");
                    }
                    else */
                    if (myCheck.transform.gameObject == this.gameObject && Input.GetMouseButtonDown(0))
                    {
                        Clicked();
                        //Debug.Log("Clicked Self");
                    }
                    else if (movable && Input.GetMouseButtonDown(1) && myCheck.transform.gameObject.tag == enemy + " Command")
                    {
                        Debug.Log("ISSA HIT" + myCheck.point);
                        buildingTarget = myCheck.transform.gameObject;
                        CmdsendbuildTarget(buildingTarget);
                        myAgent.ResetPath();
                        myAgent.SetDestination(myCheck.point);
                        myAgent.destination = myCheck.point;
                        //                        Cmd_SendVector(myCheck.point);
                    }
                    else if (movable && Input.GetMouseButtonDown(1) && myCheck.transform.gameObject.tag == enemy + " Farm")
                    {
                        Debug.Log("ISSA HIT" + myCheck.point);
                        buildingTarget = myCheck.transform.gameObject;
                        CmdsendbuildTarget(buildingTarget);
                        myAgent.ResetPath();
                        myAgent.SetDestination(myCheck.point);
                        myAgent.destination = myCheck.point;
                        //                        Cmd_SendVector(myCheck.point);
                    }
                    else if (movable && Input.GetMouseButtonDown(1))
                    {
                        Debug.Log("ISSA HIT" + myCheck.point);
                        myAgent.ResetPath();
                        myAgent.SetDestination(myCheck.point);
                        myAgent.destination = myCheck.point;
                        buildingTarget = null;
                        CmdsendbuildTarget(null);
                        //                        Cmd_SendVector(myCheck.point);
                    }

                }
                else
                {
                    //rend.material.shader = Shader.Find("Diffuse");
                }
                // movable = false;
                //setPosition();
            }
        }
           

    }

    [Command]
    void CmdsendbuildTarget(GameObject bTarget)
    {
        RpcsendbuildingTarget(bTarget);
    }

    [ClientRpc]
    void RpcsendbuildingTarget(GameObject b)
    {
        buildingTarget = b;
    }

    void Clicked()
    {
        if (hasAuthority)
        {
            //LayerMask on raycast to the layer
            //~LayerMask on ignore raycast to this 
            RaycastHit theHit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out theHit, Mathf.Infinity, ~LayerMask.GetMask("Capture Points")))
            {
                Debug.Log("insideray");
                if (theHit.transform.gameObject == this.gameObject && !movable)
                {
                    Renderer[] renderers = GetComponentsInChildren<Renderer>();
                    foreach (var r in renderers)
                    {
                        // Do something with the renderer here...
                        Debug.Log("render FOUND");
                        r.material.shader = Shader.Find("Outlined/Custom"); // like disable it for example. 
                    }
                    //rend.material.shader = Shader.Find("Outlined/Custom");
                    movable = true;
                    //Cmd_SendVector(theHit.transform.position);
                    //Vector3 newPos = new Vector3(transform.position.x, transform.position.y + 150f, transform.position.z);
                    //thisMenu.transform.position = newPos;
                    // thisMenu.SetActive(!thisMenu.activeInHierarchy);
                    //LineDrawer.enabled = !LineDrawer.enabled;
                    // drawRadius();
                }
                else if (theHit.transform.gameObject == this.gameObject && movable)
                {
                    Renderer[] renderers = GetComponentsInChildren<Renderer>();
                    foreach (var r in renderers)
                    {
                        // Do something with the renderer here...
                        //Debug.Log("render FOUND");
                        r.material.shader = Shader.Find("Diffuse"); // like disable it for example. 
                    }
                    //rend.material.shader = Shader.Find("Diffuse");
                    movable = false;
                    //Cmd_SendVector(theHit.transform.position);
                    //Vector3 newPos = new Vector3(transform.position.x, transform.position.y + 150f, transform.position.z);
                    //thisMenu.transform.position = newPos;
                    // thisMenu.SetActive(!thisMenu.activeInHierarchy);
                    //LineDrawer.enabled = !LineDrawer.enabled;
                    // drawRadius();
                }

            }

        }
            
        
        
    }

    void CheckCapture()
    {
        Vector3 origin = this.transform.position;
        origin.y = 100;
        RaycastHit myCheck;
        if (Physics.Raycast(origin, -this.transform.up, out myCheck, Mathf.Infinity, LayerMask.GetMask("Capture Points")))
        {
            if (isServer)
            {
                if (getPoints == true)
                {
                    //GameObject.Find(team).GetComponent<networkPlayer>().points += 1;
                    myCommandScript.GetComponent<CommandClick>().acceptPoints(pointRate);
                    getPoints = false;
                    StartCoroutine(atkTimer2());

                }
            }
            //Debug.Log("hit!");
            //have to change player later

                Debug.Log("points!");
        }
    }
/*
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Captures")
        {
            capturing = !capturing;
        }
    }
    */
    void Move()
    {
        //this.gameObject.transform.position 
        //this.gameObject.transform.rotation
        //Cmd_moveObject();
    }

    void Shoot()
    {

    }

    void takeDamage(float dam)
    {

        health -= dam;
        bar.fillAmount = health/100f;
        Debug.Log("IM HURT: " + dam + "enemy: " + enemy);
        
    }

    void setPosition()
    {
        //this.gameObject.transform.position;

    }

    public IEnumerator slowUpdate()
    {
        while (true)
        {
           // Debug.Log("DEST: " + myAgent.destination);
            if (canShoot)
            {
                GameObject[] targets = GameObject.FindGameObjectsWithTag(enemy);
                Debug.Log(targets.Length);
               // Debug.Log("The number of targets found are: " + targets.Length);
                RaycastHit myCheck;
                float minDistance = 10000f;
                
                if(buildingTarget != null)
                {
                    //Do Raycast for distance
                    //and set target as below
                    //call take damage funct
                    Debug.DrawRay(transform.localPosition + 1.03f * (buildingTarget.transform.localPosition - this.transform.localPosition).normalized, buildingTarget.transform.localPosition - this.transform.localPosition, Color.green, 10);
                    if (Physics.Raycast(transform.localPosition + 1.03f * (buildingTarget.transform.localPosition - this.transform.localPosition).normalized, buildingTarget.transform.position - this.transform.position, out myCheck))
                    {
                        Debug.Log("distance from target: " + myCheck.distance + "mindistance" +  minDistance);
                        //Debug.Log("Inside for loop " + myCheck.collider.gameObject.tag);
                        if (myCheck.collider.gameObject.tag == enemy + " Farm")
                        {
                            Debug.Log("Inside ray cast to target");
                            //I know I have line of sight.
                            if (myCheck.distance < minDistance && myCheck.distance < range)
                            {
                                Debug.Log("Inside set target");
                                //I know I have found a closer enemy and it is within range.
                                Target = myCheck.collider.gameObject;
                                //minDistance = myCheck.distance;
                            }
                        }
                        if (myCheck.collider.gameObject.tag == enemy + " Command")
                        {
                            Debug.Log("TARGET CMD");
                            //I know I have line of sight.
                            if (myCheck.distance < minDistance && myCheck.distance < range)
                            {
                                Debug.Log("TARGET CMD IN RANGE");
                                //I know I have found a closer enemy and it is within range.
                                Target = myCheck.collider.gameObject;
                                //minDistance = myCheck.distance;
                            }
                        }
                    }
                    else
                    {
                        Debug.Log("TARGET NO HIT");
                        Target = null;
                    }
                }
                if (Target == null)
                {
                    Debug.Log("TARGET NULL");
                    foreach (GameObject o in targets)
                    {
                        //if (isLocalPlayer)
                       // {
                        //}
                        //Check for line of sight
                        if (Physics.Raycast(this.transform.localPosition + 1.03f * (o.transform.localPosition - this.transform.localPosition).normalized, o.transform.localPosition - this.transform.localPosition, out myCheck))
                        {
                            Debug.Log("WTF did i hit " + myCheck.collider.transform.root.tag + enemy);
                            //Debug.Log("Inside for loop " + myCheck.collider.gameObject.tag);
                            if (myCheck.collider.transform.root.tag == enemy)
                            {
                                Debug.Log("Inside set targetMYCHECK DIST: " + myCheck.distance);
                                Debug.Log("Inside set targetMIN DIST: " + minDistance);
                                Debug.Log("Inside set targetRANGE DIST: " + range);
                                Debug.Log("Inside ray cast to target");
                                //I know I have line of sight.
                                if (myCheck.distance < minDistance && myCheck.distance < range)
                                {
                                    Debug.Log("ASSIGN TARGET");
                                    //I know I have found a closer enemy and it is within range.
                                    Target = myCheck.collider.transform.root.gameObject;
                                    minDistance = myCheck.distance;
                                }
                            }
                        }
                        else
                        {
                            Target = null;
                        }
                    }
                }
                if (Target != null)
                {
                    //Debug.Log("Traget not null;");
                    
                    this.transform.LookAt(Target.transform.localPosition);
                    Vector3 pos = this.transform.localPosition * 1.02f + (Target.transform.localPosition - this.transform.localPosition).normalized;
                    GameObject temp = GameObject.Instantiate(bulletPrefab, pos, Quaternion.identity);
                    temp.GetComponent<Rigidbody>().velocity = 20 * this.transform.forward;
                    //Target.GetComponent<UnitManager>().takeDamage(punishment);
                    if (isServer)
                    {
                        if(Target.tag == enemy + " Command")
                        {
                            Target.GetComponent<CommandClick>().sendDam(punishment);

                        }
                        else if (Target.tag == enemy + " Farm")
                        {
                            Target.GetComponent<FarmScript>().sendDam(punishment);
                        }
                        else
                        {
                            Target.GetComponent<UnitManager>().RpctakeDamage(punishment);
                        }
                    }
                    Target = null;
                    canShoot = false;
                    StartCoroutine(atkTimer());
                    //normally this would be a rigid body


                }
            }
            yield return new WaitForSeconds(.05f);
        }
    }

    IEnumerator atkTimer()
    {
        yield return new WaitForSeconds(fireRate);
        canShoot = true;
    }

    IEnumerator atkTimer2()
    {
        yield return new WaitForSeconds(10f);
        getPoints = true;
    }

    [ClientRpc]
    void RpctakeDamage(float dmg)
    {
        takeDamage(dmg);
    }

    [Command]
        public void Cmd_SendVector(Vector3 d)
    {
        //d.y = 50f;
        myAgent.ResetPath();
        myAgent.SetDestination(d);
        myAgent.destination = d;
        //if (gamePiece != null)
        //{
        //gamePiece.GetComponent<NetworkGamePiece>().myAgent.isStopped = true;
        //gamePiece.GetComponent<NetworkGamePiece>().myAgent.ResetPath();
        //gamePiece.GetComponent<NetworkGamePiece>().myAgent.SetDestination(d);
        //gamePiece.GetComponent<NetworkGamePiece>().myAgent.destination = d;
        //}
    }
    /*
                     Renderer[] renderers = GetComponentsInChildren<Renderer>();
                foreach (var r in renderers)
                {
                    // Do something with the renderer here...
                    r.material.shader = Shader.Find("Diffuse"); // like disable it for example. 
                }
     */
}
