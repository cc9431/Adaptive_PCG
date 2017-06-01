using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Speedometer : MonoBehaviour{
	public Rigidbody PlayerRB;
	private float maxSpeed = 88f;
	private Image speedometer;


	void Start(){
		speedometer = GetComponent<Image>();
	}

	void Update(){


		speedometer.fillAmount = PlayerRB.velocity.magnitude/maxSpeed;
	}
}