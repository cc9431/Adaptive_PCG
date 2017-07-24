using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class ButtonController : MonoBehaviour {
	private bool buttonSelected;
	public EventSystem eventSystem;
	public GameObject selectedObject;

	void Start(){
		eventSystem.SetSelectedGameObject(selectedObject);
	}

	void Update(){
		if (Input.GetAxisRaw("Vertical") != 0 && !buttonSelected) {
			eventSystem.SetSelectedGameObject(selectedObject);
			buttonSelected = true;
		}

		if (Input.GetAxisRaw("Jump") != 0){
			PointerEventData pointer = new PointerEventData(EventSystem.current);
			ExecuteEvents.Execute(selectedObject, pointer, ExecuteEvents.pointerClickHandler);
		}
	}

	void OnDisable(){
		buttonSelected = false;
	}

	public void NewGameButton(){
		bool coinFlip = (Random.Range(0, 2) == 0);
		if (coinFlip) SceneManager.LoadScene("Generate_Infinite");
		else SceneManager.LoadScene("InfiniteL0");
	}

	public void FreePlayButton(){
		SceneManager.LoadScene("Generate_Infinite");	
	}

	public void QuitGame(){
		Application.Quit();
	}

}
