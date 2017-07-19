using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class _CarController : MonoBehaviour {
	private WheelCollider[] WheelColliders;
	private Rigidbody PlayerRB;
	private MasterController Master;
	private WheelFrictionCurve drift;
	private WheelFrictionCurve notDriftSideways;
	private WheelFrictionCurve notDriftForward;

	public float RPMs;
	public float speedGate;
	public bool inAir;
	public bool maxSpeed;
	public bool boosting;
	public bool Brake;
	public bool bodyTouching;
	public bool onBack;
	public bool Alive = true;

	private bool GameEnd = false;
	private bool lastFrameReset;
	private bool lastFrameJump;
	private float waitForReset;
	private float Boost;
	private float HighSteerAngle = 6f;
	private float HorsePower = 2500f;

	void Start(){
		speedGate = 0;
		PlayerRB = gameObject.GetComponent<Rigidbody> ();
		WheelColliders = gameObject.GetComponentsInChildren<WheelCollider> ();
		notDriftSideways = WheelColliders [0].sidewaysFriction;
		notDriftForward = WheelColliders [0].forwardFriction;

		drift = notDriftSideways;
		drift.stiffness = 0.5f;
		drift.extremumValue = 1;

		Master = GameObject.FindGameObjectWithTag ("Master").GetComponent<MasterController> ();

		PlayerRB.centerOfMass = new Vector3(0f, -0.1f, 0.04f);
	}

	void FixedUpdate () {
		if (Alive) {
			if (waitForReset < 99)
				waitForReset++;
			ResetPosition ();
			CheckThatCar ();
			MoveThatCar ();
		} else {
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
		bool FRInAir = !WheelColliders[0] .isGrounded;
		bool FLInAir = !WheelColliders[1] .isGrounded;
		bool BRInAir = !WheelColliders[2] .isGrounded;
		bool BLInAir = !WheelColliders[3] .isGrounded;

		inAir =  FRInAir || FLInAir || BRInAir || BLInAir;

		onBack = inAir && bodyTouching;

		RPMs = 0;
		foreach(WheelCollider wheel in WheelColliders) RPMs += wheel.rpm;
		RPMs = RPMs/4;
	}

	void MoveThatCar(){
		float Turn = Input.GetAxis ("Horizontal");
		float Pitch = Input.GetAxis ("Vertical");
		float Accel = Input.GetAxis ("Drive");
		float Reverse = Input.GetAxis ("Reverse");
		float Jump = Input.GetAxis ("Jump");
		bool Spin = (Input.GetAxis ("Spin") != 0);
		Brake = (Input.GetAxis ("Brake") != 0);

		/*if (Input.GetKey (KeyCode.A))
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
		else Jump = 0;*/
		
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
		if (Brake) {
			driftOrNotForward = drift;
			driftOrNotSideways = drift;
		} else {
			driftOrNotForward = notDriftForward;
			driftOrNotSideways = notDriftSideways; 
		}
			

		if (!maxSpeed) PlayerRB.AddForce (30000f * gameObject.transform.forward * Boost, ForceMode.Force);

		if (inAir) {
			PlayerRB.drag = 0f;
			PlayerRB.angularDrag = 5f;

			if (Spin) PlayerRB.AddRelativeTorque (Vector3.back * Turn, ForceMode.VelocityChange);
			else PlayerRB.AddRelativeTorque (0.93f * Vector3.up * Turn, ForceMode.VelocityChange);
			PlayerRB.AddRelativeTorque (0.93f * Vector3.right * Pitch, ForceMode.VelocityChange);
		} else {
			PlayerRB.drag = 0.5f;
			PlayerRB.angularDrag = 0.5f;

			if (!maxSpeed) {
				if (Accel > 0)
					drive = HorsePower * (Accel);// - Brake);
				if (Reverse < 0) drive = HorsePower * (Reverse);// - Brake);
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
}