using System.Collections;
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

	public MasterController master;

	public bool Paused;
	public EventSystem eventSystem;
	public GameObject selectedObject;
	public GameObject PauseScreen;
	public GameObject TimeScreen;
	public GameObject PauseScreenSelectedObject;
	public GameObject TimeScreenSelectedObject;
	private StatJSON JSONClass;
	private Slider slider;
	string[] sceneList = {"StartScene", "Part1", "Part2", "Part3"};
	static int sceneIndex = 0;

	void OnEnable() {
		SceneManager.sceneLoaded += OnSceneLoaded;
	}	

	void OnDisable() {
		SceneManager.sceneLoaded -= OnSceneLoaded;
		buttonSelected = false;
	}

	private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
		// Reinitialize a whole bunch of public static variables
	}

	void Awake(){
		if (PauseScreen != null) PauseScreen.SetActive(false);
		if (TimeScreen != null) TimeScreen.SetActive(false);

		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Confined;
	}

	void Start(){
		car = GameObject.FindGameObjectWithTag("Player").transform;
		eventSystem.SetSelectedGameObject(selectedObject);
		JSONClass = GetComponent<StatJSON>();
		
		slider = eventSystem.currentSelectedGameObject.GetComponent<Slider>();
		slider.value = PlayerPrefs.GetFloat("slider");
	}

	void Update(){
		if (Input.GetAxisRaw("Vertical") != 0 && !buttonSelected) {
			eventSystem.SetSelectedGameObject(selectedObject);
			buttonSelected = true;
		}

		if (Input.GetAxisRaw("Jump") != 0 || Input.GetKeyDown(KeyCode.Return)){
			GameObject button = eventSystem.currentSelectedGameObject;
			PointerEventData pointer = new PointerEventData(EventSystem.current);
			ExecuteEvents.Execute(button, pointer, ExecuteEvents.pointerClickHandler);
		}

		if (_CarController.Alive && !_CarController.TimedOut) {
			// Where we check if the player pauses the game or not
			bool Pause = (Input.GetAxisRaw("Submit") != 0);

			if (Pause && !lastFramePause) {
				PauseGame();
			}

			// This helps emulate the OnKey method that only reacts once per button press.
			lastFramePause = Pause;
		} else {
			if (lastFrameAlive) {
				if (_CarController.TimedOut){
					TimeScreen.SetActive(true);
					selectedObject = TimeScreenSelectedObject;
					Time.timeScale = 0.2f;
					buttonSelected = false;
				} else Restart();
			}
		}

		lastFrameAlive = _CarController.Alive;
	}

	public void PauseGame(){
		Cursor.visible = Paused = !Paused;
		if (PauseScreen != null) PauseScreen.SetActive(Paused);
		if (Paused) Time.timeScale = 0;
		else Time.timeScale = 1;
	}

	public void NewGameButtonA(){
		setSeed();
		PlayerPrefs.SetInt("ADAPT", 0);
		sceneIndex++;
		SceneManager.LoadScene(sceneList[sceneIndex]);
	}
	public void NewGameButtonB(){
		setSeed();
		PlayerPrefs.SetInt("ADAPT", 1);
		sceneIndex++;
		SceneManager.LoadScene(sceneList[sceneIndex]);
	}
	public void NextScene(){
		JSONClass.DataDump(SceneManager.GetActiveScene().name);
		sceneIndex++;
		if (sceneList[sceneIndex] == "Part3") PlayerPrefs.SetInt("ADAPT", 0);
		SceneManager.LoadScene(sceneList[sceneIndex]);
	}

	public void Tutorial(){
		SceneManager.LoadScene("Tutorial");
	}

	public void OnValueChange(){
		//slider = eventSystem.currentSelectedGameObject.GetComponent<Slider>();
		_CarController.Keyboard = (slider.value == 0);

		PlayerPrefs.SetFloat("slider", slider.value);
	}

	public void QuitGame(){
		Application.Quit();
	}

	public void Finish(){
		JSONClass.DataDump(SceneManager.GetActiveScene().name);
		sceneIndex = 0;
		SceneManager.LoadScene(sceneList[sceneIndex]);
	}

	public void QuitPractice(){
		sceneIndex = 0;
		SceneManager.LoadScene(sceneList[sceneIndex]);
	}

	public void Restart(){
		Rigidbody carRB = car.gameObject.GetComponent<Rigidbody> ();

		GenerateInfiniteFull.Restart = true;

		master.playerDied();
		
		car.position = new Vector3(0, 11, 0);
		car.rotation = Quaternion.identity;
		carRB.velocity = Vector3.zero;
		_CarController.Alive = true;

		selectedObject = PauseScreenSelectedObject;
		
		Time.timeScale = 1f;
		buttonSelected = false;
	}

	public void Resume(){
		PauseGame();
	}

	private void setSeed(){
		int seed = Mathf.Abs(System.Environment.TickCount);
		PlayerPrefs.SetInt("Seed", seed);
	}

}
