using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// Class for each block of road to be generated
class Tile{
	public GameObject gameTile;
	public float creationTime;

	// Instatiation
	public Tile(GameObject tile, float ctime){
		gameTile = tile;
		creationTime = ctime;
	}
}

// Class for spawning in each game object
class Interact{
	public GameObject interactable;
	public float creationTime;
	public int idLev;
	public int idTrack;

	// Instatiation
	public Interact(GameObject i, float ctime, int lev, int track){
		interactable = i;
		creationTime = ctime;
		idLev = lev;
		idTrack = track;
	}
}

public class GenerateInfiniteFull: MonoBehaviour {
	public static bool intro = true; // To give players some time to get used to mechanics before random/adaptive spawning starts
	private bool adapt; // Boolean that decides whether player gets adaptive or random spawning
	public List<GameObject> ObjectList = new List<GameObject>(); // List of all prefab objects that can be spawned
	private int[] TopList = new int[4]; // List of cumulative preference values for each type
	private int[] BottomList = new int[3]; // List of cumulative preference values for each level

	private GameObject player; // To track the position of the player for spawning around them
	private MasterController Master; // Holding the MasterController to later pass it to each interactable after spawning

	public static bool Restart; // If the player hits a spike, they will get placed back at the beginning

	int groundPlaneSize = 50; // The size of each tile
	int tilesInFront = 10; // Number of tiles spawned in front of player
	int tilesBehind = -3; // Number of tiles spawned behind player
	int intCount = 0; // Used to only spawn interactables every 3 tiles

	Vector3 startPos; // Start position each time the player passes the tile size threshold

	Hashtable tileMatrix = new Hashtable(); // Hashtable of every tile and its creation time
	Hashtable interactableMatrix = new Hashtable(); // Hashtable of every object and its creation time

	// Each script with public static variables requires that I use OnEnable, OnDisable, and OnSceneLoaded to change those variables each time a reload the game
	void OnEnable() {
		SceneManager.sceneLoaded += OnSceneLoaded;
	}

	void OnDisable() {
		SceneManager.sceneLoaded -= OnSceneLoaded;
	}

