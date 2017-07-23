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

	void OnTriggerExit(Collider other){
		if (other.CompareTag ("PlayerTrigger") && !playerTouched) {
			Master.PlayerInteracted (id);
			playerTouched = true;
		}
	}

	public void setID(string setid){
		id = setid;
	}

	public string getID(){
		return id;
	}

	public void setMaster(MasterController m){
		Master = m;
	}

	public MasterController getMaster(){
		return Master;
	}
}