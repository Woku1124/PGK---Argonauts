using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollUV : MonoBehaviour {

	void Update ()
	{

	    UnityEngine.MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
	    Material mat = meshRenderer.sharedMaterial;

	    Vector2 offset = mat.mainTextureOffset;

	    offset.x += Time.deltaTime / 300f;
	    mat.mainTextureOffset = offset;
	}
}
