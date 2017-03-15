using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityOrbController : MonoBehaviour {
	public float pullRadius;
	public float pullForce;
	public float speed;
	private GameObject player;

	private Rigidbody playerRB;
	private Vector3 startpos;

	void Start() {
		player = GameObject.FindGameObjectWithTag("Player");
		startpos = player.transform.position;
		playerRB = player.GetComponent<Rigidbody> ();
	}

	void FixedUpdate() {
		Rotate ();

		foreach (Collider collider in Physics.OverlapSphere(transform.position, pullRadius)) {
			if (collider.gameObject.CompareTag("PlayerTrigger")) {
				
				// Calculate vector towards gravOrb
				Vector3 forceDirection = transform.position - player.transform.position;

				// apply force on player towards gravOrb
				playerRB.AddForce (forceDirection.normalized * pullForce * Time.fixedDeltaTime);
			}
		}
	}

	void Rotate(){ //This function makes the grav cubes spin in a cool looking way
		transform.Rotate (new Vector3 (30, 10, 50) * Time.deltaTime * speed);
		foreach (Transform child in transform) {
				child.Rotate (new Vector3 (30, 10, 50) * Time.deltaTime * speed);
		}
	}

	void OnTriggerEnter(Collider Other) {
		if (Other.gameObject.CompareTag("PlayerTrigger")) { //Kill the player if they touch it because otherwise you'd be stuck
			playerRB.velocity = Vector3.zero;
			playerRB.angularVelocity = Vector3.zero;
			player.transform.position = startpos;
		}
	}
	void OnTriggerStay(Collider Other){}

	void OnTriggerExit(Collider Other){}
}
