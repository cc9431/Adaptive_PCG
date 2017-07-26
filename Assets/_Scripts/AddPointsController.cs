using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddPointsController : MonoBehaviour {
	private static AddPoints addPoints;
	private static GameObject canvas;
	public static void Initialize(){
		canvas = GameObject.FindGameObjectWithTag("Canvas");
		if (!addPoints) addPoints = Resources.Load<AddPoints>("Prefabs/PointsParent");
	}

	public static void CreateText(string newText, Transform location){
		AddPoints instance = Instantiate(addPoints);
		instance.transform.SetParent(canvas.transform, false);
		instance.setText(newText);
	}
}
