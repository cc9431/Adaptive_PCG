using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimerScript : MonoBehaviour {
	private Text timer;
	private Shadow shadow;
	private float startTime;
	private float colorChangeTime = -40;
	void Start () {
		startTime = Time.time;
		timer = GetComponent<Text>();
		shadow = GetComponent<Shadow>();
	}
	
	// Update is called once per frame
	void Update () {
		float t = Time.time - startTime;
		float countDown = Mathf.Clamp(601f - t, 0, 601);

		string min = ((int) countDown / 60).ToString();
		string sec = ((int) countDown % 60).ToString("d2");

		if ((t - colorChangeTime) >= 30) ChangeColor(countDown);
		
		timer.text = string.Format("{0}:{1}", min, sec);
	}

	void ChangeColor(float t){
		// Change the color of the shadow based on how much time is left
		colorChangeTime = Time.time - startTime;
		float h, s, v;

		Color.RGBToHSV(shadow.effectColor, out h, out s, out v);
		h = t/800f;
		shadow.effectColor = Color.HSVToRGB(h, s, v);

		// Debugging
		//print(h);
		//print(s);
		//print(v);
	}
}
