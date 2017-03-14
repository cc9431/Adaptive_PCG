using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
	private Transform player;
	private Vector3 lookUp;
	private Vector3 offset;

	public float turnSpeed;

	void Start () {
		player = GameObject.FindGameObjectWithTag("Player").transform;
		offset = transform.position - player.transform.position; //Gets distance from player to camera
		lookUp = Vector3.up * 2; //Don't want the camera to look directly at player, it feels weird
	}

	void LateUpdate() {
		//This spins the offset around the player when the left or right keys are pressed, the "Mouse X" axis is just a place holder for now
		offset = Quaternion.AngleAxis (Input.GetAxis ("CameraAxis") * turnSpeed, lookUp) * offset;

		transform.position = player.position + offset; //Reset the position based on the player movement and Mouse X input
		transform.LookAt (player.position + lookUp); //Turn the camera towards the player, but don't look directly down at player
	}
}
