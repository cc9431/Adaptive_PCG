using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

static class ListQueueStackExtensions {
	public static T PeekQ<T>(this List<T> list){
		return list[0];
	}

	public static T PeekS<T>(this List<T> list){
		int last = list.Count - 1;
		return list[last];
	}

	public static void Enqueue<T>(this List<T> list, T item){
		list.Add(item);
	}

    public static T Dequeue<T>(this List<T> list){
        T item = list[0];
        list.RemoveAt(0);
        return item;
    }

	public static void Push<T>(this List<T> list, T item){
		list.Insert(0, item);
	}

	public static T Pop<T>(this List<T> list){
		int last = list.Count - 1;
		T item = list[last];
		list.RemoveAt(last);
		return item;
	}
}

public class GenerateInfiniteFull: MonoBehaviour {
	public static bool intro = true; // To give players some time to get used to mechanics before random/adaptive spawning starts
	public static bool adapt; // Boolean that decides whether player gets adaptive or random spawning
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
	List<GameObject> tilePool = new List<GameObject>(13); // Pool of every tile in the game
	List<GameObject> interactablePool = new List<GameObject>(13); // Pool of every interactable object in the game

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
		if (adapt) print("adapt");
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
			GameObject tile;
			GameObject inter;
			for (int obcts = 0; obcts < 13; obcts++){
				tile = tilePool.Dequeue();
				Destroy(tile);
				if (interactablePool.Count > 0) {
					inter = interactablePool.Dequeue();
					Destroy(inter);
				}
			}
			
