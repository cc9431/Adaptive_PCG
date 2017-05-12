using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class _CarController : MonoBehaviour {

	public GameObject CarBody;
	public GameObject[] WheelMeshes = new GameObject[4];
	private Rigidbody PlayerRB;

	private float Accel;
	private float Turn;
	private bool inAir;

	void Start(){
		PlayerRB = GetComponent<Rigidbody> ();
	}

	void Update(){
		FixMeshPositions ();
	}

	void FixedUpdate () {
		MoveThatCar ();
	}

	void MoveThatCar(){
		Turn = Input.GetAxis ("Horizontal");
		Accel = Input.GetAxis ("Vertical");
	
		/*CurrentSteerAngle = Mathf.Lerp(LowSteerAngle, HighSteerAngle, SpeedFactor);

		CurrentSteerAngle *= Turn;*/

		PlayerRB.AddRelativeForce (45f * Vector3.forward * Accel);
		PlayerRB.transform.Rotate (5f * Vector3.up * Turn);

	}

	void FixMeshPositions(){

	}

	void OnTriggerEnter(Collider Other) {
		inAir = false;
		Debug.Log ("NOT IN AIR");

	}

	void OnTriggerExit(Collider Other) {
		inAir = true;
		Debug.Log ("IN AIR");
	}
}
