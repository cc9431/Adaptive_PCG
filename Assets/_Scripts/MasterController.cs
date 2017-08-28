using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Level {
	public int Points;
	public List<KeyValuePair<float, int>> Stats = new List<KeyValuePair<float, int>>();
	public float[] avgStats = new float[4];
	public int numTotal;
	public int ID;
	public int Preference;

	public Level(int id, int pref){
		ID = id;
		Preference = pref;
	}

	public float[] AverageStats(){
		int sum, count;

		for (int i = 0; i < avgStats.Length; i++){
			sum = 0;
			for (int j = i; j < Stats.Count; j += 4){
				sum += Stats[j].Value;
			}
			count = Stats.Count/4;
			avgStats[i] = (sum/count);
		}

		return avgStats;
	}
}

public class StatTracker {
	public int Points;
	public List<KeyValuePair<float, int>> Stats = new List<KeyValuePair<float, int>>();
	public float[] avgStats = new float[4];
	public int numTotal;
	public int ID;
	public int Preference;
	public string name;

	public List<Level> Levels = new List<Level>();
	public Level L0 = new Level(0, 33);
	public Level L1 = new Level(1, 33);
	public Level L2 = new Level(2, 33);


	public StatTracker(int id, int pref, string nm){
		ID = id;
		Preference = pref;
		name = nm;
	}

	public float[] AverageStats(){
		int count = 0;

		for (int i = 0; i < Levels.Count; i++){
			if (Levels[i].numTotal > 0){
				for (int j = 0; j < avgStats.Length; j++){
					avgStats[j] += Levels[i].avgStats[j]; 
				}
				count++;
			}
		}

		for (int k = 0; k < avgStats.Length; k++) {
			avgStats[k] /= count;
		}
		
		return avgStats;
	}

	public int getPoints(){
		int sum = 0;

		sum = Levels[0].Points;
		sum += Levels[1].Points;
		sum += Levels[2].Points;

		Points = sum;
		return Points;
	}

	public int getNumTotal(){
		int sum = 0;

		for(int i = 0; i < Levels.Count; i++){
			sum += Levels[i].numTotal;
		}

		numTotal = sum;
		return numTotal;
	}
}

public class MasterController : MonoBehaviour {
	private int totalPoints;		// Total points based on tricks and orbs collected
	public static int orbs;

	public static List<StatTracker> Trackers;
	public static StatTracker Ramp;
	public static StatTracker Speed;
	public static StatTracker Spike;
	public static StatTracker Wall;

	public static int framesInAir;		// Used to evaluate the engagement of the player
	public static int framesAtMax;
	public static int framesBoosting;
	public static int framesOnBack;
	public static int framesDrifting;
	public static int timesReset;

	private int totalFlips;			// How many tricks the player does over all
	private int totalTurns;
	private int totalSpins;

	private float[,,] ExpectedValues = new float[4,3,4];
	private float[,] statSum = new float[4,3];
	private float[] Weights = new float[4];
	private float[] TrackerSums = new float[4];
	private float[] LevelSums = new float[3];
	private float TotalSum;
	
	private int qty;
	private float AvgSpeed;
	public static int jumps;				// How many times a player jumps
	public static int deaths;

	private _CarController carController;
	private GameObject player;
	private Rigidbody carRB;
	private bool lastFrameCancel = false;
	private bool StatTracking;
	public static bool objectTouched;

	private float Xspin;			// Track input from player to determine flips/spins/turns
	private float Yspin;
	private float Zspin;

	private int PostObjectAir;	// Track stats of player to determine ability with each interactable
	private int PostObjectBack;
	private int PostObjectPoints;
	public static int PostObjectSpeed;

	private StatTracker currentTracker;
	private Level currentLevel;

	public static bool inObject;
	public static int seed;
	public static string[] statList = new string[4];
	public Text PointDisplay;

	void OnEnable() {
		SceneManager.sceneLoaded += OnSceneLoaded;
	}

	void OnDisable() {
		SceneManager.sceneLoaded -= OnSceneLoaded;
	}

	private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
		orbs = 0;
		jumps = 0;
		deaths = 0;

		framesInAir = 0;
		framesAtMax = 0;
		framesBoosting = 0;
		framesOnBack = 0;
		framesDrifting = 0;
		timesReset = 0;

		inObject = false;
		StatTracking = false;
		objectTouched = false;
	
		Ramp  = new StatTracker(0, 100/4, "Ramp");
		Speed = new StatTracker(1, 100/4, "Speed");
		Spike = new StatTracker(2, 100/4, "Spike");
		Wall  = new StatTracker(3, 100/4, "Wall");