			StartFunction();
			Restart = false;

		} else {
			// Check if the player has moved a tile size distance
			int zMove = (int)(player.transform.position.z - startPos.z);
			
			if (Mathf.Abs (zMove) >= groundPlaneSize) {
				GameObject funObject;
				Vector3 tilePos = startPos;
				bool actLikeQueue = (zMove >= groundPlaneSize);
				bool coinFlip;								// If the game is still in intro-mode, coinflip is true every third tile
				if (intro) {
					coinFlip = (intCount == 1);		// else, coinflip is true with 1/3 chance
					intCount = (intCount + 1) % 3;
				}
				else coinFlip = (Random.Range(0, 3) != 1);	// coinFlip is used to determine whether an object should be spawned on a tile

				if (actLikeQueue){
					if (interactablePool.Count > 0){
						if ((interactablePool.PeekQ().transform.position - tilePos).z < tilesBehind * groundPlaneSize){
							funObject = interactablePool.Dequeue();
							DestroyImmediate(funObject);
						}
					}
					tilePos.z += groundPlaneSize * (tilesInFront + 1);
					startPos.z += groundPlaneSize;
				} else {
					coinFlip = actLikeQueue;
					if (interactablePool.Count > 0){
						if ((interactablePool.PeekS().transform.position - tilePos).z > tilesInFront * groundPlaneSize){
							funObject = interactablePool.Pop();
							DestroyImmediate(funObject);
						}
					}
					tilePos.z += groundPlaneSize * (tilesBehind - 1);
					startPos.z -= groundPlaneSize;
				}

				string tilename = "Tile_" + ((int)(tilePos.z)).ToString();
				string interactName = "Inter_" + ((int)(tilePos.z)).ToString();
				recyclePoolWater(actLikeQueue, tilePool, tilePos, tilename);
				if (coinFlip) flippedCoin(interactName, tilePos);
			}
		}
	}

	// Function used at start and when player dies
	void StartFunction(){
		// Set generator position to zero
		transform.position = Vector3.zero;
		startPos = Vector3.zero;

		// Get the time so that we can name the tiles
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

			// Enqueue tile to pool
			tilePool.Enqueue(t);
		}
	}

	void flippedCoin(string interactName, Vector3 tilePos){
		// Should we spawn an object or just a spike strip?
		GameObject funObject;
		GameObject interact;
		bool noSpike = (Random.Range(0, 5) != 1);
		float randomLocationX = Random.Range (-20f, 20f);
		float randomLocationZ = Random.Range (-20f, 20f);

		int identifyTrack;
		int identifyLev;

		if (noSpike){
			// Call function that takes data from Master and decides which item should be generated
			decideFunObj (out funObject, out identifyTrack, out identifyLev);
		} else {
			funObject = ObjectList[1];
			identifyTrack = -1;
			identifyLev = -1;
		}

		Vector3 interactPos = new Vector3 (randomLocationX, 1, tilePos.z + randomLocationZ);
		interact = (GameObject)Instantiate (funObject, interactPos, Quaternion.identity);

		// Set the name of our interactable object [mostly for debugging]
		interact.name = interactName;

		// Set the id of our object based on what type and level it is [for communication with the master]
		InteractController control = interact.GetComponent<InteractController>();
		control.setID (identifyTrack, identifyLev);
		control.setMaster (Master);

		interactablePool.Enqueue(interact);
	}

	// Function for deciding which object should be spawned based on 'Preference' values calculated by the MasterController
	private void decideFunObj(out GameObject funInteract, out int funIDTrack, out int funIDLev){
		if(intro){
			// If the player has not yet made 200 points for a single type, then only generate level 0 items with no adaptivity
			funIDLev = 0;
			funIDTrack = Random.Range(0, 4);
		} else {
			// There is a 1/2 chance everytime the player starts that the game will be adaptive or not
			if (adapt){
				// If the game is adaptive, roll a number between zero and the largest number in the cumulative preference list for both top level and bottom level
				int topLevelRNG = Random.Range(0, TopList[3]);
				int bottomLevelRNG = Random.Range(0, BottomList[2]);
				
				// Initialize variables
				// IDLev and IDTrack are standing in for funIDTrack and funIDLev because of the way 'out' works (The funIDTrack and funIDLev variables must be assigned before the function ends and are also treated as final variables)
				int IDLev = 0; 
				int IDTrack = 0;
				bool topPicked = false;
				bool bottomPicked = false;

				// The upcoming loop both assigns and uses the new preference values for the type of object
				TopList[0] = MasterController.Ramp.Preference;
				for (int type = 0; type < 4; type++){
					// This is where we sum up the preference values for the next iteration of the loop
					if (type > 0) TopList[type] = TopList [type - 1] + MasterController.Types[type].Preference;
					if((topLevelRNG - TopList[type]) <= 0 && !topPicked) {
						// The type is decided from the first preference number that is larger than the randomly generated number
						IDTrack = type;
						topPicked = true;
						// The upcoming loop both assigns and uses the new preference values for the level of object
						BottomList[0] = MasterController.Types[type].L0.Preference;
						for (int lev = 0; lev < 3; lev++){
							//This is where we sum up the preference values for the next iteration of the loop
							if (lev > 0) BottomList[lev] = BottomList[lev - 1] + MasterController.Types[type].Levels[lev].Preference;
							if((bottomLevelRNG - BottomList[lev]) <= 0 && !bottomPicked) {
								// The level is decided from the first preference number that is larger than the randomly generated number
								IDLev = lev;
								bottomPicked = true;
							}
						}
					}
					// For debugging
					//string s = string.Format ("{0} Preference: {1}, Sum: {2}", MasterController.Types[type].name, MasterController.Types[type].Preference, TopList[type]);
					//print(s);
				}

				// Finally, the out variables are assinged from the values found in the  
				funIDTrack = IDTrack;
				funIDLev = IDLev;
			} else {
				// If the game is not adaptive, the levels and types are chosen completely randomly
				funIDLev = Random.Range(0, 3);
				funIDTrack = Random.Range(0, 4);
			}
		}

		// Finally, assign the object a object model based on the ID numbers assigned (there are two objects in the beginning that must be skipped over).
		funInteract = ObjectList[2 + funIDLev + (funIDTrack * 3)];
	}

	void recyclePoolWater(bool actLikeQueue, List<GameObject> list, Vector3 newPosition, string newName){
		if (actLikeQueue){
			GameObject frontTile = list.Dequeue();
			frontTile.transform.position = newPosition;
			frontTile.name = newName;
			tilePool.Enqueue(frontTile);
		} else {
			GameObject frontTile = list.Pop();
			frontTile.transform.position = newPosition;
			frontTile.name = newName;
			tilePool.Push(frontTile);
		}
	}
}