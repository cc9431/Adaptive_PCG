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
	private List<int> rampAir = new List<int>();
	private List<int> rampAirL0 = new List<int>();
	private List<int> rampAirL1 = new List<int>();
	private List<int> rampAirL2 = new List<int>();

	private int speedPoints;
	private int speedPointsL0;
	private int speedPointsL1;
	private int speedPointsL2;
	private List<int> speedAir = new List<int>();
	private List<int>speedAirL0 = new List<int>();
	private List<int> speedAirL1 = new List<int>();
	private List<int> speedAirL2 = new List<int>();

	private int spikePoints;
	private int spikePointsL0;
	private int spikePointsL1;
	private int spikePointsL2;
	private List<int> spikeAir = new List<int>();
	private List<int> spikeAirL0 = new List<int>();
	private List<int> spikeAirL1 = new List<int>();
	private List<int> spikeAirL2 = new List<int>();

	private int wallPoints;
	private int wallPointsL0;
	private int wallPointsL1;
	private int wallPointsL2;
	private List<int> wallAir = new List<int>();
	private List<int> wallAirL0 = new List<int>();
	private List<int> wallAirL1 = new List<int>();
	private List<int> wallAirL2 = new List<int>();

	private int framesInAir;		// Used to evaluate the engagement of the player
	private int framesAtMax;
	private int framesBoosting;
	private int framesOnBack;
	private int framesDrifting;
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
	private _CarController carController;
	private Rigidbody carRB;
	private bool lastFrameCancel = false;
	private bool lastFrameAlive;
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
		lastObjectTouched = "0";
		StatTracking = false;
		objectTouched = false;

		player = GameObject.FindGameObjectWithTag("Player");
		carController = player.GetComponent<_CarController> ();
		carRB = player.GetComponent<Rigidbody> ();

		print(seed);

		AddPointsController.Initialize();
	}

	void Update () {
		if (carController.Alive) {
			// This will call TrackTricks for every frame that the car
			// is in the air and the first frame that the car touched back down
			// it also only tracks player input if the car is "freely flying"
			// (meaning that none of the cars different colliders are touching anything)
			if ((StatTracking || carController.inAir || inObject) && (!carController.onBack)) TrackStats ();

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
			StatTracking = carController.inAir;

			if (PointDisplay != null) PointDisplay.text = totalPoints.ToString();
		} else{
			Time.timeScale = 0.5f;
			if (lastFrameAlive) PrintDeath();
		}

		lastFrameAlive = carController.Alive;
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

	private void PrintDeath(){
		string fileName = "Logs/DataLog" + seed.ToString() + ".txt";
		
        StreamWriter sw = File.CreateText(fileName);
		for (int i = 0; i < speedAir.Count; i++){
			sw.WriteLine ("speedAir {0}", speedAir[i]);
		}
		for (int i = 0; i < rampAir.Count; i++){
        	sw.WriteLine ("rampAir {0}", rampAir[i]);
		}
		for (int i = 0; i < spikeAir.Count; i++){
        	sw.WriteLine ("spikeAir {0}", spikeAir[i]);
		}
		for (int i = 0; i < wallAir.Count; i++){
        	sw.WriteLine ("wallAir {0}", wallAir[i]);
		}
        sw.Close();
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
		if (carController.inAir || inObject) {
			float X = Input.GetAxis ("Vertical");
			float Y = Input.GetAxis ("Horizontal");
			bool Z = (Input.GetAxis ("Spin") != 0);

			if (carController.inAir) PostObjectAir++;
			if (carController.onBack) PostObjectBack++;
			if (carController.maxSpeed) PostObjectMax++;

			Xspin += X;
			if (!Z) Yspin += Y;
			else Zspin += Y;

		} else SendInfo (Xspin, Yspin, Zspin, PostObjectAir, PostObjectBack, PostObjectMax);
	}

	private void SendInfo (float x, float y, float z, int air, int back, int max){
		int flp = Mathf.Abs((int) (x / 25));
		int trn = Mathf.Abs((int) (y / 25));
		int spn = Mathf.Abs((int) (z / 25));

		//print(z.ToString());

		totalFlips += flp;
		totalTurns += trn;
		totalSpins += spn;

		int addedPoints = ((12 * flp) + (10 * trn) + (15 * spn));

		if (lastObjectTouched.StartsWith ("R")) {
			rampPoints += addedPoints;
			rampAir.Add(air);
			rampAir.Add(back);
			rampAir.Add(max);
			
			if (lastObjectTouched.EndsWith ("0")){
				PointsAdded(ref rampPointsL0, addedPoints);
				rampAirL0.Add(air);
				rampAirL0.Add(back);
				rampAirL0.Add(max);
			}
			else if (lastObjectTouched.EndsWith ("1")){
				PointsAdded(ref rampPointsL1, addedPoints);
				rampAirL1.Add(air);
				rampAirL1.Add(back);
				rampAirL1.Add(max);
			}
			else if (lastObjectTouched.EndsWith("2")){
				PointsAdded(ref rampPointsL2, addedPoints);
				rampAirL2.Add(air);
				rampAirL2.Add(back);
				rampAirL2.Add(max);
			}
		} else if (lastObjectTouched.StartsWith ("K")) {
			spikePoints += addedPoints;
			spikeAir.Add(air);
			spikeAir.Add(back);
			spikeAir.Add(max);

			if (lastObjectTouched.EndsWith ("0")){
				PointsAdded(ref spikePointsL0, addedPoints);
				spikeAirL0.Add(air);
				spikeAirL0.Add(back);
				spikeAirL0.Add(max);
			}
			else if (lastObjectTouched.EndsWith ("1")){
				PointsAdded(ref spikePointsL1, addedPoints);
				spikeAirL1.Add(air);
				spikeAirL1.Add(back);
				spikeAirL1.Add(max);
			}
			else if (lastObjectTouched.EndsWith("2")){
				PointsAdded(ref spikePointsL2, addedPoints);
				spikeAirL2.Add(air);
				spikeAirL2.Add(back);
				spikeAirL2.Add(max);
			}

		} else if (lastObjectTouched.StartsWith ("S")) {
			speedPoints += addedPoints;
			speedAir.Add(air);
			speedAir.Add(back);
			speedAir.Add(max);

			if (lastObjectTouched.EndsWith ("0")){
				PointsAdded(ref speedPointsL0, addedPoints);
				speedAirL0.Add(air);
				speedAirL0.Add(back);
				speedAirL0.Add(max);
			}
			else if (lastObjectTouched.EndsWith ("1")){
				PointsAdded(ref speedPointsL1, addedPoints);
				speedAirL1.Add(air);
				speedAirL1.Add(back);
				speedAirL1.Add(max);
			}
			else if (lastObjectTouched.EndsWith("2")){
				PointsAdded(ref speedPointsL2, addedPoints);
				speedAirL2.Add(air);
				speedAirL2.Add(back);
				speedAirL2.Add(max);
			}
				
		} else if (lastObjectTouched.StartsWith ("W")) {
			wallPoints += addedPoints;
			wallAir.Add(air);
			wallAir.Add(back);
			wallAir.Add(max);

			if (lastObjectTouched.EndsWith ("0")){
				PointsAdded(ref wallPointsL0, addedPoints);
				wallAirL0.Add(air);
				wallAirL0.Add(back);
				wallAirL0.Add(max);
			}
			else if (lastObjectTouched.EndsWith ("1")){
				PointsAdded(ref wallPointsL1, addedPoints);
				wallAirL1.Add(air);
				wallAirL1.Add(back);
				wallAirL1.Add(max);
			}
			else if (lastObjectTouched.EndsWith("2")){
				PointsAdded(ref wallPointsL2, addedPoints);
				wallAirL2.Add(air);
				wallAirL2.Add(back);
				wallAirL2.Add(max);
			}
		}

		TotalPoints(addedPoints);

		StatTracking = false;
		objectTouched = false;

		Xspin = 0;
		Yspin = 0;
		Zspin = 0;

		PostObjectAir = 0;
		PostObjectBack = 0;
		PostObjectMax = 0;

		lastObjectTouched = "0";
	}

	public void OrbCollected(string OrbID){
		int addedPoints = 0;
		orbs++;

		if (OrbID.StartsWith ("R")) {
			addedPoints = 10;
			rampPoints += addedPoints;
			
			if (OrbID.EndsWith ("0"))
				PointsAdded(ref rampPointsL0, addedPoints);
			else if (OrbID.EndsWith ("1"))
				PointsAdded(ref rampPointsL1, addedPoints);
			else if (OrbID.EndsWith("2"))
				PointsAdded(ref rampPointsL2, addedPoints);

		} else if (OrbID.StartsWith ("K")) {
			addedPoints = 15;
			spikePoints += addedPoints;

			if (OrbID.EndsWith ("0"))
				PointsAdded(ref spikePointsL0, addedPoints);
			else if (OrbID.EndsWith ("1"))
				PointsAdded(ref spikePointsL1, addedPoints);
			else if (OrbID.EndsWith("2"))
				PointsAdded(ref spikePointsL2, addedPoints);

		} else if (OrbID.StartsWith ("S")) {
			addedPoints = 20;
			speedPoints += addedPoints;

			if (OrbID.EndsWith ("0"))
				PointsAdded(ref speedPointsL0, addedPoints);
			else if (OrbID.EndsWith ("1"))
				PointsAdded(ref speedPointsL1, addedPoints);
			else if (OrbID.EndsWith("2"))
				PointsAdded(ref speedPointsL2, addedPoints);
				
		} else if (OrbID.StartsWith ("W")) {
			addedPoints = 25;
			wallPoints += addedPoints;

			if (OrbID.EndsWith ("0"))
				PointsAdded(ref wallPointsL0, addedPoints);
			else if (OrbID.EndsWith ("1"))
				PointsAdded(ref wallPointsL1, addedPoints);
			else if (OrbID.EndsWith("2"))
				PointsAdded(ref wallPointsL2, addedPoints);
		}

		TotalPoints(addedPoints);
	}

	void PointsAdded(ref int pointAllocation, int newPoints){
		if (newPoints > 0){
			if (carController.inAir || inObject){
				PostObjectPoints += newPoints;
			} else {
				pointAllocation += PostObjectPoints;
				PostObjectPoints = 0;
				//AIMonitorPlayer();
			}
		}
	}

	void TotalPoints(int newPoints){
		if (newPoints > 0){
			if (PointDisplay != null) AddPointsController.CreateText(newPoints.ToString(), PointDisplay.transform);
			totalPoints += newPoints;
		}
	}

	private void AIMonitorPlayer(){

	}
}
