using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
	private Transform player;
	private Vector3 centerScreen;
	private Vector3 offset;
	private Vector3 lookUp;
	private _CarController carScript;
	private Rigidbody PlayerRB;

	void Start () {
		player = GameObject.FindGameObjectWithTag("Player").transform;
		PlayerRB = player.GetComponent<Rigidbody> ();
		offset = transform.position - player.transform.position; // Gets distance from player to camera
		centerScreen = Vector3.up * 2; // Don't want the camera to look directly at player, it feels weird
		carScript= player.GetComponent<_CarController>();
	}

	void LateUpdate() {
		transform.position = player.transform.position + (CalculatePos() * offset); // Reset the position based on the player movement and Mouse X input
		transform.LookAt (player.position + centerScreen); // Turn the camera towards the player, but don't look directly down at player
	}

	Quaternion CalculatePos(){
		float desiredAngle;
		Vector2 localVel = new Vector2 (PlayerRB.velocity.normalized.x, PlayerRB.velocity.normalized.z);
		if (carScript.inAir) desiredAngle = Mathf.Atan2(localVel.x, localVel.y) * Mathf.Rad2Deg;
		else desiredAngle = player.transform.eulerAngles.y;

		float currAngle = transform.eulerAngles.y;

		// Lerp function with Time.delta creates a smooth transition every frame to the final destination
		float angle = Mathf.LerpAngle(currAngle, desiredAngle, Time.deltaTime * 8f);
		Quaternion rotation = Quaternion.Euler(0, angle, 0);
		
		return rotation;
	}
}
