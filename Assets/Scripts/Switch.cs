using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Switch : MonoBehaviour {

    //DIFFERENCE BETWEEN THIS SCRIPT AND THE SWITCHMANAGER:  This script tells "this" to deactivate, so ther switch needs to be reactivated from elsewhere,
    //i.e, the SwitchManager component in the GM GameObject...

    // Use this for initialization
    void Start () {

    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter(Collider Other)
    {
        if (Other.gameObject.CompareTag("PlayerTrigger"))
        {
            gameObject.SetActive(false);
            SwitchManager.switchCount++;
        }
    }
}
