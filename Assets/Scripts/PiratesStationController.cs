using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PiratesStationController : MonoBehaviour {
	
	public bool isSelected;
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

		transform.Rotate(Vector3.back * 2f * Time.deltaTime, Space.Self);
		GetComponent<SpriteRenderer>().color = Color.red;
	}
}
