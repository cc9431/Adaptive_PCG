using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainMover : MonoBehaviour {

	public Transform player;
	
	void LateUpdate () {
		Vector3 newPos = new Vector3(-900, -0.1f, player.position.z - 900);

		transform.position = newPos;
	}
}
