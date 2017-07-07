using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
		bool Spin = (Input.GetAxis ("Spin") != 0);

		float steering = HighSteerAngle * Turn;
		bool maxSpeed = RPMs > 3000;
		float drive = 0;

		WheelColliders [0].steerAngle = steering;
		WheelColliders [1].steerAngle = steering;

		WheelColliders [2].brakeTorque = HorsePower * Brake;
		WheelColliders [3].brakeTorque = HorsePower * Brake;

		if (!maxSpeed) PlayerRB.AddForce (20000f * gameObject.transform.forward * Boost, ForceMode.Force);

		if (inAir) {
			PlayerRB.drag = 0f;
			PlayerRB.angularDrag = 3.5f;

			if (Spin) PlayerRB.AddRelativeTorque (0.5f * Vector3.back * Turn, ForceMode.VelocityChange);
			else PlayerRB.AddRelativeTorque (0.5f * Vector3.up * Turn, ForceMode.VelocityChange);
			PlayerRB.AddRelativeTorque (0.5f * Vector3.right * Pitch, ForceMode.VelocityChange);
		} else {
			PlayerRB.drag = 0.5f;
			PlayerRB.angularDrag = 0.5f;

			if (!maxSpeed) {
				if (Accel > 0) drive = HorsePower * (Accel - Brake);
				if (Reverse < 0) drive = HorsePower * (Reverse - Brake);

				if (steering != 0) drive /= 2f;
			}
			PlayerRB.AddForce (6000f * gameObject.transform.up * Jump, ForceMode.Impulse);
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
