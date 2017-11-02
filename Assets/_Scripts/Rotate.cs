using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour {
	private Vector3 spin = new Vector3 (0, 40, 0);
	public float speed;
	
	// Update is called once per frame
	void LateUpdate () {
		transform.Rotate (spin * Time.deltaTime * speed);
	}
}
