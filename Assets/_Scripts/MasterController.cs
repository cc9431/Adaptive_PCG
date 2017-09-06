using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

// This class is for tracking each of the three levels of each type
// Each level has a specific game object model associated to it through the object's ID assigned by the GenerateInfiniteFull script
public class Level {
	public int Points;						// Total number of points made from using this object
	public int numTotal;					// Total number of times the player has interacted with this object
	public int ID;							// This ID relates to which level the object is
	public int Preference;					// The value that decides how often this object is generated
	public List<KeyValuePair<float, int>> Stats = new List<KeyValuePair<float, int>>(); // The 'dictionary' of Stats (list of keyvaluepair because I require non-unique keys)
	public float[] avgStats = new float[4];	// All of the stats tracked by this level averaged out by type
	public float[] stdDevStats = new float[4];

	// Initialization function
	public Level(int id, int pref){
		ID = id;
		Preference = pref; // All objects are given the same preference values at the beginning
	}

	// Function for taking the total list of stats and averaging them for preference analysis
	public float[] AverageStats(int air, int trick, int speed, int points, float time){
		// Add our new stats to the list of stats
		Stats.Add(new KeyValuePair<float, int>(time, air));
		Stats.Add(new KeyValuePair<float, int>(time, trick));
		Stats.Add(new KeyValuePair<float, int>(time, speed));
		Stats.Add(new KeyValuePair<float, int>(time, points));

		// For each slot, update the average by adding in the four most recent stats and calculating the average with those new values
		for (int stat = 0; stat < 4; stat++){
			// Get the most recent stat relating to which slot we are looking at: air, trick, speed, and points
			float statValue = Stats[Stats.Count - (stat + 1)].Value; 
			// Keep track of the difference between the new stat value and the old average (mean)
			float oldDiff = statValue - avgStats[stat];
			// Total count for each stat is (size of the stat list) / 4 since the list keeps track of all 4 different stats
			int statSize = Stats.Count/4;

			/// Recalculate the average (mean): 
			/// newMean = oldMean + ((newValue - oldMean) / count)
			avgStats[stat] += oldDiff / statSize;
			
			// Save the difference between the new value and the new average (mean)
			float newDiff = statValue - avgStats[stat];

			/// if we have more than one set of stats, recalculate the new standard deviation (population):
			/// newStandardDeviation = SquareRoot(newVariance)
			/// newVariance = ((count - 1)oldVariance + (newValue - oldMean)(newValue - newMean)) / count
			/// else the standard deviation is 0 because we only have one input
			if (statSize > 1) stdDevStats[stat] = Mathf.Sqrt((((statSize - 1) * stdDevStats[stat]) + ((oldDiff) * (newDiff))) / statSize );
			else stdDevStats[stat] = 0;
		}

		/*
		//If what I wrote above doesn't work, go back to this
		for (int i = 0; i < avgStats.Length; i++){
			sum = 0;
			// The stats are recorded in the same order every time the player interacts (air, trick, speed, points)
			// Thus to average we just iterate from 0-3, simply jumping four spots from front to back, starting one further each time
			for (int j = i; j < Stats.Count; j += 4){
				sum += Stats[j].Value;
			}
			count = Stats.Count/4;
			avgStats[i] = (sum/count);
		}*/

		return avgStats;
	}
}

// This class is for tracking the overall stats of each type
// This class does not have an associated model for each type, instead it is used more to organize and categorize our data
// This way we have four types with three levels instead of individually tracking all 12 different objects
public class Type {
	public int Points;						// Total number of points made from using this object
	public List<KeyValuePair<float, int>> Stats = new List<KeyValuePair<float, int>>();  // The 'dictionary' of Stats (list of keyvaluepair because I require non-unique keys)
	public float[] avgStats = new float[4];	// All of the stats tracked by this level averaged out by type
	public int numTotal;					// Total number of times the player has interacted with this object
	public int ID;							// This ID relates to which level the object is
	public int Preference;					// The value that decides how often this object is generated
	public string name;						// Name of the class, mostly used for printing data/debugging

	public List<Level> Levels = new List<Level>(); // List of the three levels associated with the class and their initializations
	public Level L0 = new Level(0, 33);
	public Level L1 = new Level(1, 33);
	public Level L2 = new Level(2, 33);

