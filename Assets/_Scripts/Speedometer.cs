using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Speedometer : MonoBehaviour{
	public Rigidbody PlayerRB;
	private float maxSpeed = 88f;
	private Image speedometer;
	private float prevFillAmount;

	void Start(){
		speedometer = GetComponent<Image>();
	}

	void Update(){
		Vector2 localVel = new Vector2 (PlayerRB.velocity.x, PlayerRB.velocity.z);
		float newFillAmount = (localVel.magnitude/maxSpeed);

		speedometer.fillAmount = Mathf.Lerp(prevFillAmount, newFillAmount, Time.deltaTime);

		prevFillAmount = newFillAmount;
	}
}