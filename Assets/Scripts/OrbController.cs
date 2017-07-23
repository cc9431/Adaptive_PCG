using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbController : MonoBehaviour {
	private string id;
	private Vector3 spin = new Vector3 (0, 40, 0);
	private MasterController Master;
	public float speed;

	void Start () {
		id = GetComponentInParent<InteractController>().getID();
		Master = GetComponentInParent<InteractController>().getMaster();
	}

	void LateUpdate () {
		transform.Rotate (spin * Time.deltaTime * speed);
	}

	void OnTriggerEnter(Collider other){
		if (other.CompareTag ("PlayerTrigger")) {
			Master.OrbCollected (id);
			Destroy(gameObject);
		}
	}
}
