using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FanController : MonoBehaviour {
    private GameObject player;
	private Transform blade;
	private Rigidbody playerRB;
	private float fanForce = 40;

	public bool buttonPressed;

	void pressedButton(){
		buttonPressed = true;
	}

    void Start () {
		player = GameObject.FindGameObjectWithTag("Player");
        playerRB = player.GetComponent<Rigidbody>();
		blade = transform.GetChild (0).transform; //The blade should always be the only child of the Fan object
    }

	void FixedUpdate(){
		if (buttonPressed) { //Spin the blade when the button is pressed
			blade.Rotate(new Vector3 (0, 0, -90) * Time.deltaTime * 2);
		}
	}

	void OnTriggerEnter(Collider Other) {
		if (Other.gameObject.CompareTag ("PlayerTrigger") && buttonPressed) {
			playerRB.AddForce (transform.up * fanForce); //Blow player away like a big fan would
	
		}
	}

	void OnTriggerStay(Collider Other) {
		if (Other.gameObject.CompareTag ("PlayerTrigger") && buttonPressed) {
			playerRB.AddForce (transform.up * fanForce); //Stop when the player is out of the fan

		}
    }

	void OnTriggerExit(Collider Other) {}
}
