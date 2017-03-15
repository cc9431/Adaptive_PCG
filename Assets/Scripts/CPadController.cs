using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CPadController : MonoBehaviour {
	private int rotating_speed;
	private string direction;

	public GameObject puzzleObject;
	public GameObject Pad;

	/* Every interactable puzzle object should have two
	functions called counterClockwise and clockwise 
	that do something to the object when the pad is rotated*/

	void OnTriggerEnter(Collider Other){
		if (gameObject.CompareTag ("CounterClockwise")) { // checks which way pad should be rotated
			rotating_speed = -10;
			direction = "counterClockwise";
		} else {
			rotating_speed = 10;
			direction = "clockwise";
		}

	}

	void OnTriggerStay(Collider Other){
		if(Other.gameObject.CompareTag("PlayerTrigger")){
			Pad.transform.Rotate(new Vector3 (0, rotating_speed,  0) * Time.deltaTime);
			puzzleObject.SendMessage (direction);
		}
	}

	void OnTriggerExit(Collider Other){}


}