	private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
		intro = true;
		Restart = false;
		if (Random.Range(0,2) == 1) adapt = true;
		if (adapt) print(adapt);
	}

	// Before anything else is started I must create a new, unique seed for each player.
	void Awake(){
		MasterController.seed = Mathf.Abs(System.Environment.TickCount);
		Random.InitState(MasterController.seed);
	}

	// Get references to player and to the MasterController, then start the player in the middle of 14 empty tiles
	void Start(){
		player = GameObject.FindWithTag("Player");
		Master = GetComponent<MasterController> ();

		StartFunction();
	}

	// Each update this function is called to check the position of the player and change the environment accordingly
	// This script is not responsible for any 'smart' adaption, only for spawning based on calculations made by other scripts
	void FixedUpdate(){
		// If the player hits a spike, destroy all tiles and game objects and start over
		if (Restart){
			foreach (Tile tls in tileMatrix.Values) {
				Destroy (tls.gameTile);
			}
			foreach (Interact fun in interactableMatrix.Values) {
				Destroy (fun.interactable);
			}

			tileMatrix = new Hashtable();
			interactableMatrix = new Hashtable();
			StartFunction();
			Restart = false;

		} else {
			// Check if the player has moved a tile size distance
			int zMove = (int)(player.transform.position.z - startPos.z);
			if (Mathf.Abs (zMove) >= groundPlaneSize) {
				float updateTime = Time.realtimeSinceStartup;

				// Loop through every tile space that should exist for the new position and check if this tile already exists
				// If so, update the time identity
				// If not, create the new tile and add it to the matrix
				for (int z = tilesBehind; z <= tilesInFront; z++) {
					// This will be our tile and object for each slot
					GameObject t;
					GameObject interact;

					// For each tile - get the position by multiplying the iteration number by the plane size and the start position of the player (0)
					float tileLocation = (z * groundPlaneSize + startPos.z);
					float randomLocation = Random.Range (-20f, 20f);

					// Create unique names for each object based on their position, this is used for debugging and hashtable logging
					Vector3 tilePos = new Vector3 (0, 0, tileLocation);
					string tilename = "Tile_" + ((int)(tilePos.z)).ToString();
					string interactName = "Inter_" + ((int)(tilePos.z)).ToString();

					// If the game is still in intro-mode, coinflip is true every third tile
					// else, coinflip is true with 1/3 chance
					// coinFlip is used to determine whether an object should be spawned on a tile
					bool coinFlip;
					if (intro) coinFlip = (intCount == 1);
					else coinFlip = (Random.Range(0, 3) != 1);

					// Only spawn objects if we are looking at the ninth tile ahead of the player
					bool ninthTile = (z == 9);

					// If the coinFlip is true, we are on the ninth tile, and the spawned object is not already in the hashtable, then we can spawn a new object
					bool spawnNewFunObj = (!interactableMatrix.ContainsKey (interactName) && ninthTile && coinFlip);

					if (spawnNewFunObj) {
						// Should we spawn an object or just a spike strip?
						bool SpikeOrFun = (Random.Range(0, 5) != 1);

						GameObject funObject;
						int identifyTrack;
						int identifyLev;
						// To reduce computational draw, setting the position is done only if the coin flip works
						if (SpikeOrFun){
							// call function that takes data from Master and decides which item should be generated
							decideFunObj (out funObject, out identifyTrack, out identifyLev);
						} else {
							funObject = ObjectList[1];
							identifyTrack = 0;
							identifyLev = 0;
						}

						Vector3 interactPos = new Vector3 (randomLocation, 1, tileLocation + randomLocation);
						interact = (GameObject)Instantiate (funObject, interactPos, Quaternion.identity);

						// Set the name of our interactable object [mostly for debugging]
						interact.name = interactName;

						// Set the id of our object based on what type and level it is [for communication with the master]
						InteractController control = interact.GetComponent<InteractController>();
						control.setID (identifyTrack, identifyLev);
						control.setMaster (Master);

						// Create an Interact class object and add it to our 
						Interact funObj = new Interact (interact, updateTime, identifyLev, identifyTrack);
						interactableMatrix.Add (interactName, funObj);
					} else if (interactableMatrix.ContainsKey (interactName)) {
						(interactableMatrix [interactName] as Interact).creationTime = updateTime;
					}

					intCount = (intCount + 1) % 3;
					
					if (!tileMatrix.ContainsKey (tilename)) {
						t = (GameObject) Instantiate(ObjectList[0], tilePos, Quaternion.identity);

						t.name = tilename;
						Tile tile = new Tile (t, updateTime);
						tileMatrix.Add (tilename, tile);
					} else {
						(tileMatrix [tilename] as Tile).creationTime = updateTime;
					}
						
				}

				// Because of the way that hashtables work in unity, we must create a new hashtable each time
				// This is because hash tables cannot delete values, they would simply keep the values forever
				Hashtable newRoad = new Hashtable ();
				foreach (Tile tls in tileMatrix.Values) {
					// Check if the the time identity is correct
					// if not, destroy it
					// if so, add it to the new hashtable
					if (tls.creationTime != updateTime) {
						Destroy (tls.gameTile);
					} else {
						newRoad.Add (tls.gameTile.name, tls);
					}
				}

				Hashtable newFunObjs = new Hashtable ();
				foreach (Interact fun in interactableMatrix.Values) {
					// Check if the the time identity is correct
					// if not, destroy it
					// if so, add it to the new hashtable
					if (fun.creationTime != updateTime) {
						Destroy (fun.interactable);
					} else {
						newFunObjs.Add (fun.interactable.name, fun);
					}
				}

				// Finally, assign the old matrix to the new matrix
				tileMatrix = newRoad;
				interactableMatrix = newFunObjs;

				if (zMove >= groundPlaneSize)
					startPos.z += groundPlaneSize;
				else
					startPos.z -= groundPlaneSize;
			}
		}
	}

	// Function for deciding which object should be spawned based on 'Preference' values calculated by the MasterController
	//:::Comment this function!
	private void decideFunObj(out GameObject funInteract, out int funIDTrack, out int funID){
		if(intro){
			funID = 0;
			funIDTrack = Random.Range(0, 4);
		} else {
			if (adapt){
				int topLevelRNG = Random.Range(0, TopList[3]);
				int bottomLevelRNG = Random.Range(0, BottomList[2]);
				
				int IDLev = 0;
				int IDTrack = 0;
				bool topPicked = false;
				bool bottomPicked = false;

				TopList[0] = MasterController.Ramp.Preference;

				for (int type = 0; type < 4; type++){
					string s = string.Format ("{0} Preference: {1}, Sum: {2}", MasterController.Trackers[type].name, MasterController.Trackers[type].Preference, TopList[type]);
					print(s);

					if (type > 0) TopList[type] = TopList [type - 1] + MasterController.Trackers[type].Preference;
					if((topLevelRNG - TopList[type]) <= 0 && !topPicked) {
						IDTrack = type;
						topPicked = true;
						BottomList[0] = MasterController.Trackers[type].L0.Preference;

						for (int lev = 0; lev < 3; lev++){
							if (lev > 0) BottomList[lev] = BottomList[lev - 1] + MasterController.Trackers[type].Levels[lev].Preference;
							if((bottomLevelRNG - BottomList[lev]) <= 0 && !bottomPicked) {
								IDLev = lev;
								bottomPicked = true;
							}
						}
					}
				}

				funIDTrack = IDTrack;
				funID = IDLev;
			} else {
				funID = Random.Range(0, 3);
				funIDTrack = Random.Range(0, 4);
			}
		}

		funInteract = ObjectList[2 + funID + (funIDTrack * 3)];
	}

	// Function used at start and when player dies
	void StartFunction(){
		// Set generator position to zero
		transform.position = Vector3.zero;
		startPos = Vector3.zero;

		// Get the time so that we can name the tils
		float updateTime = Time.realtimeSinceStartup;

		// Loop from -tiles to tiles to create a grid centered on the player
		for (int z = tilesBehind; z <= tilesInFront; z++){
			// This will be our tile for each slot
			GameObject t;

			// Tile location
			float tileLocation = (z * groundPlaneSize + startPos.z);

			Vector3 tilePos = new Vector3(0, 0, tileLocation);

			t = (GameObject) Instantiate(ObjectList[0], tilePos, Quaternion.identity);

			// This name scheme is mostly for debugging in the future when tiles have ramps on them
			string tilename = "Tile_" + ((int)(tilePos.z)).ToString();
			t.name = tilename;



			// Create tile and add it to the hashtable
			Tile tile = new Tile(t, updateTime);
			tileMatrix.Add(tilename, tile);
		}
	}
}