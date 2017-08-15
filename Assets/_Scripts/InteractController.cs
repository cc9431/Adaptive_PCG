using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractController : MonoBehaviour {
	private MasterController Master;
	private string id;
	private bool playerTouched;

	void Start(){
		playerTouched = false;
	}

	void OnTriggerEnter(Collider other){
		if (other.CompareTag ("PlayerTrigger") && !playerTouched) {
			MasterController.inObject = true;
			playerTouched = true;
			Master.PlayerInteracted (id);
		}
	}

	void OnTriggerExit(Collider other){
		if (other.CompareTag ("PlayerTrigger")) MasterController.inObject = false;
	}

	public void setID(string setid){
		id = setid;
	}

	public void setMaster(MasterController m){
		Master = m;
	}

	public MasterController getMaster(){
		return Master;
	}
}