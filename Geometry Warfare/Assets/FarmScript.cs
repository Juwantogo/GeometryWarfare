using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class FarmScript : UnitSpawner {

    [SyncVar]
    public GameObject thisOBJ;
    public bool movable;
    public float speed = 100000f;
    [SyncVar]
    public float distance;
    public float currentRadius, Radius;
    //targetPosition = Changing Value for mouse POS, commandLoc = set Value for command POS, targetPoint = set Valuie for radius Calc
    [SyncVar]
    public Vector3 commandLoc;
    [SyncVar]
    public float health = 1000f;
    [SyncVar]
    public float setHealth = 1000f;
    public Vector3 targetPosition, targetPoint;
    public Renderer rend;
    public Collider collComp;
    [SyncVar]
    public Color farmCol;
    [SyncVar]
    public GameObject myCommandScript;
    [SyncVar]
    public string myTag;
    public LineRenderer drawnCircle;
    public Image bar;

    // Use this for initialization
    void Start () {
        if (isServer)
        {
            //thisOBJ = this.gameObject;
            StartCoroutine(slowUpdate());
        }
        tag = myTag;
        GetComponent<Renderer>().material.color = farmCol;

    }

    public override void OnStartAuthority()
    {
        if (hasAuthority)
        {

            Debug.Log("FARMMMMMM");
            //StartCoroutine(slowUpdate());
            movable = true;
            targetPosition = Vector3.zero;
            rend = GetComponent<Renderer>();

            rend.material.shader = Shader.Find("Outlined/Custom");
            //Camera.main.GetComponent<MainCamScript>().target = this.gameObject.transform;
            // Camera.main.GetComponent<MainCamScript>().change = true;
            collComp = GetComponent<Collider>();
            Radius = distance * 15f;
            currentRadius = Vector3.Distance(targetPosition, targetPoint);
            Debug.Log(Radius);
            Debug.Log(currentRadius);
        }
    }

    void takeDamage(float dam)
    {

        health -= dam;
        bar.fillAmount = health / 100f;
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

    // Update is called once per frame
    void Update () {
        if (health <= 0)
        {
            StopAllCoroutines();
            NetworkServer.Destroy(this.gameObject);
        }

        if (hasAuthority)
        {
            Debug.Log("has auth: " + movable);
            if (movable)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    movable = false;
                    setPosition();
                }
                else
                {
                    setPosition();
                }
            }

        }


    }

    IEnumerator slowUpdate()
    {
        while (true)
        {
            myCommandScript.GetComponent<CommandClick>().acceptFood(1);
            yield return new WaitForSeconds(20);
        }
        //GameObject.Find(team).GetComponent<networkPlayer>().gold += 1;
    }

    //Sets The Position return movable false if valid click spot
    //returns movable true if click spot not valid
    void setPosition()
    {
        RaycastHit theHit;
        if (movable)
        {
            
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out theHit))
            {
                if (theHit.transform.gameObject.tag == "Terrain" && movable)
                {

                    Vector3 newPos = theHit.point;
                    newPos.y = 55f;
                    targetPosition = newPos;
                    currentRadius = Vector3.Distance(commandLoc, targetPosition);
                    Debug.Log(currentRadius + " << " + Radius);
                    if (currentRadius < Radius)
                    {
                        rend.enabled = true;
                        collComp.enabled = true;
                    }
                    else
                    {
                        rend.enabled = false;
                        collComp.enabled = false;


                    }
                    transform.position = Vector3.MoveTowards(transform.position, newPos, 500 * Time.deltaTime);
                }
            }

        }
        else
        {
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out theHit))
            {
                Vector3 newPos = theHit.point;
                newPos.y = 55f;
                targetPosition = newPos;
                currentRadius = Vector3.Distance(commandLoc, targetPosition);
                if (theHit.transform.gameObject.tag == "Terrain")
                {
                   // Debug.Log("Click - Terrain");
                    
                    if (currentRadius < Radius)
                    {
                        rend.enabled = true;
                        collComp.enabled = true;
                        drawnCircle.enabled = false;
                        rend.material.shader = Shader.Find("Diffuse");
                        transform.position = targetPosition;
                        movable = false;
                       // Camera.main.GetComponent<MainCamScript>().go.transform.position = this.gameObject.transform.position;
                        //Camera.main.GetComponent<MainCamScript>().target = Camera.main.GetComponent<MainCamScript>().go.transform;
                       // Camera.main.GetComponent<MainCamScript>().change = true;
                        Debug.Log("Spawn at: " + transform.position);
                    }
                    else if (currentRadius > Radius)
                    {
                        rend.enabled = false;
                        collComp.enabled = false;
                        movable = true;
                        Debug.Log("Can't Spawn There");
                    }
                }
                else if(theHit.transform.gameObject == this.gameObject)
                {

                    rend.material.shader = Shader.Find("Diffuse");
                    myCommandScript.GetComponent<CommandClick>().LineDrawer.enabled = false;
                    movable = false;
                   // Camera.main.GetComponent<MainCamScript>().go.transform.position = this.gameObject.transform.position;
                   // Camera.main.GetComponent<MainCamScript>().target = Camera.main.GetComponent<MainCamScript>().go.transform;
                    //Camera.main.GetComponent<MainCamScript>().change = true;
                    Debug.Log("Spawn at: " + transform.position);
                }
                else
                {
                    Debug.Log("Click - No Terrain");
                    movable = true;
                }
            }
            else
            {
                Debug.Log("Click - No Hit");
                movable = true;
            }
        }

    }
}
