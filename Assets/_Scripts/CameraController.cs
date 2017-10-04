using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
	private Transform player;
	private Vector3 centerScreen;
	private Vector3 offset;
	private Rigidbody PlayerRB;
	private Camera cam;

	void Start () {
		player = GameObject.FindGameObjectWithTag("Player").transform;
		PlayerRB = player.GetComponent<Rigidbody> ();
		cam = gameObject.GetComponent<Camera> ();
		offset = transform.position - player.transform.position; // Gets distance from player to camera
		centerScreen = Vector3.up * 3; // Don't want the camera to look directly at player, it feels weird
	}

	void LateUpdate() {
		float target = cam.fieldOfView;;
		transform.position = player.transform.position + (CalculatePos() * offset); // Reset the position based on the player movement and Mouse X input
		transform.LookAt (player.position + centerScreen); // Turn the camera towards the player, but don't look directly down at player

		if (PlayerRB.velocity.magnitude > 65 && PlayerRB.velocity.magnitude < 80) target = 90;
		else if (PlayerRB.velocity.magnitude > 80) target = 105;
		else if (PlayerRB.velocity.magnitude < 50) target = 60;
		if (_CarController.boosting) target += 25;

		if (target != cam.fieldOfView){
			float fov = Mathf.Lerp (cam.fieldOfView, target, Time.deltaTime * 2f);
			cam.fieldOfView = fov;
		}
	}

	Quaternion CalculatePos(){
		float desiredAngle;
		Vector2 localVelNorm = new Vector2 (PlayerRB.velocity.normalized.x, PlayerRB.velocity.normalized.z);
		Vector2 localVel = new Vector2 (Mathf.Abs(PlayerRB.velocity.x), Mathf.Abs(PlayerRB.velocity.z));
		
		if (localVel.x < 4 && localVel.y < 4) desiredAngle = 0;
		else desiredAngle = Mathf.Atan2(localVelNorm.x, localVelNorm.y) * Mathf.Rad2Deg;

		float currAngle = transform.eulerAngles.y;

		// Lerp function with Time.delta creates a smooth transition every frame to the final destination
		float angle = Mathf.LerpAngle(currAngle, desiredAngle, Time.deltaTime * 8f);
		Quaternion rotation = Quaternion.Euler(0, angle, 0);

		return rotation;
	}
}
