using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaxSpeedTrail : MonoBehaviour {
	private TrailRenderer speedTrail;
	private Light point;
	private Rigidbody carRB;
	private Color trailColor;

	// Use this for initialization
	void Awake() {
		carRB = GameObject.FindGameObjectWithTag ("Player").GetComponent<Rigidbody> ();
		speedTrail = gameObject.GetComponent<TrailRenderer> ();
		point = gameObject.GetComponent<Light> ();
		speedTrail.time = 0;
		trailColor = speedTrail.startColor;
		point.enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (carRB.velocity.magnitude > 50) {
			point.enabled = true;
			speedTrail.time = 0.3f;
			if (carRB.velocity.magnitude > 60)
				speedTrail.startColor = Color.Lerp (speedTrail.startColor, Color.red, Time.deltaTime);
			else
				speedTrail.startColor = Color.Lerp (speedTrail.startColor, trailColor, Time.deltaTime * 5f);
		} else if (speedTrail.time > 0) {
			speedTrail.time -= 0.003f;
			if (speedTrail.time <= 0.05) point.enabled = false;
		}
	}
}
