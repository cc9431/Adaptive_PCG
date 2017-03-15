using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedPadController : MonoBehaviour {
	private GameObject player;
	private Rigidbody playerRB;

	public bool buttonPressed;
	public float speedBoost;

	void Start(){
		player = GameObject.FindGameObjectWithTag("Player"); //Find player object
		playerRB = player.GetComponent<Rigidbody>();
	}

	void pressedButton(){
		buttonPressed = true;
	}

	void counterClockwise(){
		transform.Rotate(new Vector3 (0, 10,  0) * Time.deltaTime);
	}

	void clockwise(){
		transform.Rotate(new Vector3 (0, -10,  0) * Time.deltaTime);
	}

	void OnTriggerEnter(Collider Other) {
		if(Other.gameObject.CompareTag("PlayerTrigger") && buttonPressed){
			playerRB.AddForce(transform.forward * speedBoost, ForceMode.Impulse); //When the player enters the collider, add force to it regardless of its current velocity
		}
	}

	void OnTriggerStay(Collider Other) {}
	void OnTriggerExit(Collider Other) {}
}
