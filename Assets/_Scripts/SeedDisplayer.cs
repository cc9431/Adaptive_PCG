using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SeedDisplayer : MonoBehaviour {
	public Text text;
	
	void Start(){
		int seed = PlayerPrefs.GetInt("Seed");
		text.text = seed.ToString();
	}

}
