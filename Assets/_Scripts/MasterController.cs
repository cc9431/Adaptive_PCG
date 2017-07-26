using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class MasterController : MonoBehaviour {
	private int totalPoints;		// Total points based on tricks and orbs collected

	private int orbs;
	private int rampPoints;			// The points are based on how many orbs are collected from each interactable
	private int speedPoints;
	private int spikePoints;
	private int wallPoints;

	private int framesInAir;		// Used to evaluate the engagement of the player
	private int framesAtMax;
	private int framesBoosting;
	private int framesOnBack;
	private int framesDrifting;
	private int timesReset;

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

	private int wallDestroyTotal;	// How many times a player runs through a wall
	private int wallL0;
	private int wallL1;
	private int wallL2;

	private int totalFlips;			// How many tricks the player does over all
	private int totalTurns;
	private int totalSpins;

	private int jumps;				// How many times a player jumps

	private GameObject player;
	private _CarController carController;
	private Rigidbody carRB;
	private bool lastFrameCancel = false;
	private bool TrickTracking;
	private bool objectTouched;
	private string lastObjectTouched;

	private float Xspin;			// Track input from player to determine flips/spins/turns
	private float Yspin;
	private float Zspin;
	
	private float AvgSpeed;
	private int qty;
	
	public Text PointDisplay;

	void Start() {
		lastObjectTouched = "0";
		TrickTracking = false;
		objectTouched = false;

		player = GameObject.FindGameObjectWithTag("Player");
		carController = player.GetComponent<_CarController> ();
		carRB = player.GetComponent<Rigidbody> ();

		AddPointsController.Initialize();
	}

	void Update () {
		if (carController.Alive) {
			// This will call TrackTricks for every frame that the car
			// is in the air and the first frame that the car touched back down
			// it also only tracks player input if the car is "freely flying"
			// (meaning that none of the cars different colliders are touching anything)
			if ((TrickTracking || carController.inAir) && (!carController.onBack)) TrackTricks ();

			// Call this function to update the average speed
			UpdateAverageSpeed (carRB.velocity.magnitude);

			// Keep track of player engagement
			if (carController.inAir)
				framesInAir++;
			if (carController.maxSpeed)
				framesAtMax++;
			if (carController.boosting)
				framesBoosting++;
			if (carController.onBack)
				framesOnBack++;
			if (carController.Drift)
				framesDrifting++;

			bool Cancel = (Input.GetAxis ("Cancel") != 0);

			// printing functions to give myself information on the game
			if (Cancel && !lastFrameCancel) {
				PrintStats ();
				PrintPointStats ();
			}

			// This helps emulate the OnKey method that only reacts once per button press.
			lastFrameCancel = Cancel;

			// This is so we are always looking at the last frame
			TrickTracking = carController.inAir;

			if (PointDisplay != null) PointDisplay.text = totalPoints.ToString();
		} else
			Time.timeScale = 0.5f;
	}

	private void UpdateAverageSpeed(float newSpeed){
		++qty;
		AvgSpeed += (newSpeed - AvgSpeed)/qty;
	}

	private void PrintStats (){
		print ("framesAtMax: " + framesAtMax.ToString());
		print ("framesBoosting: " + framesBoosting.ToString());
		print ("framesInAir: " + framesInAir.ToString());
		print ("framesDrifting: " + framesDrifting.ToString ());
	}

	private void PrintPointStats (){
		print ("Ramp Points: " + rampPoints.ToString());
		print ("Spike Points: " + spikePoints.ToString());
		print ("Speed Points: " + speedPoints.ToString());
		print ("Wall Points: " + wallPoints.ToString ());
		print ("Total Points: " + totalPoints.ToString ());
	}

	public void PlayerInteracted(string objectID){
		// Here we evaluate the ID sent from the interactable and increment to the appropriate tracker
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
		} else if (objectID.StartsWith("W")) {
												wallDestroyTotal++;
			if (objectID.EndsWith("0"))			wallL0++;
			else if (objectID.EndsWith("1"))	wallL1++;
			else if (objectID.EndsWith("2")) 	wallL2++;
		}

		// This is used to keep track of the first object touched after the player starts a trick
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

		int addedPoints = ((12 * flp) + (10 * trn) + (15 * spn));

		if (lastObjectTouched.StartsWith ("R")) PointsAdded(ref rampPoints, addedPoints);
		else if (lastObjectTouched.StartsWith ("K")) PointsAdded(ref spikePoints, addedPoints);
		else if (lastObjectTouched.StartsWith ("S")) PointsAdded(ref speedPoints, addedPoints);
		else if (lastObjectTouched.StartsWith ("W")) PointsAdded(ref wallPoints, addedPoints);

		PointsAdded(ref totalPoints, addedPoints);

		TrickTracking = false;
		objectTouched = false;

		Xspin = 0;
		Yspin = 0;
		Zspin = 0;
		lastObjectTouched = "0";
	}

	public void OrbCollected(string OrbID){
		int addedPoints = 0;
		orbs++;

		if (OrbID.StartsWith ("R")) {
			addedPoints = 10;
			PointsAdded(ref rampPoints, addedPoints);
		} else if (OrbID.StartsWith ("K")) {
			addedPoints = 15;
			PointsAdded(ref spikePoints, addedPoints);
		} else if (OrbID.StartsWith ("S")) {
			addedPoints = 20;
			PointsAdded(ref spikePoints, addedPoints);
		} else if (OrbID.StartsWith ("W")) {
			addedPoints = 25;
			PointsAdded(ref wallPoints, addedPoints);
		}

		PointsAdded(ref totalPoints, addedPoints);
	}

	void PointsAdded(ref int pointAllocation, int newPoints){
		if (pointAllocation.Equals(totalPoints) && newPoints > 0)
			AddPointsController.CreateText(newPoints.ToString(), PointDisplay.transform);

		pointAllocation = pointAllocation + newPoints;
	}

	public void PlayerJumped() {
		jumps++;
	}

	public void PlayerReset(){
		timesReset++;
	}

	private void Generation(){
		
	}
}
