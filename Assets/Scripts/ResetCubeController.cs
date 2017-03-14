using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetCubeController : MonoBehaviour {
	private GameObject player;

	private Rigidbody playerRB;
	private Vector3 startpos;

	void Start() {
		player = GameObject.FindGameObjectWithTag("Player");
		startpos = player.transform.position;
		playerRB = player.GetComponent<Rigidbody> ();
	}

	void OnTriggerEnter(Collider Other) {}

	void OnTriggerStay(Collider Other){}

	void OnTriggerExit(Collider Other){
		if (Other.gameObject.CompareTag("PlayerTrigger")) { // Send player back to the starting position if you fall out of the gameplay area
			playerRB.velocity = Vector3.zero;
			playerRB.angularVelocity = Vector3.zero;
			player.transform.position = startpos;
		}
	}
}
