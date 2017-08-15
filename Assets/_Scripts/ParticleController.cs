using System.Collections;
using System.Collections.Generic;
using UnityEngine;

	// This script is used to animate the particles in the main game.
	// When the player reaches close to maximum speed, the particles will
	// begin to stream past the player. When the player reduces their speed
	// the particle trails' length will decay to 0

public class ParticleController : MonoBehaviour {
	private Rigidbody carRB;
	private ParticleSystem PS;

	void Start () {
		carRB = GameObject.FindGameObjectWithTag ("Player").GetComponent<Rigidbody> ();
		PS = GetComponent<ParticleSystem>();
	}

	void FixedUpdate () {
		var main = PS.main;

		Vector3 rot = transform.rotation.eulerAngles;
		rot.x = 0;
		Quaternion qrot = Quaternion.Euler(rot);
		transform.rotation = qrot;
		
		if (carRB.velocity.magnitude > 80) main.startLifetime = 0.5f;
		else main.startLifetime = 0;
		//else if (carRB.velocity.magnitude < 65) gameObject.SetActive(false);
	}
}
