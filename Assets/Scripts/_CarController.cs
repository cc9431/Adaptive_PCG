using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class _CarController : MonoBehaviour {
	private float hoverHeight = 2.5f;
	private float hoverForce = 64f;
	private float proportionalHeight;

	public float speedModifier;
	private float acceleration;
	private float turnInput;

	private Ray groundSensorRay;
	private Vector3 appliedHoverForce;
	private Vector3 gravDown = Vector3.down * 25f;
	private Rigidbody carRB;

	void Start(){
		Physics.gravity = gravDown;
	}

	void Awake(){
		carRB = GetComponent<Rigidbody> ();
	}
	
	// Update is called once per frame
	void Update () {
		acceleration = Input.GetAxis ("Vertical");
		turnInput = Input.GetAxis ("Horizontal");
	}

	void FixedUpdate () {
		groundSensorRay = new Ray (transform.position, -transform.forward);
		RaycastHit hit;

		if (Physics.Raycast (groundSensorRay, out hit, hoverHeight)) {
			proportionalHeight = (hoverHeight - hit.distance) / hoverHeight;
			appliedHoverForce = Vector3.up * hoverForce * proportionalHeight;
			carRB.AddForce (appliedHoverForce, ForceMode.Acceleration);
		}

		carRB.AddRelativeForce (0, -acceleration * speedModifier, 0);
		carRB.AddRelativeTorque (0, 0, turnInput * speedModifier * 0.15f);
	}
}
