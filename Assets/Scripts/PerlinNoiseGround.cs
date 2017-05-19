using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PerlinNoiseGround : MonoBehaviour {
	public float power = 3.0f;
	public float scale = 1.0f;
	private Vector3 v2SampleStart = new Vector3(0f, 0f, 0f);

	void Start () {
		v2SampleStart = new Vector3(10f, Random.Range (0.0f, 50.0f), 10f);
		MeshFilter mf = GetComponent<MeshFilter>();
		Vector3[] vertices = mf.mesh.vertices;
		for (int i = 0; i < vertices.Length; i++) {    
			float xCoord = v2SampleStart.x + vertices[i].x / scale;
			float zCoord = v2SampleStart.z + vertices[i].z  / scale;
			vertices[i].y = (Mathf.PerlinNoise (xCoord, zCoord)) / power; 
		}
		mf.mesh.vertices = vertices;
		mf.mesh.RecalculateBounds();
		mf.mesh.RecalculateNormals();
	}
} 