using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoverObject : MonoBehaviour {
	public float hoverHeight = 2.5f;
	public float hoverForce = 100f;
	private float proportionalHeight;

	private Ray groundSensorRay;
	private Vector3 appliedHoverForce;
	private Rigidbody ObjectRB;

	public bool buttonPressed;

	//I need to put comments on this

	void Start(){
		ObjectRB = GetComponent<Rigidbody> ();
	}

	void pressedButton(){
		buttonPressed = true;
	}

	void FixedUpdate () {

		if (buttonPressed) {
			groundSensorRay = new Ray (transform.position, -transform.up);
			RaycastHit hit;

			if (Physics.Raycast (groundSensorRay, out hit, hoverHeight)) {
				proportionalHeight = (hoverHeight - hit.distance) / hoverHeight;
				appliedHoverForce = Vector3.up * hoverForce * proportionalHeight;
				ObjectRB.AddForce (appliedHoverForce, ForceMode.Acceleration);
			}
		}
	}
}
