using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MasterController : MonoBehaviour {
	//private int spikesTouched;
	//private int timesFallen;

	private int totalPoints;		// Total points based on tricks and orbs collected

	private int orbs;
	private int rampPoints;			// The points are based on how many orbs are collected from each interactable
	private int speedPoints;
	private int spikePoints;

	private int framesInAir;		// Used to evaluate the engagement of the player
	private int framesAtMax;
	private int framesBoosting;

	private int rampJumpTotal;		// How many times a player uses a ramp
	private int rampL0;
	private int rampL1;
	private int rampL2;

	private int speedPortalTotal;	// How many times a player uses a speed portal
	private int speedL0;
	private int speedL1;
	private int speedL2;

	private int spikeStripTotal;	// How many times a player jumps over a spikestrip
	private int spikeL0;
	private int spikeL1;
	private int spikeL2;

	private int totalFlips;			// How many tricks the player does over all
	private int totalTurns;
	private int totalSpins;

	private int jumps;				// How many times a player jumps

	private GameObject player;
	private _CarController carController;
	private GenerateInfinite PCG;

	private bool lastFramePrint;
	private bool lastFrameCancel;
	private bool TrickTracking;
	private bool objectTouched;
	private string lastObjectTouched;

	private float Xspin;
	private float Yspin;
	private float Zspin;

	void Start() {
		// Start all trackers at 0
		totalPoints = 0;

		orbs = 0;
		rampPoints = 0;
		speedPoints = 0;
		spikePoints = 0;

		framesAtMax = 0;
		framesInAir = 0;
		framesBoosting = 0;

		rampJumpTotal = 0;
		rampL0 = 0;
		rampL1 = 0;
		rampL2 = 0;

		speedPortalTotal = 0;
		speedL0 = 0;
		speedL1 = 0;
		speedL2 = 0;

		spikeStripTotal = 0;
		spikeL0 = 0;
		spikeL1 = 0;
		spikeL2 = 0;

		totalFlips = 0;
		totalTurns = 0;
		totalSpins = 0;

		jumps = 0;

		Xspin = 0;
		Yspin = 0;
		Zspin = 0;

		lastObjectTouched = "0";

		player = GameObject.FindGameObjectWithTag("Player");
		carController = player.GetComponent<_CarController> ();
		TrickTracking = false;
		objectTouched = false;
	}

	void Update () {
		if ((TrickTracking || carController.inAir) && (!carController.onBack)) TrackTricks ();

		if (carController.inAir) framesInAir++;
		if (carController.maxSpeed) framesAtMax++;
		if (carController.boosting) framesBoosting++;

		bool Print = (Input.GetAxis ("Submit") != 0);			// Print out all of the relevant info [for debugging]
		bool Cancel = (Input.GetAxis ("Cancel") != 0);

		if (Print && !lastFramePrint) PrintStats ();
		if (Cancel && !lastFrameCancel) PrintOrbStats ();

		// This helps emulate the OnKey method that only reacts once per button press.
		lastFramePrint = Print;
		lastFrameCancel = Cancel;

		// This is so we are always looking at the last frame
		TrickTracking = carController.inAir;
	}

	private void PrintStats (){
		print ("framesAtMax: " + framesAtMax.ToString());
		print ("framesBoosting: " + framesBoosting.ToString());
		print ("framesInAir: " + framesInAir.ToString());
		print ("Ramps: " + rampJumpTotal.ToString());
		print ("Spikes: " + spikeStripTotal.ToString());
		print ("Portals: " + speedPortalTotal.ToString ());
		print ("Jumps: " + jumps.ToString());
	}

	private void PrintOrbStats (){
		print ("Ramp Points: " + rampPoints.ToString());
		print ("Spike Points: " + spikePoints.ToString());
		print ("Speed Points: " + speedPoints.ToString());
		print ("Total Points: " + totalPoints.ToString ());
	}

	public void PlayerInteracted(string objectID){
		// Here we evaluate the ID sent from the interactable and assing points to the appropriate tracker
		if (objectID.StartsWith ("R")) {
												rampJumpTotal++;
			if (objectID.EndsWith ("0")) 		rampL0++;
			else if (objectID.EndsWith ("1")) 	rampL1++;
			else if (objectID.EndsWith("2"))	rampL2++;

		} else if (objectID.StartsWith("K")) {
												spikeStripTotal++;
			if (objectID.EndsWith ("0"))		spikeL0++;
			else if (objectID.EndsWith("1"))	spikeL1++;
			else if (objectID.EndsWith("2"))	spikeL2++;

		} else if (objectID.StartsWith("S")) {
												speedPortalTotal++;
			if (objectID.EndsWith("0"))			speedL0++;
			else if (objectID.EndsWith("1"))	speedL1++;
			else if (objectID.EndsWith("2")) 	speedL2++;
		}

		if (!objectTouched) {
			lastObjectTouched = objectID;
			objectTouched = true;
		}
	}

	private void TrackTricks(){
		if (carController.inAir) {
			float X = Input.GetAxis ("Vertical");
			float Y = Input.GetAxis ("Horizontal");
			bool Z = (Input.GetAxis ("Spin") != 0);

			Xspin += X;
			if (!Z) Yspin += Y;
			else Zspin += Y;

		} else SendInfo (Xspin, Yspin, Zspin);
	}

	private void SendInfo (float x, float y, float z){
		int flp = Mathf.Abs((int) (x / 40));
		int trn = Mathf.Abs((int) (y / 40));
		int spn = Mathf.Abs((int) (z / 40));

		totalFlips += flp;
		totalTurns += trn;
		totalSpins += spn;

		if (lastObjectTouched.StartsWith ("R")) rampPoints += ((12 * flp) + (10 * trn) + (15 * spn));
		else if (lastObjectTouched.StartsWith ("K")) spikePoints += ((12 * flp) + (10 * trn) + (15 * spn));
		else if (lastObjectTouched.StartsWith ("S")) speedPoints += ((12 * flp) + (10 * trn) + (15 * spn));

		totalPoints += ((12 * flp) + (10 * trn) + (15 * spn));

		TrickTracking = false;
		objectTouched = false;

		Xspin = 0;
		Yspin = 0;
		Zspin = 0;
		lastObjectTouched = "0";
	}

	public void OrbCollected(string OrbID){
		orbs++;

		if (OrbID.StartsWith ("R")) {
			totalPoints += 10;
			rampPoints += 10;
		} else if (OrbID.StartsWith ("K")) {
			totalPoints += 15;
			spikePoints += 15;
		} else if (OrbID.StartsWith ("S")) {
			totalPoints += 20;
			speedPoints += 20;
		}
	}

	public void PlayerJumped() {
		jumps++;
	}
}
