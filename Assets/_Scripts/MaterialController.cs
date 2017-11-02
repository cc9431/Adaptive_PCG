using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialController : MonoBehaviour {
	public List<Material> Materials;
	private Material[] mat = new Material[1];
	private int pos = 0;
	private bool lastFrameSkin;
	
	void Update () {
		bool Skin = (Input.GetAxis("Skin") != 0);
		if (Skin && !lastFrameSkin) freshPaint();

		lastFrameSkin = Skin;
	}

	public void freshPaint(){
		pos = (pos + 1) % 4;
		mat[0] = Materials[pos];

		GetComponent<Renderer>().materials = mat;
	}
}
