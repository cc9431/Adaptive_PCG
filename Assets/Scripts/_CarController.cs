using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// --------------------------------------------------------------- //
/* 
		- All the wheels need to move as though they are child objects of each other
		- Right stick [inAir] { : MOSTLY DONE
			Up : Pitch Forward,
			Down : Pitch Backward,
			Left : Roll Left,
			Right : Roll Right,
		}
		- Left stick [Spherically] {
			Up/Down : Camera Up/Down
			Left/Right : Camera Left/Right
		}
		- Very high Wheel dampening : DONE
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
			- rigidbody.AddRelativeForce(tinytiny, ForceMode.Impulse) : DONE

		Visuals:
		- Wheel/RearLight trails
		- Smoke/gravel particles when driving
		- Wind and Trail particles when you hit high speed!
*/ 
// --------------------------------------------------------------- //

public class _CarController : MonoBehaviour {
	private WheelCollider[] WheelColliders;
	private Rigidbody PlayerRB;

	public float RPMs;
	public bool inAir;

	private float HighSteerAngle = 10f;
	private float HorsePower = 2500f;

	void Awake(){
		Physics.gravity = (Vector3.down * 17);
		WheelColliders = gameObject.GetComponentsInChildren<WheelCollider> ();
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
		float Pitch = Input.GetAxis ("Vertical");
		float Accel = Input.GetAxis ("Drive");
		float Reverse = Input.GetAxis ("Reverse");
		float Brake = Input.GetAxis ("Brake");
		float Boost = Input.GetAxis ("Boost");
		float Jump = Input.GetAxis ("Jump");

		float steering = HighSteerAngle * Turn;
		bool maxSpeed = RPMs > 3000;
		float drive = 0;

		WheelColliders [0].steerAngle = steering;
		WheelColliders [1].steerAngle = steering;

		WheelColliders [2].brakeTorque = 1700f * Brake;
		WheelColliders [3].brakeTorque = 1700f * Brake;

		PlayerRB.AddForce (20000f * gameObject.transform.forward * Boost, ForceMode.Force);

		if (inAir) {
			// [rocketleague] If left bumper is pressed have car spin instead of turn?
			// Maybe just use Rigidbody.transform.Rotate?!?!?
			PlayerRB.drag = 0f;
			PlayerRB.AddRelativeTorque (100f * Vector3.up * Turn, ForceMode.Impulse);
			PlayerRB.AddRelativeTorque (100f * Vector3.right * Pitch, ForceMode.Impulse);
		} else {
			PlayerRB.AddForce (6000f * gameObject.transform.up * Jump, ForceMode.Impulse);
			PlayerRB.drag = 0.5f;
			if (!maxSpeed) {
				if (Accel > 0) drive = HorsePower * Accel;
				if (Reverse < 0) drive = HorsePower * Reverse;

				if (steering != 0) drive /= 2f;
			}
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
