using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowUV : MonoBehaviour
{

    public float parralax = 20f;

	void Update ()
	{

	    UnityEngine.MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
	    Material mat = meshRenderer.sharedMaterial;

	    Vector2 offset = mat.mainTextureOffset;

	    offset.x = transform.position.x /transform.localScale.x/ parralax;
	    offset.y = transform.position.y / transform.localScale.y / parralax;
	    mat.mainTextureOffset = offset;
	}
}
