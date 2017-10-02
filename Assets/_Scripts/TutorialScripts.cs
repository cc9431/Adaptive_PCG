using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialScripts : MonoBehaviour {
	static string thankYou = "Thank you for agreeing to participate\n in this research. Please read the controls\n and instructions carefully, then press return\n to continue through the rest of this tutorial.";
	static string empty = "";
	static string tricks = "Take some time and try performing some tricks.\n Press the space bar to jump.\n While in the air, use the arrow keys to spin.\n To get a perfect trick, do not let the car body\n touch the ground while doing the trick.";

	private Text text;
	private string[] scripts = {thankYou, empty, tricks, empty};
	public int index = 0;

	void Start () {
		text = GetComponent<Text>();
		text.text = scripts[index];
	}
	
	void Update () {
		if (Input.GetKeyDown(KeyCode.Return)) text.text = scripts[(++index % scripts.Length)];
	}
}
