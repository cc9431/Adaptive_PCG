using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTriggerController : MonoBehaviour {
	void OnTriggerEnter(Collider other){
		if (other.tag == "Ground") _CarController.bodyTouching = true;
		if (other.tag == "Spike") _CarController.Alive = false;
	}

	void OnTriggerExit(Collider other){
		_CarController.bodyTouching = false;
	}
}
