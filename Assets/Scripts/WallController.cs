using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallController : MonoBehaviour {
	public GameObject cubeParent;
	private GameObject[] Cubes;
	private bool killCubes;
	private int countDown;

	// Use this for initialization
	void Start () {
		killCubes = false;
		countDown = 100;
		Cubes = cubeParent.GetComponentsInChildren<GameObject> ();
	}

	void Update(){
		if (killCubes && countDown > 0)
			countDown--;

		if (countDown == 0)
			DestroyCubes ();
	}
	
	void OnTriggerExit(Collider other){
		if (other.tag == "PlayerTrigger")
			killCubes = true;
	}

	private void DestroyCubes(){
		for (int i = 0; i < Cubes.Length; i++) {
			Destroy (Cubes [i]);
		}
	}
}
