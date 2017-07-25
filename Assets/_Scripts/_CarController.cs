using System.Collections;
using System.Collections.Generic;
using UnityEngine;

	// This script is to allow the player to control the car (while they are still alive)
	// The player can accelerate, reverse, drift/handbrake, boost, jump, and rotate in the air on three axes.
	// This script keeps track of data about the player's interaction with the game and sends it to the
	// master controller.

public class _CarController : MonoBehaviour {
	private WheelCollider[] WheelColliders;
	private Rigidbody PlayerRB;
	private MasterController Master;
	private WheelFrictionCurve drift;
	private WheelFrictionCurve notDriftSideways;
	private WheelFrictionCurve notDriftForward;

	public float RPMs;
	public float speedGate;
	public float highestPoint;
	public bool inAir;
	public bool maxSpeed;
	public bool boosting;
	public bool Drift;
	public bool bodyTouching;
	public bool onBack;
	public bool Alive;

	private bool GameEnd = false;
	private bool lastFrameJump;
	private float waitForReset;
	private float Boost;
	private float HighSteerAngle = 6f;
	private float HorsePower = 2500f;
	void Awake(){
		Alive = true;
	}

	void Start(){
		speedGate = 0;
		
		PlayerRB = gameObject.GetComponent<Rigidbody> ();
		PlayerRB.centerOfMass = new Vector3(0f, -0.1f, 0.04f);

		WheelColliders = gameObject.GetComponentsInChildren<WheelCollider> ();
		notDriftSideways = WheelColliders [0].sidewaysFriction;
		notDriftForward = WheelColliders [0].forwardFriction;

		drift = notDriftSideways;
		drift.extremumValue = 0.1f;
		drift.asymptoteValue = 0.1f;

		Master = GameObject.FindGameObjectWithTag ("Master").GetComponent<MasterController> ();
	}

	void FixedUpdate () {
		if (Alive) {
			if (waitForReset < 99) waitForReset++;
			
			ResetPosition ();
			CheckThatCar ();
			MoveThatCar ();
		} else {
			// TODO set all possible booleans to false
			// TODO deathscreen and setup ability to restart
			if (!GameEnd) {
				foreach (WheelCollider wheel in WheelColliders) {
					Destroy (wheel.transform.GetChild(0).gameObject);
					Destroy (wheel);
					GameEnd = true;
				}
			}
		}
	}

	void CheckThatCar(){
		bool FRInAir = !WheelColliders[0].isGrounded;
		bool FLInAir = !WheelColliders[1].isGrounded;
		bool BRInAir = !WheelColliders[2].isGrounded;
		bool BLInAir = !WheelColliders[3].isGrounded;

		inAir =  (FRInAir || FLInAir || BRInAir || BLInAir);

		onBack = inAir && bodyTouching;

		RPMs = 0;
		foreach(WheelCollider wheel in WheelColliders) RPMs += wheel.rpm;
		RPMs = RPMs/4;

		if (PlayerRB.position.y > highestPoint)
			highestPoint = PlayerRB.position.y;
	}

	void MoveThatCar(){
		float Turn = Input.GetAxis ("Horizontal");
		float Pitch = Input.GetAxis ("Vertical");
		float Accel = Input.GetAxis ("Drive");
		float Reverse = Input.GetAxis ("Reverse");
		float Jump = Input.GetAxis ("Jump");
		bool Spin = (Input.GetAxis ("Spin") != 0);
		Drift = (Input.GetAxis ("Brake") != 0);

		/*
		if (InputMethod = Keyboard){
			if (Input.GetKey (KeyCode.A))
				Turn = -1;
			else if (Input.GetKey (KeyCode.D))
				Turn = 1;
			else
				Turn = 0;
			
			if (Input.GetKey (KeyCode.W))
				Accel = 1;
			else
				Accel = 0;

			if (Input.GetKey (KeyCode.LeftShift)) Jump = 1;
			else Jump = 0;
		}*/
		
		if (speedGate > 0) {
			Boost = 1.5f;
			speedGate -= 0.1f;
		} else Boost = Input.GetAxis ("Boost");

		float steering = HighSteerAngle * Turn;
		float drive = 0;

		maxSpeed = RPMs >= 3000;
		boosting = Boost != 0;

		WheelColliders [0].steerAngle = steering;
		WheelColliders [1].steerAngle = steering;

		WheelFrictionCurve driftOrNotSideways = new WheelFrictionCurve ();
		WheelFrictionCurve driftOrNotForward = new WheelFrictionCurve ();

		if (!maxSpeed) PlayerRB.AddForce (30000f * gameObject.transform.forward * Boost, ForceMode.Force);

		if (inAir) {
			Drift = false;
			driftOrNotForward = notDriftForward;
			driftOrNotSideways = notDriftSideways;

			PlayerRB.drag = 0f;
			PlayerRB.angularDrag = 5f;

			RocketLeagueAirControls(Spin, Turn, Pitch);
		} else {
			PlayerRB.drag = 0.5f;

			if (Drift) {
				PlayerRB.angularDrag = 5f;

				driftOrNotForward = drift;
				driftOrNotSideways = drift;
				RocketLeagueAirControls(Spin, Turn, Pitch);
			} else {
				PlayerRB.angularDrag = 0.5f;

				driftOrNotForward = notDriftForward;
				driftOrNotSideways = notDriftSideways; 
			}
			if (!maxSpeed) {
				if (Accel > 0)
					drive = HorsePower * (Accel);
				if (Reverse < 0) drive = HorsePower * (Reverse);
			} 
			if (Jump != 0 && !lastFrameJump) {
				PlayerRB.AddForce (11000f * gameObject.transform.up * Jump, ForceMode.Impulse);
				Master.PlayerJumped ();
			}
		}

		foreach (WheelCollider wheel in WheelColliders) {
			wheel.sidewaysFriction = driftOrNotSideways;
			wheel.forwardFriction = driftOrNotForward;

			wheel.motorTorque = drive;
			FixMeshPositions(wheel);
		}

		lastFrameJump = (Jump == 1);
	}

	void FixMeshPositions(WheelCollider collider){
        Transform visualWheel = collider.transform.GetChild(0);
 
        Vector3 position;
        Quaternion rotation;
        collider.GetWorldPose(out position, out rotation);
 
        visualWheel.transform.position = position;
        visualWheel.transform.rotation = rotation;
    }

	void ResetPosition() {
		bool Reset = (Input.GetAxis ("Reset") != 0);

		if (waitForReset == 99 && Reset) {
			float playerZ = transform.position.z;

			PlayerRB.velocity = Vector3.zero;
			transform.rotation = Quaternion.identity;
			transform.position = new Vector3 (0, 5, playerZ);

			Master.PlayerReset ();
			waitForReset = 0;
		}
	}

	void RocketLeagueAirControls(bool spin, float turn, float pitch){
		if (spin) 	PlayerRB.AddRelativeTorque (Vector3.back * turn, ForceMode.VelocityChange);
			else 		PlayerRB.AddRelativeTorque (0.93f * Vector3.up * turn, ForceMode.VelocityChange);
						PlayerRB.AddRelativeTorque (0.93f * Vector3.right * pitch, ForceMode.VelocityChange);
	}
}