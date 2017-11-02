using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResetCube : MonoBehaviour {
	public bool TutorialScene;
	static readonly string speed0 = "Ahead is a speed boost.\n Drive through it and into the back wall\nto continue to the next part\nof the tutorial.\nTo accelerate use the W key.\nPress the Escape button\nto pause and unpause the game at any time.";
	static readonly string ramp0 = "Use the arrow keys while\nin the air to do tricks!\nPractice this after jumping off\nof the ramp ahead of you.";
	static readonly string spike0 = "Spikes will hurt you!\nPress the space bar to jump over them.\nTime your jumps exactly to grab all the orbs.";
	static readonly string wall0 = "Drive through walls to gain points!\nYou can also try jumping through the wall\nand doing a trick to get extra points.";
	static readonly string speed1 = "This speed boost is in the air.\nYou have to jump to get the orb inside!\nAdvanced Technique:\nTry angling your car up by tapping the\ndown arrow while jumping through the\nbooster to shoot yourself into the air!";
	static readonly string ramp1 = "Tricks are rewarded for full spins, or full flips.\nIn order to get Perfect Trick points\nmake sure the body of the car doesn't\ntouch the ground during a trick!";
	static readonly string spike1 = "Try doing a trick while jumping\nover the spikes ahead to get extra points!";
	static readonly string wall1 = "This wall is bigger and gives you\neven more points for driving through it.";
	static readonly string speed2 = "This speed boost is the hardest to hit!\nTime your jump perfectly and you will\nget a ton of boost!";
	static readonly string ramp2 = "This ramp has a kick to it.\nKeep your speed in the orange to\nget all the orbs while still giving\nyourself a lot of air!";
	static readonly string spike2 = "This is the largest spike pit you will see.\nTry spining with the left or right arrow keys\nwhile jumping over it to get all of the orbs.";
	static readonly string wall2 = "This is the largest wall there is!\nPop off of the little ramp in front of it to\ndestroy it and get the points!";
	static readonly string freeplay = "You have now seen all of the interactable\nobjects in the game. Feel free to mess around\nhere while the timer runs out\nor quit back to the main menu if you\nare ready to start the testing!";
	private string[] scripts = {speed0, ramp0, spike0, wall0, speed1, ramp1, spike1, wall1, speed2, ramp2, spike2, wall2, freeplay};
	private string[] inters = {"Speed Portal Level 0", "Ramp Level 0", "Spikes Level 0", "Wall Level 0", "Speed Portal Level 1", "Ramp Level 1", "Spikes Level 1", "Wall Level 1", "Speed Portal Level 1", "Ramp Level 1", "Spikes Level 1", "Wall Level 1"};
	public int index;
	public Text text;
	public Text text2;

	Vector3 startPos;
	public Rigidbody Car;

	public UIController UI;

	void Start () {
		if (TutorialScene) {
			startPos = new Vector3(275f, 7f, 18f);
			text.text = scripts[index];
			text2.text = index < inters.Length - 1 ? string.Format("Interactable: {0}\n", inters[index]): "";
			text2.enabled = false;
			UI.PauseGame();
		} else {
			startPos = Car.position;
		}
	}

	void Update(){
		if (TutorialScene) text2.enabled = !UI.Paused;
	}
	
	void OnTriggerExit(Collider other){
		if (other.CompareTag("Player")) {
			if (TutorialScene){
				startPos.x -= 50;
				Car.velocity = Vector3.zero;
				Car.angularVelocity = Vector3.zero;
				Car.rotation = Quaternion.identity;
				Car.transform.position = startPos;
				text2.text = index < inters.Length - 1 ? string.Format("Interactable: {0}\n", inters[index]): "";
				text.text = index < scripts.Length - 1 ? scripts[++index]: "";
				UI.PauseGame();
			} else {
				Car.velocity = Vector3.zero;
				Car.angularVelocity = Vector3.zero;
				Car.rotation = Quaternion.identity;
				Car.transform.position = startPos;
			}
		}
	}
}
