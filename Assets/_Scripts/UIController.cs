﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class UIController : MonoBehaviour {
	private bool buttonSelected;
	private bool lastFramePause = false;
	private bool lastFrameAlive;
	private Transform car;

	public bool Paused;
	public EventSystem eventSystem;
	public GameObject selectedObject;
	public GameObject PauseScreen;
	public GameObject DeathScreen;
	public GameObject DeathScreenSelectedObject;
	public GameObject PauseScreenSelectedObject;
	private JSON JSONClass;
	private Slider slider;

	void OnEnable() {
		//SceneManager.sceneLoaded += OnSceneLoaded;
	}

	void OnDisable() {
		buttonSelected = false;
		//SceneManager.sceneLoaded -= OnSceneLoaded;
	}

	private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
		
	}

	void Awake(){
		if (PauseScreen != null) PauseScreen.SetActive(false);
		if (DeathScreen != null) DeathScreen.SetActive(false);

		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Confined;
	}

	void Start(){
		car = GameObject.FindGameObjectWithTag("Player").transform;
		eventSystem.SetSelectedGameObject(selectedObject);
		JSONClass = GetComponent<JSON>();
	}

	void Update(){
		if (Input.GetAxisRaw("Vertical") != 0 && !buttonSelected) {
			eventSystem.SetSelectedGameObject(selectedObject);
			buttonSelected = true;
		}

		if (Input.GetAxisRaw("Jump") != 0 || Input.GetKeyDown(KeyCode.Space)){
			GameObject button = eventSystem.currentSelectedGameObject;
			PointerEventData pointer = new PointerEventData(EventSystem.current);
			ExecuteEvents.Execute(button, pointer, ExecuteEvents.pointerClickHandler);
		}

		if (_CarController.Alive) {
			// Where we check if the player pauses the game or not
			bool Pause = (Input.GetAxisRaw("Submit") != 0);

			if (Pause && !lastFramePause) {
				Paused = !Paused;
				if (PauseScreen != null) PauseScreen.SetActive(Paused);
				Cursor.visible = Paused;
			}

			if (Paused) Time.timeScale = 0;
			if (!Paused) Time.timeScale = 1;

			// This helps emulate the OnKey method that only reacts once per button press.
			lastFramePause = Pause;
		} else {
			if (lastFrameAlive && DeathScreen != null) {
				if (Time.timeSinceLevelLoad < 300f) Restart();
				else {
					DeathScreen.SetActive(true);
					selectedObject = DeathScreenSelectedObject;
					Time.timeScale = 0.3f;
					buttonSelected = false;
				}
			}
		}

		lastFrameAlive = _CarController.Alive;
	}

	public void NewGameButton(){
		SceneManager.LoadScene("Generate_Infinite");
	}

	public void OnValueChange(){
		slider = eventSystem.currentSelectedGameObject.GetComponent<Slider>();
		_CarController.Keyboard = (slider.value == 1);

		//print(_CarController.Keyboard.ToString());
	}

	public void FreePlayButton(){
		SceneManager.LoadScene("FreePlay");
	}

	public void QuitGame(){
		Application.Quit();
	}

	public void Finish(){
		JSONClass.LogDeath();
		SceneManager.LoadScene("StartScene");
	}

	public void Restart(){
		Rigidbody carRB = car.gameObject.GetComponent<Rigidbody> ();

		GenerateInfiniteFull.Restart = true;
		
		car.position = new Vector3(0, 11, 0);
		car.rotation = Quaternion.identity;
		carRB.velocity = Vector3.zero;
		_CarController.Alive = true;

		DeathScreen.SetActive(false);
		selectedObject = PauseScreenSelectedObject;
		
		Time.timeScale = 1f;
		buttonSelected = false;
	}

	public void Resume(){
		Paused = false;
		PauseScreen.SetActive(false);
	}

}
