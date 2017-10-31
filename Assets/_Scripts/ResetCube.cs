using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResetCube : MonoBehaviour {
	static readonly string speed0 = "Press the W key to accelerate.\n[Press escape or the resume button below\nto continue through the rest of this tutorial]";
	static readonly string ramp0 = "Use the arrow keys while\nin the air to do tricks!";
	static readonly string spike0 = "Spikes will hurt you!\nPress the space bar to jump over them.\nTime your jumps exactly to grab all the orbs.";
	static readonly string wall0 = "Drive through walls to gain points!";
	static readonly string speed1 = "";
	static readonly string ramp1 = "";
	static readonly string spike1 = "";
	static readonly string wall1 = "";
	static readonly string speed2 = "";
	static readonly string ramp2 = "";
	static readonly string spike2 = "";
	static readonly string wall2 = "";
	private string[] scripts = {speed0, ramp0, spike0, wall0, speed1, ramp1, spike1, wall1, speed2, ramp2, spike2, wall2};
	private string[] inters = {"Speed Portal Level 0", "Ramp Level 0", "Spikes Level 0", "Wall Level 0", "Speed Portal Level 1", "Ramp Level 1", "Spikes Level 1", "Wall Level 1", "Speed Portal Level 1", "Ramp Level 1", "Spikes Level 1", "Wall Level 1"};
	int index = 0;
	public Text text;
	public Text text2;

	Vector3 startPos = new Vector3(275f, 7f, 18f);
	public Rigidbody Car;

	public UIController UI;

	void Start () {
		text.text = scripts[index];
		text2.text = string.Format("Interactable: {0}\n", inters[index]);
		text2.enabled = false;
		UI.PauseGame();
	}

	void Update(){
		text2.enabled = !UI.Paused;
	}
	
	void OnTriggerExit(Collider other){
		if (other.CompareTag("Player")) {
			startPos.x -= 50;
			Car.velocity = Vector3.zero;
			Car.rotation = Quaternion.identity;
			Car.transform.position = startPos;

			if (index < scripts.Length - 1) {
				UI.PauseGame();
				text.text = scripts[++index];
				text2.text = string.Format("Interactable: {0}\n", inters[index]);
			} else text2.text = text.text = "";
		}
	}
}
