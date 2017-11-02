using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackWallController : MonoBehaviour {
	MeshRenderer MR;
	Material mat;

	void Start(){
		MR = GetComponent<MeshRenderer>();
		mat = MR.material;
	}
	
	void LateUpdate () {
		float h, s, v;

		Color.RGBToHSV(mat.color, out h, out s, out v);
		h = (Time.timeSinceLevelLoad/100f) % 1;
		mat.color = Color.HSVToRGB(h, s, v);
	}
}
