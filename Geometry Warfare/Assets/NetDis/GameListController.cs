using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameListController : MonoBehaviour {
    
    public Transform contentObject;
    public GameObject button;
    public List<GameObject> listOfButtons;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void RemoveGames()
    {
        Debug.Log(listOfButtons.Count);
        int count = listOfButtons.Count;
        for (int i = 0; i < count; i++)
        {
            Destroy(listOfButtons[0]);
            listOfButtons.Remove(listOfButtons[0]);
        }

    }

    public void AddGames(List<ConnectionInfo> addressKeyList)
    {
        Debug.Log(listOfButtons.Count);
        Debug.Log(addressKeyList.Count);
        RemoveGames();

        for (int i = 0; i < addressKeyList.Count; i++)
        {
            Debug.Log("ok");
            GameObject newButton = Instantiate(button);
            newButton.GetComponent<gameButton>().address = addressKeyList[i].fromAddress;
            newButton.GetComponent<gameButton>().data = addressKeyList[i].name;
            newButton.transform.GetChild(0).GetComponent<Text>().text = addressKeyList[i].name;
            newButton.transform.SetParent(contentObject);
            listOfButtons.Add(newButton);
        }
       // button
        //contentObject
    }
}
