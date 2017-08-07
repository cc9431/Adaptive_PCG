using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoostTrailBlazer : MonoBehaviour {
	private TrailRenderer speedTrail;
	private Light point;
	private float trailTime;

	void Awake() {
		speedTrail = gameObject.GetComponent<TrailRenderer> ();
		point = gameObject.GetComponent<Light> ();
		trailTime = speedTrail.time;
		speedTrail.time = 0;
		point.enabled = false;
	}
		
	void Update () {
		if (_CarController.boosting) {
			point.enabled = true;
			speedTrail.time = trailTime;
		} else if (speedTrail.time > 0) {
			speedTrail.time -= 0.1f;
			if (speedTrail.time <= 0.05) point.enabled = false;
		}
	}
}