		Trackers = new List<StatTracker>();
		Trackers.Add(Ramp);
		Trackers.Add(Speed);
		Trackers.Add(Spike);
		Trackers.Add(Wall);
		for (int i = 0; i < Trackers.Count; i++){
			Trackers[i].Levels.Add(Trackers[i].L0);
			Trackers[i].Levels.Add(Trackers[i].L1);
			Trackers[i].Levels.Add(Trackers[i].L2);
		}
	}

	void Start() {
		statList[0] = "Air";
		statList[1] = "Back";
		statList[2] = "Speed";
		statList[3] = "Points";

		Weights[0] = 0.4f;
		Weights[1] = 0.6f;
		Weights[2] = 0.4f;
		Weights[3] = 0.8f;

		player = GameObject.FindGameObjectWithTag("Player");
		carRB = player.GetComponent<Rigidbody> ();
		carController = player.GetComponent<_CarController>();

		for (int type = 0; type < 4; type++){
			for (int lev = 0; lev < 3; lev++){
				statSum[type, lev] = Weights[0] + Weights[1] + Weights[2] + Weights[3];

				float sum = statSum[type, lev];

				TotalSum 			+= sum;
				TrackerSums[type] 	+= sum;
				LevelSums[lev] 		+= sum;
			}
		}

		// Ramp expected values
		// RampL0
		ExpectedValues[0,0,0] = 65f;	// Air
		ExpectedValues[0,0,1] = 3f;   	// Back
		ExpectedValues[0,0,2] = 60f;	// Speed
		ExpectedValues[0,0,3] = 35f;	// Points

		ExpectedValues[0,1,0] = 135f;	// Air
		ExpectedValues[0,1,1] = 4f;		// Back
		ExpectedValues[0,1,2] = 55f;	// Speed
		ExpectedValues[0,1,3] = 50f;	// Points
		// RampL2
		ExpectedValues[0,2,0] = 225f; 	// Air
		ExpectedValues[0,2,1] = 4f;  	// Back
		ExpectedValues[0,2,2] = 30f;  	// Speed
		ExpectedValues[0,2,3] = 80f; 	// Points
	
		//SpeedL0
		ExpectedValues[1,0,0] = 1f; 	// Air
		ExpectedValues[1,0,1] = 1f;  	// Back
		ExpectedValues[1,0,2] = 80f;  	// Speed
		ExpectedValues[1,0,3] = 20f; 	// Points
		//SpeedL1
		ExpectedValues[1,1,0] = 75; 	// Air
		ExpectedValues[1,1,1] = 4f;  	// Back
		ExpectedValues[1,1,2] = 80f;  	// Speed
		ExpectedValues[1,1,3] = 25f; 	// Points
		//SpeedL2
		ExpectedValues[1,2,0] = 80; 	// Air
		ExpectedValues[1,2,1] = 4f;  	// Back
		ExpectedValues[1,2,2] = 80f;  	// Speed
		ExpectedValues[1,2,3] = 40f; 	// Points
		
		//SpikedL0
		ExpectedValues[2,0,0] = 60f; 	// Air
		ExpectedValues[2,0,1] = 1f;  	// Back
		ExpectedValues[2,0,2] = 40f;  	// Speed
		ExpectedValues[2,0,3] = 30f; 	// Points
		//SpikeL1
		ExpectedValues[2,1,0] = 60f; 	// Air
		ExpectedValues[2,1,1] = 1f;  	// Back
		ExpectedValues[2,1,2] = 45f;  	// Speed
		ExpectedValues[2,1,3] = 30; 	// Points
		//SpikeL2
		ExpectedValues[2,2,0] = 60f; 	// Air
		ExpectedValues[2,2,1] = 1f;  	// Back
		ExpectedValues[2,2,2] = 50f;  	// Speed
		ExpectedValues[2,2,3] = 55f; 	// Points
		
		//WallL0
		ExpectedValues[3,0,0] = 1f; 	// Air
		ExpectedValues[3,0,1] = 1f;  	// Back
		ExpectedValues[3,0,2] = 70f;  	// Speed
		ExpectedValues[3,0,3] = 25f; 	// Points
		//WallL1
		ExpectedValues[3,1,0] = 4f; 	// Air
		ExpectedValues[3,1,1] = 1f;  	// Back
		ExpectedValues[3,1,2] = 70f;  	// Speed
		ExpectedValues[3,1,3] = 45f; 	// Points
		//WallL2
		ExpectedValues[3,2,0] = 70f; 	// Air
		ExpectedValues[3,2,1] = 1f;  	// Back
		ExpectedValues[3,2,2] = 50f;  	// Speed
		ExpectedValues[3,2,3] = 65f; 	// Points

		print(seed);

		AddPointsController.Initialize();
	}

	void LateUpdate () {
		if (_CarController.Alive) {
			// This will call TrackTricks for every frame that the car
			// is in the air and the first frame that the car touched back down
			// it also only tracks player input if the car is "freely flying"
			// (meaning that none of the cars different colliders are touching anything)
			//if (inObject) print ("Interact inObject: " + inObject.ToString());

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
			StatTracking = _CarController.inAir || inObject;

			if (PointDisplay != null) PointDisplay.text = totalPoints.ToString();

		} else {
			currentLevel = null;
			currentTracker = null;
			objectTouched = false;
		}
	}

	private void UpdateAverageSpeed(float newSpeed){
		++qty;
		AvgSpeed += (newSpeed - AvgSpeed)/qty;
	}

	private void PrintStats(){
		print ("SpeedatExit: " + framesAtMax.ToString());
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

	public void PlayerInteracted(int idTrack, int idLev){
		// Here we evaluate the ID sent from the interactable and increment to the appropriate tracker
		StatTracker tracker = Trackers[idTrack];
		Level level = tracker.Levels[idLev];

		level.numTotal++;

		// This is used to keep track of the first object touched after the player starts a trick
		if (!objectTouched) {
			//print ("Interact " + tracker.ID);
			currentTracker = tracker;
			currentLevel = level;
			objectTouched = true;

			Xspin = 0;
			Yspin = 0;
			Zspin = 0;

			PostObjectSpeed = 0;
			PostObjectAir = 0;
			PostObjectBack = 0;
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
			PostObjectSpeed = (int) carRB.velocity.magnitude;
			//print("SendInfoPoints: " + addedPoints.ToString() + " inAir: " + _CarController.inAir.ToString());
			if (objectTouched) PointsAdded(addedPoints, PostObjectAir, PostObjectBack, PostObjectSpeed);
			
			DisplayPoints(addedPoints);

			Xspin = 0;
			Yspin = 0;
			Zspin = 0;

			PostObjectSpeed = 0;
			PostObjectAir = 0;
			PostObjectBack = 0;
			PostObjectPoints = 0;
		}
	}

	public void PointsAdded(int addedPoints, int air = 0, int back = 0, int speed = 0){
		PostObjectPoints += addedPoints;
		
		if (!(_CarController.inAir || inObject)) {
			int preObjectPoints = currentTracker.getPoints();
			currentLevel.Points += PostObjectPoints;

			currentLevel.Stats.Add(new KeyValuePair<float, int>(Time.timeSinceLevelLoad, air));
			currentLevel.Stats.Add(new KeyValuePair<float, int>(Time.timeSinceLevelLoad, back));
			currentLevel.Stats.Add(new KeyValuePair<float, int>(Time.timeSinceLevelLoad, speed));
			currentLevel.Stats.Add(new KeyValuePair<float, int>(Time.timeSinceLevelLoad, PostObjectPoints));

			bool state1 = (preObjectPoints <= 200 && currentTracker.getPoints() >= 200);

			if (state1) {
				GenerateInfiniteFull.intro = false;
				carController.HorsePower = 2500f;
			}

			// reset tracking variables
			StatTracking = false;
			objectTouched = false;

			if (!GenerateInfiniteFull.intro) AIMonitorPlayer();

			currentLevel = null;
			currentTracker = null;
		}
	}

	public void DisplayPoints(int newPoints){
		if (newPoints > 0){
			if (PointDisplay != null) AddPointsController.CreateText(newPoints.ToString(), PointDisplay.transform);
			totalPoints += newPoints;
		}
	}

	private void AIMonitorPlayer(){
		float value = 0;
		
		currentLevel.AverageStats();

		for (int i = 0; i < 4; i++){
			value += ((currentLevel.avgStats[i]/ExpectedValues[currentTracker.ID, currentLevel.ID, i]) * Weights[i]);
		}

		value += currentLevel.numTotal;
		float diffValue = value - statSum[currentTracker.ID, currentLevel.ID];

		TotalSum += diffValue;
		TrackerSums[currentTracker.ID] += diffValue;
		LevelSums[currentLevel.ID] += diffValue;

		statSum[currentTracker.ID, currentLevel.ID] = value;
		
		for (int j = 0; j < 4; j++){
			Trackers[j].Preference = (int) (100 * (TrackerSums[j]/TotalSum));
			for (int k = 0; k < 3; k++){
				Trackers[j].Levels[k].Preference = (int) (100 * (statSum[j,k]/LevelSums[k]));
			}
		}

		// For printing information
		/*for (int stat = 0; stat < 4; stat++){
			string s = string.Format("L{2}:{0} {1}", statList[stat], currentLevel.avgStats[stat], currentLevel.ID);
			print(s);
		}*/
	}

	public static void LogDeath(){
		string fileName = string.Format("Logs/DataLog{0}.txt", seed);
        StreamWriter sw = File.CreateText(fileName);

		StatTracker track;
		Level lev;
		KeyValuePair<float, int> stat;
		string s;

		sw.WriteLine("Stats\nair, back, speed, points");
		for (int i = 0; i < Trackers.Count; i++){
			track = Trackers[i];

			if (track.getNumTotal() > 0) {
				for (int j = 0; j < track.Levels.Count; j++){
					lev = track.Levels[j];
					
					if (lev.numTotal > 0) {
						foreach(int num in lev.avgStats){
							sw.Write("{0} ", num.ToString());
						}
						for (int k = 0; k < lev.Stats.Count; k++){
							stat = lev.Stats[k];
							if (k%4 == 0) sw.Write("{0}| ", lev.Stats[k].Key);
							s = string.Format("{0}{1} {2}:{3}", track.ID, lev.ID, statList[k%4], stat.Value.ToString());
							sw.Write(s + ", ");
						}
					}
				}
			}
			sw.WriteLine();
		}
        sw.Close();
	}
}