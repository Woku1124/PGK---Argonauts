using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StationController : MonoBehaviour {

	public bool isSelected;
	public float Ore;

	private Attributes myAttributes;

	// Use this for initialization
	void Start () {
		myAttributes = GetComponent<Attributes>();
	}
	
	// Update is called once per frame
	void Update () {
		if (myAttributes.hp <= 0) {
			Destroy(gameObject);
		}

        transform.Rotate(Vector3.forward * 1f * Time.deltaTime, Space.Self);
    }
}
