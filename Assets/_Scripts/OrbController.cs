using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbController : MonoBehaviour {
	private Vector3 spin = new Vector3 (0, 40, 0);
	private MasterController Master;
	public float speed;
	public int points;

	void Start () {
		Master = GetComponentInParent<InteractController>().getMaster();
	}

	void LateUpdate () {
		transform.Rotate (spin * Time.deltaTime * speed);
	}

	void OnTriggerEnter(Collider other){
		if (other.CompareTag ("PlayerTrigger")) {
			Master.InteractPoints (true, points);
			Destroy(gameObject);
		}
	}
}
