using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportController : MonoBehaviour {

    private Transform teleportDest;

    public bool buttonPressed;

    public bool hasSwapped; //must be accessed by Reset Cube Script

    // Use this for initialization
    void Start () {
		teleportDest = GameObject.FindGameObjectWithTag("TeleportDest").GetComponent<Transform>();
        hasSwapped = false;
    }
	
	// Update is called once per frame
	void Update () {
        if (buttonPressed && !hasSwapped)
        {
            reverseRoute();
            hasSwapped = true;
        }
	}

    void pressedButton()
    {
        buttonPressed = true;
    }

    void OnTriggerEnter(Collider Other)
    {
        if (Other.gameObject.CompareTag("PlayerTrigger"))
        {
            Other.transform.parent.GetComponentInParent<Transform>().position = new Vector3(teleportDest.position.x, teleportDest.position.y, teleportDest.position.z);
        }
    }

    //The teleports are one-way.  This function switches the I/O.
    public void reverseRoute ()
    {
        Vector3 temp = transform.position;
        transform.position = teleportDest.transform.position;
        teleportDest.transform.position = temp;
    }
}
