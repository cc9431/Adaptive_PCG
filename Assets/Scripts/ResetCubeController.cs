using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetCubeController : MonoBehaviour {
	/*private GameObject player;
    private PlayerController playerScript;
	private Rigidbody playerRB;
	private Vector3 startpos;

	//array for double jumping

	private GameObject[] bouncePills;

    private GameObject[] breakables;

    private GameObject[] teleportPairs;

	private GameObject[] portal;

	private GameObject[] pickUps;

	void Start() {
		player = GameObject.FindGameObjectWithTag("Player");
		startpos = player.transform.position;
		playerRB = player.GetComponent<Rigidbody> ();
        playerScript = player.GetComponent<PlayerController>();
		//reference to aray for double jumping

		bouncePills = GameObject.FindGameObjectsWithTag("Bounce");
        breakables = GameObject.FindGameObjectsWithTag("Breakable");
        teleportPairs = GameObject.FindGameObjectsWithTag("TeleportEnter");
		portal = GameObject.FindGameObjectsWithTag("Portal");
		pickUps = GameObject.FindGameObjectsWithTag("PickUp");
    }

	void OnTriggerEnter(Collider Other) {}

	void OnTriggerStay(Collider Other){}

	void OnTriggerExit(Collider Other){
		if (Other.gameObject.CompareTag ("PlayerTrigger")) { // Send player back to the starting position if you fall out of the gameplay area
			playerRB.velocity = Vector3.zero;
			playerRB.angularVelocity = Vector3.zero;
			player.transform.position = startpos;

			//for loop is for resetting powerups.  While two for loops in a row is gross (this plus the switch manager), we're not going to be calling this, a lot.

			for (int x = 0; x < bouncePills.Length; x++) {
				if (!bouncePills [x].activeSelf) {
					bouncePills [x].SetActive (true);
				}
			}

			for (int x = 0; x < breakables.Length; x++) {
				if (!breakables [x].activeSelf) {
					breakables [x].SetActive (true);
				}
			}

			for (int x = 0; x < teleportPairs.Length; x++) {
				//reset teleport buttons to unpushed and place teleports at original positions... we might want to do this for all buttons, eventually
				TeleportController temp = teleportPairs [x].GetComponent<TeleportController> ();
				if (temp.hasSwapped) {
					temp.reverseRoute ();
					temp.buttonPressed = false;
					temp.hasSwapped = false;
				}
			}

			foreach(GameObject pickUp in pickUps){
				pickUp.SendMessage ("Reset");
			}

			playerScript.firstGatePassed = false;
			if (playerScript.speed < 0) {
				playerScript.speed = -playerScript.speed;
			}
			if (portal.Length > 0) {
				SwitchManager.Reset ();

			}
		}
	}*/
}
