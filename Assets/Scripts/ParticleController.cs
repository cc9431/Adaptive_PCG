using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleController : MonoBehaviour {
	private Rigidbody carRB;
	private ParticleSystem PS;
	private float maxSpeed = 10f;
	private float origSpeed = 5f;

	// Use this for initialization
	void Start () {
		PS = GetComponent<ParticleSystem> ();
		carRB = GameObject.FindGameObjectWithTag ("Player").GetComponent<Rigidbody> ();
	}

	void Update () {
		var main = PS.main;
		var trail = PS.trails;

		if (!trail.enabled && carRB.velocity.magnitude > 65) {
			main.startSpeed = maxSpeed;
			trail.enabled = true;
		} else if (trail.enabled && carRB.velocity.magnitude < 50) {
			main.startSpeed = origSpeed;
			trail.enabled = false;
		}
	}
}
