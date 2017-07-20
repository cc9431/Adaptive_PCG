using System.Collections;
using System.Collections.Generic;
using UnityEngine;

	// This script is used to animate the particles in the main game.
	// When the player reaches close to maximum speed, the particles will
	// begin to stream past the player. When the player reduces their speed
	// the particle trails' length will decay to 0

public class ParticleController : MonoBehaviour {
	private Transform cam;
	private Rigidbody carRB;
	private ParticleSystem PS;
	private ParticleSystem.TrailModule trail; 
	ParticleSystem.MinMaxCurve rate;
	private float maxSpeed = 800f;
	private float origSpeed = 5f;

	void Start () {
		cam = GameObject.FindGameObjectWithTag ("MainCamera").transform;
		carRB = GameObject.FindGameObjectWithTag ("Player").GetComponent<Rigidbody> ();
		PS = GetComponent<ParticleSystem> ();

		rate = new ParticleSystem.MinMaxCurve ();

		trail = PS.trails;
		trail.enabled = true;
	}

	void FixedUpdate () {
		var main = PS.main;
		Quaternion camRotation = new Quaternion ();
		camRotation.y = cam.rotation.y;

		gameObject.transform.rotation = camRotation;

		if (carRB.velocity.magnitude > 70) {
			
			main.startSpeed = maxSpeed;

			rate.constantMax = 0.08f;
			rate.constantMin = 0.01f;
			trail.lifetime = rate;
		} else if (carRB.velocity.magnitude < 60) {
			
			main.startSpeed = origSpeed;

			rate.constantMin = 0;
			trail.lifetime = rate;
			if (trail.lifetime.constantMax > 0) {
				// float lerpRate = Mathf.Lerp (rate.constantMax, 0, Time.deltaTime);
				// rate.constantMax = lerpRate;
				rate.constantMax -= 0.008f;
			}
		}
	}
}
