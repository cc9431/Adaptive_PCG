using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallGravity : MonoBehaviour {

	void OnTriggerEnter(Collider other){
		if (other.tag == "Player"){
			Rigidbody[] boxes = GetComponentsInChildren<Rigidbody>();
			foreach (Rigidbody box in boxes){
				box.useGravity = true;
			}
		}
	}
}
