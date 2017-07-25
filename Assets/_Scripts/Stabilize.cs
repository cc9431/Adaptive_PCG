using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// This script takes a realistic feature of cars that is meant to stabilize
// and use it to the extreme to make sure that the car will never tip over

public class Stabilize : MonoBehaviour{ 
    public WheelCollider WheelL;
    public WheelCollider WheelR;
    public float AntiRoll;
	private _CarController car;

	void Start() {
		car = GetComponentInParent<_CarController> ();
	}
 
    public void FixedUpdate(){
		if (car.Alive) {
			WheelHit hit;
			float travelL = 1.0f;
			float travelR = 1.0f;

			bool groundedL = WheelL.GetGroundHit (out hit);
			if (groundedL)
				travelL = (-WheelL.transform.InverseTransformPoint (hit.point).y - WheelL.radius) / WheelL.suspensionDistance;

			bool groundedR = WheelR.GetGroundHit (out hit);
			if (groundedR)
				travelR = (-WheelR.transform.InverseTransformPoint (hit.point).y - WheelR.radius) / WheelR.suspensionDistance;

			float antiRollForce = (travelL - travelR) * AntiRoll;

			if (groundedL)
				gameObject.GetComponent<Rigidbody> ().AddForceAtPosition (WheelL.transform.up * -antiRollForce, WheelL.transform.position);
			if (groundedR)
				gameObject.GetComponent<Rigidbody> ().AddForceAtPosition (WheelR.transform.up * antiRollForce, WheelR.transform.position);
		}
	}
}