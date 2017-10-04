using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetCube : MonoBehaviour {
	Vector3 startPos = new Vector3();
	public Rigidbody Car;
	
	void Start () {
		startPos = Car.transform.position;
	}
	
	// Update is called once per frame
	void OnTriggerExit(Collider other){
		if (other.CompareTag("Player")) {
			Car.transform.position = startPos;
			Car.velocity = Vector3.zero;
			Car.rotation = Quaternion.identity;
		}
	}
}
