using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PiratesStationController : MonoBehaviour {
	public bool isSelected;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		transform.Rotate(Vector3.back * 2f * Time.deltaTime, Space.Self);
		GetComponent<SpriteRenderer>().color = Color.red;
	}
}
