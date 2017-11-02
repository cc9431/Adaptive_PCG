using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractController : MonoBehaviour {
	public MasterController Master;
	private int idTrack;
	private int idLev;
	private bool playerTouched;

	void Start(){
		playerTouched = false;
	}

	void OnTriggerEnter(Collider other){
		if (other.CompareTag ("PlayerTrigger") && !playerTouched) {
			MasterController.inObject = true;
			playerTouched = true;
			Master.PlayerInteracted (idTrack, idLev);
		}
	}

	void OnTriggerExit(Collider other){
		if (other.CompareTag ("PlayerTrigger")) MasterController.inObject = false;
	}

	public void setID(int setidTrack, int setidLev){
		idTrack = setidTrack;
		idLev = setidLev;
	}

	public void setMaster(MasterController m){
		Master = m;
	}

	public MasterController getMaster(){
		return Master;
	}
}