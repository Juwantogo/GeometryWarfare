using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//used for piucking spawn point no longer needed
public class ClickEvent : MonoBehaviour {
    int ccCreated = 0;
    public GameObject commandObj;
    //public GameObject spawnSpot;
    // Use this for initialization
    void Start () {
        ccCreated = 0;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
   /* 
    void OnMouseDown()
    {
        
        RaycastHit theHit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out theHit))
        {
            Debug.Log(theHit.transform.name);
            //Debug.Log(theHit.transform.gameObject.tag);
            //if(theHit.transform.gameObject.tag == "Command")
            //{
            //    Destroy(theHit.transform.gameObject);
            //    ccCreated = 0;
            //}
            if (theHit.transform.name == "SpawnTeam2" && ccCreated == 0)// (theHit.transform.gameObject.tag == "Terrain" && ccCreated == 0)
            {
                Vector3 newPos = theHit.point;
                newPos.y = 55f;
                GameObject.Instantiate(commandObj, newPos, Quaternion.identity);
                //GameObject.Instantiate(spawnSpot, newPos, Quaternion.identity);
                ccCreated = 1;
            }
        }
       // Debug.Log(theHit.transform.gameObject.tag + "outside");

    }
    */
}
