using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* 
 * Decision tree implementation?
 *	Build decision tree based off of test data that takes
 *	in player's stats -- refering to the numbers generated
 *	by the player's interaction with the objects. Allow
 *	game to classify player after set intervals. Use this
 *	classification to determine generation
 */ 

public class MasterController : MonoBehaviour {
	//private int spikesTouched;
	//private int timesFallen;
	//private int orbs;
	//private int totalFrames;
	private int framesInAir;
	private int framesAtMax;
	private int framesBoosting;

	private int rampJump;
	private int rampL0;
	private int rampL1;
	private int rampL2;

	private int speedStrip;
	private int speedL0;
	private int speedL1;
	private int speedL2;

	private int spikeStrip;
	private int spikeL0;
	private int spikeL1;
	private int spikeL2;

	private int jumps;

	private GameObject player;
	private _CarController carController;
	private GenerateInfinite PCG;

	void Start() {
		//totalFrames = 0;
		//spikesTouched = 0;
		//timesFallen = 0;
		//orbs = 0;

		framesAtMax = 0;
		framesInAir = 0;
		framesBoosting = 0;

		rampJump = 0;
		rampL0 = 0;
		rampL1 = 0;
		rampL2 = 0;

		speedStrip = 0;
		speedL0 = 0;
		speedL1 = 0;
		speedL2 = 0;

		spikeStrip = 0;
		spikeL0 = 0;
		spikeL1 = 0;
		spikeL2 = 0;

		jumps = 0;

		player = GameObject.FindGameObjectWithTag("Player");
		carController = player.GetComponent<_CarController> ();
	}

	void Update () {
		bool Print = (Input.GetAxis ("Submit") != 0);

		if (carController.inAir) framesInAir++;
		if (carController.maxSpeed) framesAtMax++;
		if (carController.boosting) framesBoosting++;

		if (Print) {
			print ("framesAtMax: " + framesAtMax.ToString());
			print ("framesBoosting: " + framesBoosting.ToString());
			print ("framesInAir: " + framesInAir.ToString());
			print ("Ramps: " + rampJump.ToString());
		}
	}

	public void PlayerInteracts(string objectID){
		if (objectID.StartsWith ("R")) {
												rampJump++;
			if (objectID.EndsWith ("0")) 		rampL0++;
			else if (objectID.EndsWith ("1")) 	rampL1++;
			else if (objectID.EndsWith("2"))	rampL2++;

		} else if (objectID.StartsWith("K")) {
												spikeStrip++;
			if (objectID.EndsWith ("0"))		spikeL0++;
			else if (objectID.EndsWith("1"))	spikeL1++;
			else if (objectID.EndsWith("2"))	spikeL2++;

		} else if (objectID.StartsWith("S")) {
												speedStrip++;
			if (objectID.EndsWith("0"))			speedL0++;
			else if (objectID.EndsWith("1"))	speedL1++;
			else if (objectID.EndsWith("2")) 	speedL2++;
		}
	}

	public void PlayerJumps() {
		jumps++;
	}
}
