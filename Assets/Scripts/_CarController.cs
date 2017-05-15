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

	private float HighSteerAngle = 25f;
	private float HorsePower = 30f;

	private Vector3 downForce;
	//private Vector3 currSpeed;
	//private Vector3 minSpeed;
	//private Vector3 maxSpeed;

	void Awake(){
		WheelColliders = gameObject.GetComponentsInChildren<WheelCollider> ();

		foreach (WheelCollider wheel in WheelColliders) wheel.motorTorque = 0.000001f;
	}

	void Start(){
		//inAir = false;
		PlayerRB = gameObject.GetComponent<Rigidbody> ();

		PlayerRB.centerOfMass = new Vector3(0f, 0.03f, 0.02f);
		downForce = new Vector3(0f, -1f, 0f);
		//minSpeed = new Vector3 (-30f, -30f, -30f);
		//maxSpeed = new Vector3 (30f, 30f, 30f);
	}

	void Update(){
		
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

		if (true){
			float drive = HorsePower * Accel;
			foreach (WheelCollider wheel in WheelColliders) {
				wheel.motorTorque = drive;

				if (Brake){
					wheel.brakeTorque = 40f;
					PlayerRB.AddForce(downForce); 
				} else wheel.brakeTorque = 0f;

				FixMeshPositions(wheel);
			}

			PlayerRB.AddRelativeForce (3000f * Vector3.forward * Accel, ForceMode.Force);
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

	void OnTriggerEnter(Collider Other) {
		//inAir = false;
		//Debug.Log ("NOT IN AIR");

	}

	void OnTriggerExit(Collider Other) {
		//inAir = true;
		//Debug.Log ("IN AIR");
	}
}
