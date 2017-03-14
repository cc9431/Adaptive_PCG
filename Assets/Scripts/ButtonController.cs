using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonController : MonoBehaviour {
	public GameObject puzzleObject;

	//Every interactable object should have a pressedButton() function to make it do something when you press its button

	void OnTriggerEnter(Collider Other) {
		if(Other.gameObject.CompareTag("PlayerTrigger")){
			puzzleObject.SendMessage("pressedButton"); 
		}
	}

	void OnTriggerStay(Collider Other) {}
	void OnTriggerExit(Collider Other) {}
		
}
