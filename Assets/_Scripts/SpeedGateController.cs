using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedGateController : MonoBehaviour {
	private _CarController car;
	private bool touched = false;
	public float speedTime;

	void Awake(){
		car = GameObject.FindGameObjectWithTag ("Player").GetComponent<_CarController>();
	}

	void OnTriggerEnter(Collider other){
		if (other.tag == "Boost") {
			if (!touched){
				car.speedGate += speedTime;
				touched = true;
			}
		}
	}
}
