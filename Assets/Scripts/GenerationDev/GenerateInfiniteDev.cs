/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateInfinite : MonoBehaviour {
	public GameObject player;

	int groundPlaneSize = 10;
	int tilesZ = 10;
	int tilesBehind = -3;

	Vector3 startPos;

	// Hashtable of every tile and its creation time
	Hashtable tileMatrix = new Hashtable();

	void Start(){
		// Set player and generator positions to zero
		this.gameObject.transform.position = Vector3.zero;
		startPos = Vector3.zero;

		// Get the time so that we can name the tils
		float updateTime = Time.realtimeSinceStartup;

		// Loop from -tiles to tiles to create a grid centered on the player
		for (int z = tilesBehind; z <= tilesZ; z++){
			// For each tile - get the position by multiplying the iteration number by the plane size and the start position of the player (0)
			// Then create the tile
			Vector3 posZ = new Vector3(0, 0, (z * groundPlaneSize + startPos.z));

			// This name scheme is mostly for debugging in the future
			string tilename = "Tile_" + ((int)(posZ.z)).ToString();

			// Create tile
			Tile tile = Tile(tilename, updateTime, posZ);

			// Add it to the hashtable
			tileMatrix.Add(tilename, tile);
		}
	}

	void FixedUpdate(){
		// This is where we check the position of the player and update our hashtable based on that
		int zMove = (int)(player.transform.position.z - startPos.z);

		// Check if the player has moved a tile size distance
		if (Mathf.Abs (zMove) >= groundPlaneSize) {
			float updateTime = Time.realtimeSinceStartup;

			int playerZ = (int)(Mathf.Floor(player.transform.position.z/groundPlaneSize) * groundPlaneSize);
			// Loop through every tile space that should exist for the new position and check if this tile already exists
			// If so, update the time identity
			// If not, create the new tile and add it to the matrix
			for (int z = tilesBehind; z <= tilesZ; z++) {
				// This will be our tile for each slot
				GameObject t;

				// For each tile - get the position by multiplying the iteration number by the plane size and the start position of the player (0)
				// Then create the tile
				Vector3 posZ = new Vector3 (0, 0, (z * groundPlaneSize + playerZ));

				// Check every 10 tiles to insert a ramp
				bool rampTile = (posZ.z%100 == 0);
				int coinFlip = Random.Range(0, 2);

				string tilename = "Tile_" + ((int)(posZ.z)).ToString();
				
				if (!tileMatrix.ContainsKey (tilename)) {
					// If the ramp wins, place a ramp tile else normal tile
					if (rampTile && coinFlip == 0) t = (GameObject) Instantiate(bigRampPlane, posZ, Quaternion.identity);
					else if (rampTile && coinFlip == 1) t = (GameObject) Instantiate(rampPlane, posZ, Quaternion.identity);
					else t = (GameObject) Instantiate(groundPlane, posZ, Quaternion.identity);

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

			// Finally, assign the old matrix to the new matrix
			tileMatrix = newRoad;

			startPos = player.transform.position;
		}
	}
}*/