	// Initialization
	public Type(int id, int pref, string nm){
		ID = id;
		Preference = pref;
		name = nm;
	}

	// Function for averaging stats of all three levels for this type
	public float[] AverageStats(){
		int count = 0;

		// For each stat, loop through the three levels.
		// For each level, if there are stats associated with it, sum up the stats from that level
		// At the end, divide the summed stats by how many levels were counted
		for (int j = 0; j < avgStats.Length; j++){
			for (int i = 0; i < Levels.Count; i++){
				if (Levels[i].numTotal > 0){
					avgStats[j] += Levels[i].avgStats[j]; 
					count++;
				}
			}
			avgStats[j] /= count;
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
	private int totalPoints;								// Total points based on tricks and orbs collected
	public static int orbs;									// Total number of orbs collected

	public static List<Type> Types;				// List of four types of objects with the four different types below it
	public static Type Ramp;
	public static Type Speed;
	public static Type Spike;
	public static Type Wall;
	
	// Used to evaluate the engagement of the player
	public static int framesInAir;							// Number of frames spent in the air in total
	public static int framesAtMax;							// Number of frames spent at max speed in total
	public static int framesBoosting;						// Number of frames spent boosting in total
	public static int framesOnBack;							// Number of frames spent on back in total
	public static int framesDrifting;						// Number of frames spent drifting in total
	public static int timesReset;							// Number of frames reset from being stuck

	// How many tricks the player does over all
	private int totalFlips;									// Number of spins about the x axis
	private int totalTurns;									// Number of spins about the y axis
	private int totalSpins;									// Number of spins about the z axis

	// Lists and sums of the expected and tracked statistic values
	private float[,,] ExpectedValues = new float[4,3,4];	// 3D matrix of expected stat values based on play throughs and logging of average stat data
	private float[,] statSum = new float[4,3];				// 2D matrix of the summed values for each interactable object
	private float[] Weights = new float[4];					// List of weights that are applied to each type of statistic
	private float[] TypeSums = new float[4];				// List of summed stats from each type overall
	private float[] LevelSums = new float[3];				// List of summed stats from each level overall
	private float TotalSum;									// Total sum of all stats from all objects
	
	private int qty;										// Counter for the average speed stat
	private float AvgSpeed;									// Average speed since startup
	public static int jumps;								// How many times a player jumps
	public static int deaths;								// How many times a player dies

	private _CarController carController;					// Reference to the _CarController script
	private GameObject player;								// Reference to the car GameObject
	private Rigidbody carRB;								// Reference to the rigidbody aspect of the car GameObject
	private bool lastFrameCancel = false;					// Used for debugging and printing data
	private bool StatTracking;								// Represents the value of (inObject || inAir) from the previous frame. This allows me to continuously track stat data while all three are true, then there is only one frame in which this is true and the others are false, allowing me to save the data and call other useful functions only once per object interaction
	public static bool objectTouched;						// When a player interacts with an object, this boolean is used to check whether the player is already in the middle of a trick from a previous object

	// These values are where we store the current amount of spin done by a player while they interact with an object
	private float Xspin;									
	private float Yspin;
	private float Zspin;

	// These values are where we store the current running stats while the player interacts with an object
	private int PostObjectAir;								// Frames spent in air during interaction
	private int PostObjectTricks;							// Number of tricks performed during interaction
	private int PostObjectPoints;							// Points gained during interaction
	public static int PostObjectSpeed;						// Speed reached at the moment the player exits the object's collider

	private Type currentType;						// Reference to the current type of object the player is interacting with
	private Level currentLevel;								// Reference to the current level of the type of the object the player is interacting with

	public static bool inObject;							// If the player's collider is inside an object's collider, this boolean is true
	public static int seed;									// Each play has a unique seed, used for determining the random number generation and used by me to save data to a unique file each play through
	public static string[] statList = new string[4];		// List of the names of each type of stat for debugging
	public Text PointDisplay;								// Reference to the UI element that displays the total number of points
	public JSON JSONClass;

	// Each script with public static variables requires that I use OnEnable, OnDisable, and OnSceneLoaded to change those variables each time a reload the game
	void OnEnable() {
		// <
		SceneManager.sceneLoaded += OnSceneLoaded;
	}

	void OnDisable() {
		// =
		SceneManager.sceneLoaded -= OnSceneLoaded;
	}

	private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
		// 3
		// Reinitialize a whole bunch of public static variables
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
	
		Ramp  = new Type(0, 100/4, "Ramp");
		Speed = new Type(1, 100/4, "Speed");
		Spike = new Type(2, 100/4, "Spike");
		Wall  = new Type(3, 100/4, "Wall");

		Types = new List<Type>();
		Types.Add(Ramp);
		Types.Add(Speed);
		Types.Add(Spike);
		Types.Add(Wall);
		for (int i = 0; i < Types.Count; i++){
			Types[i].Levels.Add(Types[i].L0);
			Types[i].Levels.Add(Types[i].L1);
			Types[i].Levels.Add(Types[i].L2);
		}
	}

	// Initialize a whole bunch of our variables and trackers and such
	void Start() {
		statList[0] = "Air";
		statList[1] = "Tricks";
		statList[2] = "Speed";
		statList[3] = "Points";

		Weights[0] = 0.4f;
		Weights[1] = 0.5f;
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
				TypeSums[type] 	+= sum;
				LevelSums[lev] 		+= sum;
			}
		}

