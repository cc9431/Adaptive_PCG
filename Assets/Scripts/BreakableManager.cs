using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableManager : MonoBehaviour {

    private _CarController player;

	// Use this for initialization
	void Start () {
		//player = GameObject.FindGameObjectWithTag("Player").GetComponent<_CarController>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter(Collider Other) {
    }
}
