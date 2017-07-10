using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractController : MonoBehaviour {
	private MasterController Master;
	private string id;

	void Start(){
		Master = GameObject.FindGameObjectWithTag ("Master").GetComponent<MasterController> ();
	}


	void OnTriggerEnter(Collider other){
		if (other.CompareTag("PlayerTrigger")) Master.PlayerInteracts(id);
	}

	public void setID(string setid){
		id = setid;
	}
}