		// Ramp expected values
		// RampL0
		ExpectedValues[0,0,0] = 65f;	// Air
		ExpectedValues[0,0,1] = 1f;   	// Tricks
		ExpectedValues[0,0,2] = 60f;	// Speed
		ExpectedValues[0,0,3] = 35f;	// Points
		// RampL1
		ExpectedValues[0,1,0] = 135f;	// Air
		ExpectedValues[0,1,1] = 3f;		// Tricks
		ExpectedValues[0,1,2] = 55f;	// Speed
		ExpectedValues[0,1,3] = 50f;	// Points
		// RampL2
		ExpectedValues[0,2,0] = 225f; 	// Air
		ExpectedValues[0,2,1] = 4f;  	// Tricks
		ExpectedValues[0,2,2] = 30f;  	// Speed
		ExpectedValues[0,2,3] = 80f; 	// Points
		// Speed expected values
		//SpeedL0
		ExpectedValues[1,0,0] = 1f; 	// Air
		ExpectedValues[1,0,1] = 1f;  	// Tricks
		ExpectedValues[1,0,2] = 80f;  	// Speed
		ExpectedValues[1,0,3] = 20f; 	// Points
		//SpeedL1
		ExpectedValues[1,1,0] = 75; 	// Air
		ExpectedValues[1,1,1] = 3f;  	// Tricks
		ExpectedValues[1,1,2] = 80f;  	// Speed
		ExpectedValues[1,1,3] = 25f; 	// Points
		//SpeedL2
		ExpectedValues[1,2,0] = 80; 	// Air
		ExpectedValues[1,2,1] = 4f;  	// Tricks
		ExpectedValues[1,2,2] = 80f;  	// Speed
		ExpectedValues[1,2,3] = 40f; 	// Points
		// Spike expected values
		//SpikedL0
		ExpectedValues[2,0,0] = 60f; 	// Air
		ExpectedValues[2,0,1] = 1f;  	// Tricks
		ExpectedValues[2,0,2] = 40f;  	// Speed
		ExpectedValues[2,0,3] = 30f; 	// Points
		//SpikeL1
		ExpectedValues[2,1,0] = 60f; 	// Air
		ExpectedValues[2,1,1] = 1f;  	// Tricks
		ExpectedValues[2,1,2] = 45f;  	// Speed
		ExpectedValues[2,1,3] = 30; 	// Points
		//SpikeL2
		ExpectedValues[2,2,0] = 60f; 	// Air
		ExpectedValues[2,2,1] = 1f;  	// Tricks
		ExpectedValues[2,2,2] = 50f;  	// Speed
		ExpectedValues[2,2,3] = 55f; 	// Points
		// Wall expected values
		//WallL0
		ExpectedValues[3,0,0] = 1f; 	// Air
		ExpectedValues[3,0,1] = 1f;  	// Tricks
		ExpectedValues[3,0,2] = 70f;  	// Speed
		ExpectedValues[3,0,3] = 25f; 	// Points
		//WallL1
		ExpectedValues[3,1,0] = 4f; 	// Air
		ExpectedValues[3,1,1] = 1f;  	// Tricks
		ExpectedValues[3,1,2] = 70f;  	// Speed
		ExpectedValues[3,1,3] = 45f; 	// Points
		//WallL2
		ExpectedValues[3,2,0] = 70f; 	// Air
		ExpectedValues[3,2,1] = 2f;  	// Tricks
		ExpectedValues[3,2,2] = 50f;  	// Speed
		ExpectedValues[3,2,3] = 65f; 	// Points

