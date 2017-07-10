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

public class GenerateInfinite : MonoBehaviour {
	public GameObject groundPlane;
	public GameObject Interact_Ramp;
	private GameObject player;

	private string rampJump = "R";
	private string speedStrip = "S";
	private string spikeStrip = "K";

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

	void Start(){
		// Get player from tag
		player = GameObject.FindWithTag("Player");

		// Set player and generator positions to zero
		this.gameObject.transform.position = Vector3.zero;
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

	void FixedUpdate(){

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
				Quaternion quat = Quaternion.Euler (-90, 0, -90);

				Vector3 tilePos = new Vector3 (0, 0, tileLocation);
				string tilename = "Tile_" + ((int)(tilePos.z)).ToString();
				string interactName = "Inter_" + ((int)(tilePos.z)).ToString();

				// TODO When the MasterController starts to make decisions about the generation here will be where I need
				// to set the ID based on what is chosed. I should also have a way of instantiating the object simply,
				// without having a bunch of if statements.
				string identify = rampJump + level0;

				if (!interactableMatrix.ContainsKey (interactName)) {
					// 10% chance to create ramp
					bool coinFlip = Random.Range(0, 4) == 1 && (z == 9);

					// If the ramp wins, place a ramp and add to 
					if (coinFlip) {
						// To reduce computational draw, setting the position is done only if the coin flip works
						Vector3 interactPos = new Vector3 (randomLocation, 1, tileLocation + randomLocation);
						interact = (GameObject)Instantiate (Interact_Ramp, interactPos, quat);

						// Set the name of our interactable object [mostly for debugging]
						interact.name = interactName;

						// Set the id of our object based on what type and level it is [for communication with the master]
						InteractController inter = interact.GetComponent<InteractController> ();
						inter.setID (identify);

						// Create an Interact objecta and add it to our 
						Interact funObj = new Interact (interact, updateTime, identify);
						interactableMatrix.Add (interactName, funObj);
					}
				} else {
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