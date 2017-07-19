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
	private ParticleSystem.TrailModule trail;
	ParticleSystem.MinMaxCurve rate;
	private float maxSpeed = 100f;
	private float origSpeed = 5f;

	void Start () {
		carRB = GameObject.FindGameObjectWithTag ("Player").GetComponent<Rigidbody> ();
		PS = GetComponent<ParticleSystem> ();

		rate = new ParticleSystem.MinMaxCurve ();

		trail = PS.trails;
		trail.enabled = true;
	}

	void LateUpdate () {
		var main = PS.main;

		if (carRB.velocity.magnitude > 65) {
			
			main.startSpeed = maxSpeed;

			rate.constantMax = 1;
			rate.constantMin = 0.5f;
			trail.lifetime = rate;
		} else if (carRB.velocity.magnitude < 60) {
			
			main.startSpeed = origSpeed;

			rate.constantMin = 0;
			trail.lifetime = rate;
			if (trail.lifetime.constantMax > 0) {
				rate.constantMax -= 0.02f;
			}
		}
	}
}
