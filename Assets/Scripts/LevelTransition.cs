using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class LevelTransition : MonoBehaviour {

	//this whole thing is just for loading the next level.  We attach it to the goal.

    public string levelToLoad;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.name == "player")
        {
            SceneManager.LoadScene(levelToLoad);
        }
    }
}
