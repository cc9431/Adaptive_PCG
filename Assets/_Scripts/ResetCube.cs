using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResetCube : MonoBehaviour {
	static readonly string thankYou = "Thank you for agreeing to participate\nin this research. Please read the controls\nand instructions carefully, then press escape\nor the resume button below\nto continue through the rest of this tutorial.";
	static readonly string ramp = "Take some time and try performing some tricks.\n Press the space bar to jump.\n While in the air, use the arrow keys to spin.\n To get a perfect trick, do not let the car body\n touch the ground while doing the trick.";
	private string[] scripts = {thankYou, ramp, "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a"};
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

			UI.PauseGame();
			if (index < inters.Length - 1) {
				text.text = scripts[++index];
				text2.text = string.Format("Interactable: {0}\n", inters[index]);
			} else text2.text = text.text = "";
		}
	}
}