		print(seed);

		AddPointsController.Initialize();
	}

	void LateUpdate () {
		// Only track stats, update speed, or update points if the player is alive
		if (_CarController.Alive) {
			// TrackStats() is called every frame that the car is in the air or in an Object's collider and the first frame that the car touches back down
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
			// If the player dies, zero out all currently stored variables and trackers
			currentLevel = null;
			currentType = null;
			objectTouched = false;

			Xspin = 0;
			Yspin = 0;
			Zspin = 0;

			PostObjectSpeed = 0;
			PostObjectAir = 0;
			PostObjectTricks = 0;
			PostObjectPoints = 0;
		}
	}

	// Called once per frame to keep track of the average speed of the player throughout the game
	private void UpdateAverageSpeed(float newSpeed){
		++qty;
		AvgSpeed += (newSpeed - AvgSpeed)/qty;
	}

	// Used for debugging
	private void PrintStats(){
		print ("framesAtMax: " + framesAtMax.ToString());
		print ("framesBoosting: " + framesBoosting.ToString());
		print ("framesInAir: " + framesInAir.ToString());
		print ("framesDrifting: " + framesDrifting.ToString ());
		print ("framesOnBack: " + framesOnBack.ToString ());
	}

	// Used for debugging
	private void PrintPointStats(){
		print ("Ramp Points: " + Ramp.Points.ToString());
		print ("Spike Points: " + Spike.Points.ToString());
		print ("Speed Points: " + Speed.Points.ToString());
		print ("Wall Points: " + Wall.Points.ToString ());
		print ("Total Points: " + totalPoints.ToString ());
	}

	// Called whenever a player collides with an object's collider
	public void PlayerInteracted(int idTrack, int idLev){
		// Here we evaluate the ID sent from the interactable and increment the total number of the appropriate tracker
		Type type = Types[idTrack];
		Level level = type.Levels[idLev];

		level.numTotal++;

		// This is used to keep track of the first object touched after the player starts a trick
		if (!objectTouched) {
			currentType = type;
			currentLevel = level;
			objectTouched = true;

			Xspin = 0;
			Yspin = 0;
			Zspin = 0;

			PostObjectSpeed = 0;
			PostObjectAir = 0;
			PostObjectTricks = 0;
			PostObjectPoints = 0;
		}
	}

	// This function is used to update the stats the player gains whenever they leave the ground
	private void TrackStats(){
		if (_CarController.inAir || inObject) {
			// If the player is inside of an object or off of the ground, continue to track stats
			float X = Input.GetAxis ("Vertical");
			float Y = Input.GetAxis ("Horizontal");
			bool Z = (Input.GetAxis ("Spin") != 0);

			if (_CarController.inAir) PostObjectAir++;

			Xspin += X;
			if (!Z) Yspin += Y;
			else Zspin += Y;

		} else {
			int div;

			if(_CarController.Keyboard) div = 40;
			else div = 25;

			// This function is called each frame the player is in an object or in the air, it is also called the first frame that those are both false, thus this part of the function is only called once per interaction

			// This is where we tally up all of the stats that were collected while the player was doing tricks
			int flp = Mathf.Abs((int) (Xspin / div));
			int trn = Mathf.Abs((int) (Yspin / div));
			int spn = Mathf.Abs((int) (Zspin / div));

			totalFlips += flp;
			totalTurns += trn;
			totalSpins += spn;

			int addedPoints = ((12 * flp) + (10 * trn) + (15 * spn));
			PostObjectTricks = (flp + trn + spn);
			PostObjectSpeed = (int) carRB.velocity.magnitude;

			// If the player interacted with an object during their trick, we send the data to the Points added function to evaluate the new stats
			if (objectTouched) PointsAdded(addedPoints, PostObjectAir, PostObjectTricks, PostObjectSpeed);
			
			// The points gained from doing tricks is updated with the DisplayPoints function
			DisplayPoints(addedPoints);

			// Zero out all storage variables for the next trick
			Xspin = 0;
			Yspin = 0;
			Zspin = 0;

			PostObjectSpeed = 0;
			PostObjectAir = 0;
			PostObjectTricks = 0;
			PostObjectPoints = 0;
		}
	}

	// Used to update total points and change UI element that displays the number of total points
	public void DisplayPoints(int newPoints){
		if (newPoints > 0){
			if (PointDisplay != null) AddPointsController.CreateText(newPoints.ToString(), PointDisplay.transform);
			totalPoints += newPoints;
		}
	}

	// This function is used to evaluate the stats gained and assign them to the corrent type/level
	public void PointsAdded(int addedPoints, int air = 0, int trick = 0, int speed = 0){
		// This function is called when a player gets an orb and when they finish a trick so this is where we store the points gained
		PostObjectPoints += addedPoints;
		
		// If the function is called becasue we just finished a trick then we must evaluate which object to give the stats to
		if (!(_CarController.inAir || inObject)) {
			int preObjectPoints = currentType.getPoints(); // This is used to evaluate whether the player passes the threshold to pass the intro level difficulty
			currentLevel.Points += PostObjectPoints;

			// This is where we update our stats list and average stats list
			currentLevel.AverageStats(air, trick, speed, PostObjectPoints, Time.timeSinceLevelLoad);

			// If the player had less than 200 and now has more than 200 points, they have passed the threshold for the intro and can now begin the true game
			bool state1 = (preObjectPoints <= 200 && currentType.getPoints() >= 200);
			if (state1) {
				GenerateInfiniteFull.intro = false;
				carController.HorsePower = 2500f;
			}

			// Reset tracking variables
			StatTracking = false;
			objectTouched = false;

			// If the player has finished the intro requirements and they have been given the adaptive version of the game, apply the adaptive generation algorithm
			if (!GenerateInfiniteFull.intro) AIMonitorPlayer();

			currentLevel = null;
			currentType = null;
		}
	}

	// This is the function where we analyze the player's stats and tweak the preference values of each type/level accordingly
	private void AIMonitorPlayer(){
		// This algorithm only checks the stats for the object the player just interacted with, since that is the only one that would have new stat values
		float value = 0;
		// Value is the sum of the weighted comparisons of the current average stats and the expected values
		for (int stat = 0; stat < 4; stat++){
			value += ((currentLevel.avgStats[stat]/ExpectedValues[currentType.ID, currentLevel.ID, stat]) * Weights[stat]);
		}

		// Value is augmented by the number of times the player has interacted with an object
		// Since I believe this number is one of the best predicters of a player's preference for an object, it is weighted with a full 100%
		value += currentLevel.numTotal;
		
		// We then find get the difference between the current summed stat value in our 2D matrix and our new value and apply it to our different tracking lists (I do it this way to avoid having to go back and sum up each list from scratch)
		float diffValue = value - statSum[currentType.ID, currentLevel.ID];
		
		TotalSum += diffValue;
		TypeSums[currentType.ID] += diffValue;
		LevelSums[currentLevel.ID] += diffValue;

		// The new value is then assinged to the spot it belongs in the 2D stat matrix
		statSum[currentType.ID, currentLevel.ID] = value;
		
		// This is done by summing the total weighted stat values (plus number of objects interacted with) and finding the ratio of that to the total sum of all stats. The same is done for the levels of each type
		// Should I do bottom up? or maybe every object for itself??
		for (int type = 0; type < 4; type++){
			Types[type].Preference = (int) (100 * (TypeSums[type]/TotalSum));
			for (int lev = 0; lev < 3; lev++){
				Types[type].Levels[lev].Preference = (int) (100 * (statSum[type,lev]/LevelSums[lev]));
			}
		}

		// For printing information
		/*for (int stat = 0; stat < 4; stat++){
			string s = string.Format("L{2}:{0} {1}", statList[stat], currentLevel.avgStats[stat], currentLevel.ID);
			print(s);
		}*/
	}
}