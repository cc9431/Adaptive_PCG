using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

	/// This script is to allow the player to control the car (while they are still alive)
	/// The player can accelerate, reverse, drift/handbrake, boost, jump, and rotate in the air on three axes.
	/// This script keeps track of data about the player's interaction with the game and sends it to the
	/// master controller.

public class _CarController : MonoBehaviour {
	private WheelCollider[] WheelColliders;
	private Rigidbody PlayerRB;
	private WheelFrictionCurve drift;
	private WheelFrictionCurve notDriftSideways;
	private WheelFrictionCurve notDriftForward;

	public float RPMs;
	public float speedGate;
	public float highestPoint;
	public static bool inAir;
	public static bool maxSpeed;
	public static bool boosting;
	public static bool Drift;
	public static bool bodyTouching;
	public static bool onBack;
	public static bool Alive;
	public static bool TimedOut;
	public static bool Keyboard = true;

	private bool lastFrameJump;
	private bool lastFrameAlive = true;
	private float waitForReset;
	public float Boost;
	private float HighSteerAngle = 6f;
	public float HorsePower = 1700f;

	public float MaxTime = 601f;

	float Turn;
	float Accel;
	float Reverse;
	float Jump;

	void OnEnable() {
		SceneManager.sceneLoaded += OnSceneLoaded;
	}	

	void OnDisable() {
		SceneManager.sceneLoaded -= OnSceneLoaded;
	}

	private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
		
	}

	void Awake(){
		Alive = true;
		TimedOut = false;
	}

	void Start(){
		speedGate = 0;
		
		PlayerRB = gameObject.GetComponent<Rigidbody> ();
		PlayerRB.centerOfMass = new Vector3(0f, -0.1f, 0.04f);

		WheelColliders = gameObject.GetComponentsInChildren<WheelCollider> ();
		notDriftSideways = WheelColliders [0].sidewaysFriction;
		notDriftForward = WheelColliders [0].forwardFriction;

		drift = notDriftSideways;
		//drift.extremumValue = 2.1f;
		//drift.asymptoteValue = 2.1f;
		drift.extremumValue = 0.1f;
		drift.asymptoteValue = 0.01f;

		float sliderVal = PlayerPrefs.GetFloat("slider");
		Keyboard = (sliderVal == 0);
	}

	void FixedUpdate () {
		TimedOut = (Time.timeSinceLevelLoad > MaxTime);
		if (!TimedOut) {
			if (Alive) {
				if (!lastFrameAlive){
					foreach (WheelCollider wheel in WheelColliders) {
						wheel.gameObject.SetActive(true);
					}
				}
				if (waitForReset < 99) waitForReset++;
				
				ResetPosition ();
				CheckThatCar ();
				MoveThatCar ();

				if (inAir) MasterController.framesInAir++;
				if (maxSpeed) MasterController.framesAtMax++;
				if (boosting) MasterController.framesBoosting++;
				if (onBack) MasterController.framesOnBack++;
				if (Drift) MasterController.framesDrifting++;

			}
			lastFrameAlive = Alive;
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

		if (PlayerRB.position.y > highestPoint) highestPoint = PlayerRB.position.y;
	}

	void MoveThatCar(){
		float Pitch = Input.GetAxis ("Vertical");
		bool Spin = (Input.GetAxis ("Spin") != 0);
		float drive = 0;
		maxSpeed = RPMs >= 3000;
		Drift = (Input.GetAxis ("Brake") != 0);
		WheelFrictionCurve driftOrNotSideways = new WheelFrictionCurve();
		WheelFrictionCurve driftOrNotForward = new WheelFrictionCurve();

		if (Keyboard){
			if (Input.GetKey (KeyCode.A))		Turn = Mathf.Lerp(Turn, -1, 2*Time.deltaTime);
			else if (Input.GetKey (KeyCode.D))	Turn = Mathf.Lerp(Turn, 1, 2*Time.deltaTime);
			else								Turn = 0;
			if (Input.GetKey (KeyCode.W))		Accel = Mathf.Lerp(Accel, 1, 2*Time.deltaTime);
			else								Accel = 0;
			if (Input.GetKey(KeyCode.S)) 		Reverse = Mathf.Lerp(Reverse, -1, 2*Time.deltaTime);
			else 								Reverse = 0;
			if (Input.GetKey (KeyCode.Space)) 	Jump = 1;
			else 								Jump = 0;
		} else {
			Turn = Input.GetAxis ("Horizontal");
			Accel = Input.GetAxis ("Drive");
			Reverse = Input.GetAxis ("Reverse");
			Jump = Input.GetAxis ("Jump");
		}

		if (speedGate > 0) {
			Boost = 1.5f;
			speedGate -= 0.1f;
		} else Boost = 0;
		boosting = Boost != 0;

		HighSteerAngle = 9 - 3 * (RPMs/3000);
		float steering = HighSteerAngle * Turn;

		WheelColliders [0].steerAngle = steering;
		WheelColliders [1].steerAngle = steering;

		if (!maxSpeed) PlayerRB.AddForce (30000f * gameObject.transform.forward * Boost, ForceMode.Force);

		if (inAir) {
			Drift = false;

			PlayerRB.drag = 0f;
			PlayerRB.angularDrag = 5f;

			RocketLeagueAirControls(Spin, Input.GetAxis ("Horizontal"), Pitch);
		} else {
			PlayerRB.drag = 0.5f;
			PlayerRB.angularDrag = 0.5f;

			if (Drift) {
				driftOrNotSideways = drift;
				driftOrNotForward = drift;
				RocketLeagueAirControls(false, Turn*0.1f, 0f);
				if (Accel != 0 && !maxSpeed) PlayerRB.AddForce (30000f * gameObject.transform.forward, ForceMode.Force);
				Accel = 0;
			} else {
				driftOrNotSideways = notDriftSideways;
				driftOrNotForward = notDriftForward;
			}

			if (!maxSpeed) {
				if (Accel > 0)		drive = HorsePower * (Accel);
				if (Reverse < 0) 	drive = HorsePower * (Reverse);
			} 
			if (Jump != 0 && !lastFrameJump) {
				PlayerRB.AddForce (13000f * gameObject.transform.up * Jump, ForceMode.Impulse);
				MasterController.jumps++;
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

			MasterController.timesReset++;
			waitForReset = 0;
		}
	}

	void RocketLeagueAirControls(bool spin, float turn, float pitch){
		if (spin) 	PlayerRB.AddRelativeTorque (Vector3.back * turn, ForceMode.VelocityChange);
		else 		PlayerRB.AddRelativeTorque (0.93f * Vector3.up * turn, ForceMode.VelocityChange);
					PlayerRB.AddRelativeTorque (0.93f * Vector3.right * pitch, ForceMode.VelocityChange);
	}
}