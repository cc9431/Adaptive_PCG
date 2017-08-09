using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

class Interact{
	public GameObject interactable;
	public float creationTime;
	public string id;

	// Instatiation
	public Interact(GameObject i, float ctime, string identify){
		interactable = i;
		creationTime = ctime;
		id = identify;
	}
}

public class GenerateInfiniteFull: MonoBehaviour {
	public GameObject groundPlane;
	public GameObject groundSpike;

	public GameObject Interact_RampL0;
	public GameObject Interact_RampL1;
	public GameObject Interact_RampL2;

	public GameObject Interact_SpikesL0;
	public GameObject Interact_SpikesL1;
	public GameObject Interact_SpikesL2;

	public GameObject Interact_SpeedL0;
	public GameObject Interact_SpeedL1;
	public GameObject Interact_SpeedL2;

	public GameObject Interact_WallL0;
	public GameObject Interact_WallL1;
	public GameObject Interact_WallL2;

	private GameObject player;
	private MasterController Master;

	public static bool Restart;

	private string rampJump = "R";
	private string speedPortal = "S";
	private string spikeStrip = "K";
	private string wallDestroy = "W";

	private string level0 = "0";
	private string level1 = "1";
	private string level2 = "2";

	int groundPlaneSize = 10 * 5;
	int tilesInFront = 10;
	int tilesBehind = -3;

	Vector3 startPos;

	// Hashtable of every tile and its creation time
	Hashtable tileMatrix = new Hashtable();
	Hashtable interactableMatrix = new Hashtable();

	void Awake(){
		//MasterController.seed = Mathf.Abs(System.Environment.TickCount);
		MasterController.seed = 5;
		Random.InitState(MasterController.seed);
	}

	void Start(){
		// Get player from tag
		player = GameObject.FindWithTag("Player");
		Master = GetComponent<MasterController> ();

		StartFunction();
	}

	void FixedUpdate(){
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
			// This is where we check the position of the player and update our hashtable based on that
			int zMove = (int)(player.transform.position.z - startPos.z);

			// Check if the player has moved a tile size distance
			if (Mathf.Abs (zMove) >= groundPlaneSize) {
				float updateTime = Time.realtimeSinceStartup;

				// Loop through every tile space that should exist for the new position and check if this tile already exists
				// If so, update the time identity
				// If not, create the new tile and add it to the matrix
				for (int z = tilesBehind; z <= tilesInFront; z++) {
					// This will be our tile for each slot
					GameObject t;
					GameObject interact;

					// For each tile - get the position by multiplying the iteration number by the plane size and the start position of the player (0)
					float tileLocation = (z * groundPlaneSize + startPos.z);
					float randomLocation = Random.Range (-20f, 20f);

					Vector3 tilePos = new Vector3 (0, 0, tileLocation);
					string tilename = "Tile_" + ((int)(tilePos.z)).ToString();
					string interactName = "Inter_" + ((int)(tilePos.z)).ToString();

					bool coinFlip = Random.Range(0, 4) != 1;
					bool ninthTile = (z == 9);
					bool spawnNewFunObj = (!interactableMatrix.ContainsKey (interactName) && ninthTile && coinFlip);

					if (spawnNewFunObj) {
						bool SpikeOrFun = (Random.Range(0, 5) != 1);

						GameObject funObject;
						string identify;
						// To reduce computational draw, setting the position is done only if the coin flip works
						if (SpikeOrFun){
							// call function that takes data from Master and decides which item should be generated
							decideFunObj (out funObject, out identify);
						} else {
							funObject = groundSpike;
							identify = level0 + level0;
						}

						Vector3 interactPos = new Vector3 (randomLocation, 1, tileLocation + randomLocation);
						interact = (GameObject)Instantiate (funObject, interactPos, Quaternion.identity);

						// Set the name of our interactable object [mostly for debugging]
						interact.name = interactName;

						// Set the id of our object based on what type and level it is [for communication with the master]
						InteractController control = interact.GetComponent<InteractController>();
						control.setID (identify);
						control.setMaster (Master);

						// Create an Interact class object and add it to our 
						Interact funObj = new Interact (interact, updateTime, identify);
						interactableMatrix.Add (interactName, funObj);
					} else if (interactableMatrix.ContainsKey (interactName)) {
						(interactableMatrix [interactName] as Interact).creationTime = updateTime;
					}
					
					if (!tileMatrix.ContainsKey (tilename)) {
						t = (GameObject) Instantiate(groundPlane, tilePos, Quaternion.identity);

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

	private void decideFunObj(out GameObject funInteract, out string funID){
		int topLevelRNG = Random.Range(0, 100);
		int bottomLevelRNG = Random.Range(0, 33);
		
		bool R = (topLevelRNG >= 0 && topLevelRNG < MasterController.Ramp.Preference);
		bool S = (topLevelRNG >= MasterController.Ramp.Preference && topLevelRNG < MasterController.Speed.Preference);
		bool K = (topLevelRNG >= MasterController.Speed.Preference && topLevelRNG < MasterController.Spike.Preference);

		bool RL0 = (bottomLevelRNG < MasterController.Ramp.L0.preference);
		bool RL1 = (!RL0 && bottomLevelRNG < MasterController.Ramp.L1.preference);

		bool SL0 = (bottomLevelRNG < MasterController.Speed.L0.preference);
		bool SL1 = (!SL0 && bottomLevelRNG < MasterController.Speed.L1.preference);
		
		bool KL0 = (bottomLevelRNG < MasterController.Spike.L0.preference);
		bool KL1 = (!KL0 && bottomLevelRNG < MasterController.Spike.L1.preference);
		
		bool WL0 = (bottomLevelRNG < MasterController.Wall.L0.preference);
		bool WL1 = (!WL0 && bottomLevelRNG < MasterController.Wall.L1.preference);

		if (R) {
			funID = rampJump;
			if (RL0) {
				funInteract = Interact_RampL0;
				funID += level0;
			} else if (RL1) {
				funInteract = Interact_RampL1;
				funID += level1;
			} else {
				funInteract = Interact_RampL2;
				funID += level2;
			}
		} else if (S) {
			funID = speedPortal;
			if (SL0) {
				funInteract = Interact_SpeedL0;
				funID += level0;
			} else if (SL1) {
				funInteract = Interact_SpeedL1;
				funID += level1;
			} else {
				funInteract = Interact_SpeedL2;
				funID += level2;
			}
		} else if (K) {
			funID = spikeStrip;
			if (KL0) {
				funInteract = Interact_SpikesL0;
				funID += level0;
			} else if (KL1) {
				funInteract = Interact_SpikesL1;
				funID += level1;
			} else {
				funInteract = Interact_SpikesL2;
				funID += level2;
			}
		} else {
			funID = wallDestroy;
			if (WL0) {
				funInteract = Interact_WallL0;
				funID += level0;
			} else if (WL1) {
				funInteract = Interact_WallL1;
				funID += level1;
			} else {
				funInteract = Interact_WallL2;
				funID += level2;
			}
		}
	}

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

			t = (GameObject) Instantiate(groundPlane, tilePos, Quaternion.identity);

			// This name scheme is mostly for debugging in the future when tiles have ramps on them
			string tilename = "Tile_" + ((int)(tilePos.z)).ToString();
			t.name = tilename;



			// Create tile and add it to the hashtable
			Tile tile = new Tile(t, updateTime);
			tileMatrix.Add(tilename, tile);
		}
	}
}