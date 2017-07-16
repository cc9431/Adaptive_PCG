using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTriggerController : MonoBehaviour {
	private _CarController car;

	void Awake (){
		car = gameObject.GetComponentInParent<_CarController> ();
	}

	void OnTriggerEnter(Collider other){
		if (other.tag == "Ground") car.bodyTouching = true;
	}

	void OnTriggerExit(Collider other){
		car.bodyTouching = false;
	}
}
