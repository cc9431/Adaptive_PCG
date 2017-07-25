using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SideWallVisual : MonoBehaviour {
	private bool countDown = false;
	private Renderer rend;

	private void Awake(){
		rend = gameObject.GetComponent<Renderer> ();
		rend.enabled = false;
	}

	private void OnTriggerEnter(Collider other){
		if (other.CompareTag("PlayerTrigger")) {
			if (!countDown) {
				rend.enabled = true;
				countDown = true;
				Invoke ("ReEnable", 2);
			} else {
				rend.enabled = false;
			}
		}
	}

	private void ReEnable(){
		rend.enabled = false;
		countDown = false;
	}
}
