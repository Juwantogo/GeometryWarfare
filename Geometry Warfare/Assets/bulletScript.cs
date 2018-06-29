using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bulletScript : MonoBehaviour {

    public MyNetManager manager;
    // Use this for initialization
    void Start()
    {
        manager = GameObject.Find("Network Manager").GetComponent<MyNetManager>();
        StartCoroutine(LifeSpan());
    }
    IEnumerator LifeSpan()
    {
        yield return new WaitForSeconds(2.0f);
        Destroy(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void OnTriggerEnter(Collider other)
    {
        if (other.tag == manager.getEnemy())
        {
            StopAllCoroutines();
            Destroy(this.gameObject);
        }
    }
}
