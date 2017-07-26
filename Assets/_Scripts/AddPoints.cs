using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class AddPoints : MonoBehaviour {
	public Animator animator;
	private Text addPointText;

	void Awake () {
		addPointText = animator.GetComponent<Text>();
		AnimatorClipInfo[] clipInfo = animator.GetCurrentAnimatorClipInfo(0);
		Destroy(gameObject, clipInfo[0].clip.length);
	}
	
	public void setText(string newText){
		addPointText.text = '+'.ToString() + newText;
	}
}
