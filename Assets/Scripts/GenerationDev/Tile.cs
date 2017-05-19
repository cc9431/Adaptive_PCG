/*using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Class for each block of road to be generated
public class Tile : MonoBehaviour{
	public GameObject groundPlane;
	public GameObject rampPlane;
	public GameObject bigRampPlane;
	private GameObject gameTile;
	private float creationTime;

	// Instatiation
	public Tile(string tilename, float ctime, Vector3 posZ){
		GameObject t;

		// Check every 100 tiles to insert a ramp
		bool rampTile = (posZ.z%100 == 50);
		float coinFlip = Random.Range(0, 2);

		// If the ramp wins, place a ramp tile else normal tile
		if (rampTile && coinFlip == 0) t = (GameObject) Instantiate(rampPlane, posZ, Quaternion.identity);
		else if (rampTile && coinFlip == 1) t = (GameObject) Instantiate(bigRampPlane, posZ, Quaternion.identity);
		else t = (GameObject) Instantiate(groundPlane, posZ, Quaternion.identity);

		gameTile = t;
		gameTile.name = tilename;
		creationTime = ctime;
	}
}*/