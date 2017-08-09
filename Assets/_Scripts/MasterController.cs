using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class Level{
	public int points;
	public List<int> stats = new List<int>();
	public int num;
	public string ID;
	public int preference;

	public Level (string id, int pref){
		ID = id;
		preference = pref;
	}
}
public class StatTracker {
	public int Points;
	public List<int> Stats = new List<int>();
	public int numTotal;
	public string ID;
	public int Preference;

	public Level L0 = new Level("0", 33);
	public Level L1 = new Level("1", 33);
	public Level L2 = new Level("2", 33);

	public StatTracker(string id, int pref){
		ID = id;
		Preference = pref;
	}
}

public class MasterController : MonoBehaviour {
	private int totalPoints;		// Total points based on tricks and orbs collected
	public static int orbs;

	public static List<StatTracker> Trackers = new List<StatTracker>();
	public static StatTracker Ramp = new StatTracker("R", 25);
	public static StatTracker Speed = new StatTracker("S", 50);
	public static StatTracker Spike = new StatTracker("K", 75);
	public static StatTracker Wall = new StatTracker("W", 100);

	public static int framesInAir;		// Used to evaluate the engagement of the player
	public static int framesAtMax;
	public static int framesBoosting;
	public static int framesOnBack;
	public static int framesDrifting;
	public static int timesReset;

	private int totalFlips;			// How many tricks the player does over all
	private int totalTurns;
	private int totalSpins;

	public static int jumps;				// How many times a player jumps
	public static int deaths;

	private GameObject player;
	private Rigidbody carRB;
	private bool lastFrameCancel = false;
	private bool StatTracking;
	public bool objectTouched;

	private float Xspin;			// Track input from player to determine flips/spins/turns
	private float Yspin;
	private float Zspin;

	private int PostObjectAir;	// Track stats of player to determine ability with each interactable
	private int PostObjectBack;
	private int PostObjectMax;
	private int PostObjectPoints;

	private StatTracker currentTracker;
	private Level currentLevel;
	
	private float AvgSpeed;
	private int qty;
	public static bool inObject;
	public static int seed;
	
	public Text PointDisplay;

	void Start() {
		StatTracking = false;
		objectTouched = false;

		player = GameObject.FindGameObjectWithTag("Player");
		carRB = player.GetComponent<Rigidbody> ();

		Trackers.Add(Ramp);
		Trackers.Add(Speed);
		Trackers.Add(Spike);
		Trackers.Add(Wall);

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
				//PrintStats ();
				PrintPointStats ();
			}

			// This helps emulate the OnKey method that only reacts once per button press.
			lastFrameCancel = Cancel;

			// This is so we are always looking at the last frame
			StatTracking = _CarController.inAir;

			if (PointDisplay != null) PointDisplay.text = totalPoints.ToString();

		}
	}

	private void UpdateAverageSpeed(float newSpeed){
		++qty;
		AvgSpeed += (newSpeed - AvgSpeed)/qty;
	}

	private void PrintStats(){
		print ("framesAtMax: " + framesAtMax.ToString());
		print ("framesBoosting: " + framesBoosting.ToString());
		print ("framesInAir: " + framesInAir.ToString());
		print ("framesDrifting: " + framesDrifting.ToString ());
		print ("framesOnBack: " + framesOnBack.ToString ());
	}

	private void PrintPointStats(){
		print ("Ramp Points: " + Ramp.Points.ToString());
		print ("Spike Points: " + Spike.Points.ToString());
		print ("Speed Points: " + Speed.Points.ToString());
		print ("Wall Points: " + Wall.Points.ToString ());
		print ("Total Points: " + totalPoints.ToString ());
	}

	public void PlayerInteracted(string id){
		// Here we evaluate the ID sent from the interactable and increment to the appropriate tracker
		StatTracker tracker;
		Level level;

		GetTracker(id, out tracker, out level);

		tracker.numTotal++;
		level.num++;

		// This is used to keep track of the first object touched after the player starts a trick
		if (!objectTouched) {
			//print ("Interact " + tracker.ID);
			currentTracker = tracker;
			currentLevel = level;
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
			if (objectTouched) PointsAdded(addedPoints, PostObjectAir, PostObjectBack, PostObjectMax);
			
			TotalPoints(addedPoints);

			Xspin = 0;
			Yspin = 0;
			Zspin = 0;

			PostObjectAir = 0;
			PostObjectBack = 0;
			PostObjectMax = 0;
			PostObjectPoints = 0;
		}
	}

	public void PointsAdded(int addedPoints, int air = 0, int back = 0, int max = 0){
		PostObjectPoints += addedPoints;

		if (!(_CarController.inAir || inObject)) {
			int preObjectPoints = currentTracker.Points;
			currentTracker.Points += PostObjectPoints;
			currentLevel.points += PostObjectPoints;

			currentTracker.Stats.Add(air);
			currentTracker.Stats.Add(back);
			currentTracker.Stats.Add(max);
			currentTracker.Stats.Add(PostObjectPoints);

			currentLevel.stats.Add(air);
			currentLevel.stats.Add(back);
			currentLevel.stats.Add(max);
			currentLevel.stats.Add(PostObjectPoints);

			bool state1 = (preObjectPoints < 100 && currentTracker.Points > 100);
			bool state2 = (preObjectPoints < 300 && currentTracker.Points > 300);
			
			if (state1) currentTracker.L0.preference = 16;
			if (state2) {
				currentTracker.L0.preference = 11;
				currentTracker.L1.preference = 22;
			}

			// reset all tracking variables
			StatTracking = false;
			objectTouched = false;

			currentLevel = null;
			currentTracker = null;
		}
	}

	public void TotalPoints(int newPoints){
		if (newPoints > 0){
			if (PointDisplay != null) AddPointsController.CreateText(newPoints.ToString(), PointDisplay.transform);
			totalPoints += newPoints;
		}
	}

	private void AIMonitorPlayer(){

	}

	public static void LogDeath(){
		string fileName = string.Format("Logs/DataLog{0}.txt", seed);
        StreamWriter sw = File.CreateText(fileName);

		sw.WriteLine("Stats\nair, back, max, points");
		for (int i = 0; i < Trackers.Count; i++){
			sw.WriteLine(Trackers[i].ID.ToString());
			for (int j = 0; j < Trackers[i].Stats.Count; j++){
				sw.WriteLine(Trackers[i].Stats[j].ToString());
			}
			sw.WriteLine();
		}
        sw.Close();
	}

	private void GetTracker(string id, out StatTracker tracker, out Level level){
		if (id.StartsWith("R")) tracker = Ramp;
		else if (id.StartsWith("S")) tracker = Speed;
		else if (id.StartsWith("K")) tracker = Spike;
		else tracker = Wall;

		if (id.EndsWith("0")) level = tracker.L0;
		else if (id.EndsWith("1")) level = tracker.L1;
		else level = tracker.L2;
	}
}