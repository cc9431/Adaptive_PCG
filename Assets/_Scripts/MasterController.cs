using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class MasterController : MonoBehaviour {
	private int totalPoints;		// Total points based on tricks and orbs collected

	private int orbs;
	private int rampPoints;			// The points are based on how many orbs are collected from each interactable
	private int rampPointsL0;
	private int rampPointsL1;
	private int rampPointsL2;
	private List<int> rampStats = new List<int>();
	private List<int> rampStatsL0 = new List<int>();
	private List<int> rampStatsL1 = new List<int>();
	private List<int> rampStatsL2 = new List<int>();

	private int speedPoints;
	private int speedPointsL0;
	private int speedPointsL1;
	private int speedPointsL2;
	private List<int> speedStats = new List<int>();
	private List<int>speedStatsL0 = new List<int>();
	private List<int> speedStatsL1 = new List<int>();
	private List<int> speedStatsL2 = new List<int>();

	private int spikePoints;
	private int spikePointsL0;
	private int spikePointsL1;
	private int spikePointsL2;
	private List<int> spikeStats = new List<int>();
	private List<int> spikeStatsL0 = new List<int>();
	private List<int> spikeStatsL1 = new List<int>();
	private List<int> spikeStatsL2 = new List<int>();

	private int wallPoints;
	private int wallPointsL0;
	private int wallPointsL1;
	private int wallPointsL2;
	private List<int> wallStats = new List<int>();
	private List<int> wallStatsL0 = new List<int>();
	private List<int> wallStatsL1 = new List<int>();
	private List<int> wallStatsL2 = new List<int>();

	public static int framesInAir;		// Used to evaluate the engagement of the player
	public static int framesAtMax;
	public static int framesBoosting;
	public static int framesOnBack;
	public static int framesDrifting;
	public static int timesReset;

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

	public static int jumps;				// How many times a player jumps

	private GameObject player;
	private Rigidbody carRB;
	private bool lastFrameCancel = false;
	private bool loggedDeath = false;
	private bool StatTracking;
	private bool objectTouched;
	private string lastObjectTouched;

	private float Xspin;			// Track input from player to determine flips/spins/turns
	private float Yspin;
	private float Zspin;

	private int PostObjectAir;	// Track stats of player to determine ability with each interactable
	private int PostObjectBack;
	private int PostObjectMax;
	private int PostObjectPoints;
	
	private float AvgSpeed;
	private int qty;
	public static bool inObject;
	public static int seed;
	
	public Text PointDisplay;

	void Start() {
		lastObjectTouched = "FALSE";
		StatTracking = false;
		objectTouched = false;

		player = GameObject.FindGameObjectWithTag("Player");
		carRB = player.GetComponent<Rigidbody> ();

		//print(seed);

		AddPointsController.Initialize();
	}

	void FixedUpdate () {
		if (_CarController.Alive) {
			// This will call TrackTricks for every frame that the car
			// is in the air and the first frame that the car touched back down
			// it also only tracks player input if the car is "freely flying"
			// (meaning that none of the cars different colliders are touching anything)
			if (StatTracking || _CarController.inAir || inObject) TrackStats ();

			// Call this function to update the average speed
			UpdateAverageSpeed (carRB.velocity.magnitude);

			bool Cancel = (Input.GetAxis ("Cancel") != 0);

			// printing functions to give myself information on the game
			if (Cancel && !lastFrameCancel) {
				PrintStats ();
				//PrintPointStats ();
			}

			// This helps emulate the OnKey method that only reacts once per button press.
			lastFrameCancel = Cancel;

			// This is so we are always looking at the last frame
			StatTracking = _CarController.inAir;

			if (PointDisplay != null) PointDisplay.text = totalPoints.ToString();
		} else{
			Time.timeScale = 0.5f;
			if (!loggedDeath){
				LogDeath();
				loggedDeath = true;
			}
		}
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
		print ("framesOnBack: " + framesOnBack.ToString ());
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
			//print ("Interact " + objectID);
			//print ("Master inObject " + inObject.ToString());
			lastObjectTouched = objectID;
			objectTouched = true;
			PostObjectPoints = 0;
		}
	}

	private void TrackStats(){
		if (_CarController.inAir || inObject) {
			float X = Input.GetAxis ("Vertical");
			float Y = Input.GetAxis ("Horizontal");
			bool Z = (Input.GetAxis ("Spin") != 0);

			if (_CarController.inAir) PostObjectAir++;
			if (_CarController.onBack) PostObjectBack++;
			if (_CarController.maxSpeed) PostObjectMax++;

			Xspin += X;
			if (!Z) Yspin += Y;
			else Zspin += Y;
		} else {
			int flp = Mathf.Abs((int) (Xspin / 25));
			int trn = Mathf.Abs((int) (Yspin / 25));
			int spn = Mathf.Abs((int) (Zspin / 25));

			totalFlips += flp;
			totalTurns += trn;
			totalSpins += spn;

			int addedPoints = ((12 * flp) + (10 * trn) + (15 * spn));
			//print("SendInfoPoints: " + addedPoints.ToString() + " inAir: " + _CarController.inAir.ToString());
			InteractPoints(false, addedPoints, lastObjectTouched, PostObjectAir, PostObjectBack, PostObjectMax);
		}
	}

	public void InteractPoints(bool orb, int addedPoints, string inputID = "FALSE", int air = 0, int back = 0, int max = 0){ 
		if (orb) {
			orbs++;
			inputID = lastObjectTouched;
		}

		//print("InteractPoints: " + addedPoints.ToString() + inputID);

		if (inputID.StartsWith ("R")) {
			rampPoints += addedPoints;
			
			if (inputID.EndsWith ("0")){
				PointsAdded(ref rampPointsL0, ref rampStatsL0, ref rampStats,
					addedPoints, air, back, max);
			}
			else if (inputID.EndsWith ("1")){
				PointsAdded(ref rampPointsL1, ref rampStatsL1, ref rampStats,
					addedPoints, air, back, max);
			}
			else if (inputID.EndsWith("2")){
				PointsAdded(ref rampPointsL2, ref rampStatsL2, ref rampStats,
					addedPoints, air, back, max);
			}
		} else if (inputID.StartsWith ("K")) {
			spikePoints += addedPoints;

			if (inputID.EndsWith ("0")){
				PointsAdded(ref spikePointsL0, ref spikeStatsL0, ref spikeStats,
					addedPoints, air, back, max);
			}
			else if (inputID.EndsWith ("1")){
				PointsAdded(ref spikePointsL1, ref spikeStatsL1, ref spikeStats,
					addedPoints, air, back, max);
			}
			else if (inputID.EndsWith("2")){
				PointsAdded(ref spikePointsL2, ref spikeStatsL2, ref spikeStats,
					addedPoints, air, back, max);
			}

		} else if (inputID.StartsWith ("S")) {
			speedPoints += addedPoints;

			if (inputID.EndsWith ("0")){
				PointsAdded(ref speedPointsL0, ref speedStatsL0, ref speedStats,
					addedPoints, air, back, max);
			}
			else if (inputID.EndsWith ("1")){
				PointsAdded(ref speedPointsL1, ref speedStatsL1, ref speedStats,
					addedPoints, air, back, max);
			}
			else if (inputID.EndsWith("2")){
				PointsAdded(ref speedPointsL2, ref speedStatsL2, ref speedStats,
					addedPoints, air, back, max);
			}
				
		} else if (inputID.StartsWith ("W")) {
			wallPoints += addedPoints;

			if (inputID.EndsWith ("0")){
				PointsAdded(ref wallPointsL0, ref wallStatsL0, ref wallStats,
					addedPoints, air, back, max);
			}
			else if (inputID.EndsWith ("1")){
				PointsAdded(ref wallPointsL1, ref wallStatsL1, ref wallStats,
					addedPoints, air, back, max);
			}
			else if (inputID.EndsWith("2")){
				PointsAdded(ref wallPointsL2, ref wallStatsL2, ref wallStats,
					addedPoints, air, back, max);
			}
		}

		TotalPoints(addedPoints);
	}

	void PointsAdded(ref int pointAllocation, ref List<int> statAllocation, ref List<int> statTotalAllocation,
						int newPoints, int air, int back, int max){	
		if (_CarController.inAir || inObject) {
			PostObjectPoints += newPoints;
		} else {
			PostObjectPoints += newPoints;
			pointAllocation += PostObjectPoints;

			statAllocation.Add(air);
			statAllocation.Add(back);
			statAllocation.Add(max);
			statAllocation.Add(PostObjectPoints);

			statTotalAllocation.Add(air);
			statTotalAllocation.Add(back);
			statTotalAllocation.Add(max);
			statTotalAllocation.Add(PostObjectPoints);

			//AIMonitorPlayer(PostObjectAir, PostObjectBack, PostObjectMax, PostObjectPoints);

			// Zero out all tracking variables
			StatTracking = false;
			objectTouched = false;

			Xspin = 0;
			Yspin = 0;
			Zspin = 0;

			PostObjectAir = 0;
			PostObjectBack = 0;
			PostObjectMax = 0;
			PostObjectPoints = 0;

			lastObjectTouched = "FALSE";
		}

		//print("PostObjectPoints: " + PostObjectPoints.ToString());
		//print("inAir or inObject: " + (_CarController.inAir || inObject).ToString());
	}

	void TotalPoints(int newPoints){
		if (newPoints > 0){
			if (PointDisplay != null) AddPointsController.CreateText(newPoints.ToString(), PointDisplay.transform);
			totalPoints += newPoints;
		}
	}

	private void AIMonitorPlayer(int air, int back, int max, int points){
		/*
		GenerateInfiniteFull.RampPrefMax;
		GenerateInfiniteFull.SpeedPrefMax;
		GenerateInfiniteFull.SpikePrefMax;
		GenerateInfiniteFull.WallPrefMax;

		for(int i = 0; i < rampStats.Count; i++){

		}
		for(int i = 0; i < spikeStats.Count; i++){

		}
		for(int i = 0; i < speedStats.Count; i++){

		}
		for(int i = 0; i < wallStats.Count; i++){

		}
		*/
	}

	private void LogDeath(){
		string fileName = string.Format("Logs/DataLog{0}.txt", seed);
		
        StreamWriter sw = File.CreateText(fileName);
		for (int i = 0; i < rampStats.Count; i++){
        	sw.WriteLine ("rampStats:{0}", rampStats[i]);
		}
		sw.WriteLine();
		for (int i = 0; i < speedStats.Count; i++){
        	sw.WriteLine ("speedStats:{0}", speedStats[i]);
		}
		sw.WriteLine();
		for (int i = 0; i < spikeStats.Count; i++){
        	sw.WriteLine ("spikeStats:{0}", spikeStats[i]);
		}
		sw.WriteLine();
		for (int i = 0; i < wallStats.Count; i++){
        	sw.WriteLine ("wallStats:{0}", wallStats[i]);
		}
        sw.Close();
	}
}