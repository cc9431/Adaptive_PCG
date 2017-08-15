using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunMoon : MonoBehaviour {

	private Vector3 spin = new Vector3 (100, 10, 0);
	public float speed;

	void LateUpdate () {
		transform.Rotate(spin * Time.deltaTime * speed);
	}
}
