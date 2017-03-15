using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinablePlatformController : MonoBehaviour {
	private float endRotation;
	private float speed;

	public GameObject puzzleObject;

	private Vector3 destEuler = new Vector3(-90,0,0);
	private Vector3 currEuler = new Vector3(-90,0,0);

	private bool puzzleObjectOnPad;
	public bool buttonPressed;

	void Start () {
		endRotation = 90.0f; //The amount that you want to object to spin
		speed = 10.0f;
		transform.eulerAngles = destEuler; //set the transform of the object equal to the curr/destEuler just in case it wasn't placed correctly
		//puzzleObjectOnPad = true; //set the ability to rotate puzzle object to false because why would we start with the puzzle finished?
	}
		
	void FixedUpdate () {
		if (buttonPressed) {
			destEuler.y += endRotation; //Set the destination rotation to be 90 degrees away
			buttonPressed = false; //So the turn only happens once per button press
		}

		currEuler = Vector3.Lerp(currEuler, destEuler, Time.deltaTime * speed); //Lerp function with Time.delta creates a smooth transition every frame to the final destination
		transform.eulerAngles = currEuler; //Set the rotation to for every frame so object actually spins in real time
		if (puzzleObjectOnPad) { //If our puzzle object is on top of the pad then spin it as well
			puzzleObject.transform.eulerAngles = currEuler;
		}

	}

	void OnTriggerEnter(Collider Other){
		if (Other.gameObject.Equals(puzzleObject)) {
			puzzleObjectOnPad = true; // Check if our puzzle object is on the pad
		}
	}

	void OnTriggerStay(Collider Other){
		if (Other.gameObject.Equals(puzzleObject)) {
			puzzleObjectOnPad = true; // Check if our puzzle object is on the pad
		}
	}

	void OnTriggerExit(Collider Other){
		if (Other.gameObject.Equals(puzzleObject)) {
			puzzleObjectOnPad = false;
		}
	}

	void pressedButton(){
		buttonPressed = true;
	}

	void counterClockwise(){
		transform.Rotate(new Vector3 (0, 10,  0) * Time.deltaTime);
	}

	void clockwise(){
		transform.Rotate(new Vector3 (0, -10,  0) * Time.deltaTime);
	}

}
