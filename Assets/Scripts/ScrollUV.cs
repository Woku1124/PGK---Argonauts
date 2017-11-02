using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollUV : MonoBehaviour
{
    public float speed = 300.0f;

	void Update () {
	    UnityEngine.MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
	    Material mat = meshRenderer.sharedMaterial;

	    Vector2 offset = mat.mainTextureOffset;

	    offset.x += Time.deltaTime / speed;
	    mat.mainTextureOffset = offset;
	}
}
