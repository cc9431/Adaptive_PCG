using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class UIController : MonoBehaviour {
	private bool buttonSelected;
	private bool lastFramePause = false;
	public bool Paused;
	private _CarController carController;
	public EventSystem eventSystem;
	public GameObject selectedObject;
	public GameObject PauseScreen;
	public GameObject DeathScreen;

	void Awake(){
		if (PauseScreen != null) PauseScreen.SetActive(false);
		if (DeathScreen != null) DeathScreen.SetActive(false);
	}

	void Start(){
		carController = GameObject.FindGameObjectWithTag("Player").GetComponent<_CarController>();
		eventSystem.SetSelectedGameObject(selectedObject);
	}

	void Update(){
		if (Input.GetAxisRaw("Vertical") != 0 && !buttonSelected) {
			eventSystem.SetSelectedGameObject(selectedObject);
			buttonSelected = true;
		}

		if (Input.GetAxisRaw("Jump") != 0){
			GameObject button = eventSystem.currentSelectedGameObject;
			PointerEventData pointer = new PointerEventData(EventSystem.current);
			ExecuteEvents.Execute(button, pointer, ExecuteEvents.pointerClickHandler);
		}

		if (carController.Alive) {
			// Where we check if the player pauses the game or not
			bool Pause = (Input.GetAxisRaw("Submit") != 0); // = Input.GetKeyDown(KeyCode.Space);

			if (Pause && !lastFramePause) Paused = !Paused;

			if (PauseScreen != null) PauseScreen.SetActive(Paused);

			if (Paused)	Time.timeScale = 0;
			if (!Paused) Time.timeScale = 1;

			// This helps emulate the OnKey method that only reacts once per button press.
			lastFramePause = Pause;
		} else {
			//TODO: setup death screen and allow player to reset game (within scene)
			if (DeathScreen != null) {
				DeathScreen.SetActive(true);
				selectedObject = DeathScreen.GetComponentInChildren<Slider>().gameObject;
				Time.timeScale = 0.5f;
			}
		}
	}

	void OnDisable(){
		buttonSelected = false;
	}

	public void NewGameButton(){
		bool coinFlip = (Random.Range(0, 2) == 0);
		if (coinFlip) SceneManager.LoadScene("Generate_Infinite");
		else SceneManager.LoadScene("GenerateL0");
	}

	public void SliderControl(){

	}

	public void FreePlayButton(){
		SceneManager.LoadScene("Generate_Infinite");	
	}

	public void QuitGame(){
		Application.Quit();
	}

}
