using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// --------------------------------------------------------------- //
/* 
	Ideas for car improvments:
		- All the wheels need to move as though they are child objects of each other
		- Tiny amount of relative forward force in drive
			- ForceMode.Impulse?
		- Right stick [inAir] {
			Up : Pitch Forward,
			Down : Pitch Backward,
			Left : Roll Left,
			Right : Roll Right,
		}
		- Left stick [Spherically] {
			Up/Down : Camera Up/Down
			Left/Right : Camera Left/Right
		}
		- Very high Wheel dampening
		- if(body.isGrounded && Wheels.inAir && pressA){
			RigidBody.centerOfGravity = Vector3.Down * 25
		} else {
			RigidBody.centerOfGravity = centerOfGrav // Global variable
		}
		- foreach(WheelCollider wheel in WheelColliders){
			inAir = !wheel.isGrounded && inAir
		}
		- if (carSpeed < someNumber){
			wheels.addTorque(normal)
			rigibody.addRelativeForce(tiny)
		}
		- Back wheels wider that front
		- Maybe... If wheels are grounded, addforce down on all the wheels... [That's what Stabilize.cs is trying to do!]
		- Boost.....
			- small invisible cube in the back of the car?
			- rigidbody.AddRelativeForce(tinytiny, ForceMode.Impulse)

		Visuals:
		- Wheel/RearLight trails
		- Smoke/gravel particles when driving
		- Wind and Trail particles when you hit high speed!
*/ 
// --------------------------------------------------------------- //

public class _CarController : MonoBehaviour {
	private WheelCollider[] WheelColliders;
	private Rigidbody PlayerRB;

	private float RPMs;
	private bool inAir;

	private float HighSteerAngle = 10f;
	private float HorsePower = 1300f;

	void Awake(){
		Physics.gravity = (Vector3.down * 17);
		WheelColliders = gameObject.GetComponentsInChildren<WheelCollider> ();

		foreach (WheelCollider wheel in WheelColliders) {
			wheel.motorTorque = 0.00001f;
		}
	}

	void Start(){
		PlayerRB = gameObject.GetComponent<Rigidbody> ();
		PlayerRB.centerOfMass = new Vector3(0f, -0.1f, 0.04f);
	}

	void FixedUpdate () {
		CheckInAirAndSpeed();
		MoveThatCar ();
	}

	void CheckInAirAndSpeed(){
		bool FRInAir = !WheelColliders[0] .isGrounded;
		bool FLInAir = !WheelColliders[1] .isGrounded;
		bool BRInAir = !WheelColliders[2] .isGrounded;
		bool BLInAir = !WheelColliders[3] .isGrounded;

		inAir =  FRInAir && FLInAir && BRInAir && BLInAir;

		RPMs = 0;
		foreach(WheelCollider wheel in WheelColliders) RPMs += wheel.rpm;
		RPMs = RPMs/4;
	}

	void MoveThatCar(){
		float Turn = Input.GetAxis ("Horizontal");
		float Accel = Input.GetAxis ("Drive");
		float Reverse = Input.GetAxis ("Reverse");
		float Brake = Input.GetAxis("Brake");
		float Boost = Input.GetAxis("Boost");
		float Jump = Input.GetAxis("Jump");

		float steering = HighSteerAngle * Turn;
		WheelColliders [0].steerAngle = steering;
		WheelColliders [1].steerAngle = steering;

		WheelColliders [2].brakeTorque = 1700f * Brake;
		WheelColliders [3].brakeTorque = 1700f * Brake;

		if (!inAir) PlayerRB.AddForce (5000f * gameObject.transform.up * Jump, ForceMode.Impulse);

		PlayerRB.AddForce (40000f * gameObject.transform.forward * Boost, ForceMode.Force);

		bool maxSpeed = RPMs > 3000;
		float drive = 3000;
		if (!maxSpeed & !inAir){
			if (Accel > 0) drive = HorsePower * Accel;
			if (Reverse < 0) drive = HorsePower * Reverse;
		}

		foreach (WheelCollider wheel in WheelColliders) {
				wheel.motorTorque = drive;
				FixMeshPositions(wheel);	
		}
	}

	void FixMeshPositions(WheelCollider collider){
        Transform visualWheel = collider.transform.GetChild(0);
 
        Vector3 position;
        Quaternion rotation;
        collider.GetWorldPose(out position, out rotation);
 
        visualWheel.transform.position = position;
        visualWheel.transform.rotation = rotation;
    }
}
