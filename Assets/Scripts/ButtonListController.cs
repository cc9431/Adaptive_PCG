using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonListController : MonoBehaviour {
	public GameObject puzzleObject;

	//Every interactable object should have a pressedButton() function to make it do something when you press its button

	void OnTriggerEnter(Collider Other) {
		if(Other.gameObject.CompareTag("PlayerTrigger")){
			foreach (Transform child in puzzleObject.transform) {
				child.gameObject.SendMessage("pressedButton");
			}
			//puzzleObject.SendMessage("pressedButton");
		}
	}

	void OnTriggerStay(Collider Other) {}
	void OnTriggerExit(Collider Other) {}
		
}
