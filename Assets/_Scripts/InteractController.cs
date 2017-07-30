using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractController : MonoBehaviour {
	private MasterController Master;
	private string id;
	private bool playerTouched;
	private bool inObject;
	private bool lastFrameinObject;

	void Start(){
		playerTouched = false;
	}

	void Update(){
		MasterController.inObject = (inObject || lastFrameinObject);
		lastFrameinObject = inObject;
		
		if (inObject) print ("Interact inObject: " + inObject.ToString());
	}

	void OnTriggerEnter(Collider other){
		if (other.CompareTag ("PlayerTrigger") && !playerTouched) {
			inObject = true;
			playerTouched = true;
			Master.PlayerInteracted (id);
		}
	}

	void OnTriggerExit(Collider other){
		if (other.CompareTag("PlayerTrigger")){
			inObject = false;
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