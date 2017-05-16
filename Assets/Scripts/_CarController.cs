using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class _CarController : MonoBehaviour {
	private WheelCollider[] WheelColliders;
	private Rigidbody PlayerRB;

	private float Accel;
	private float Turn;
	private bool Brake;
	//private bool inAir;

	private float HighSteerAngle = 10f;
	private float HorsePower = 1300f;

	private Vector3 downForce;
	private Vector3 maxSpeed;

	void Awake(){
		Physics.gravity = (Vector3.down * 17);
		WheelColliders = gameObject.GetComponentsInChildren<WheelCollider> ();

		foreach (WheelCollider wheel in WheelColliders) wheel.ConfigureVehicleSubsteps(10, 10, 30);
	}

	void Start(){
		//inAir = false;
		PlayerRB = gameObject.GetComponent<Rigidbody> ();

		PlayerRB.centerOfMass = new Vector3(0f, -0.1f, 0.09f);
		downForce = new Vector3(0f, -200f, 0f);
		maxSpeed = new Vector3 (40f, 0f, 40f);
	}

	void FixedUpdate () {
		MoveThatCar ();
	}

	void MoveThatCar(){
		Turn = Input.GetAxis ("Horizontal");
		Accel = Input.GetAxis ("Vertical");
		Brake = Input.GetKey (KeyCode.Space);

		float steering = HighSteerAngle * Turn;
		WheelColliders [0].steerAngle = steering;
		WheelColliders [1].steerAngle = steering;

		float drive = HorsePower * Accel;
		foreach (WheelCollider wheel in WheelColliders) {
			wheel.motorTorque = drive;

			if (Brake){
				wheel.brakeTorque = 2000f;
				PlayerRB.AddForce(downForce); 
			} else wheel.brakeTorque = 0f;

			FixMeshPositions(wheel);
		}

		//bool canDrive = (PlayerRB.velocity.x < maxSpeed.x) && (PlayerRB.velocity.z < maxSpeed.z);
		//if (canDrive) PlayerRB.AddRelativeForce (4000f * Vector3.forward * Accel, ForceMode.Force);
	}

	void FixMeshPositions(WheelCollider collider){
        Transform visualWheel = collider.transform.GetChild(0);
 
        Vector3 position;
        Quaternion rotation;
        collider.GetWorldPose(out position, out rotation);
 
        visualWheel.transform.position = position;
        visualWheel.transform.rotation = rotation;
    }

	void OnTriggerEnter(Collider Other) {
		//inAir = false;
		//Debug.Log ("NOT IN AIR");

	}

	void OnTriggerExit(Collider Other) {
		//inAir = true;
		//Debug.Log ("IN AIR");
	}
}
