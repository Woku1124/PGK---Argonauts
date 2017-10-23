using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StationController : MonoBehaviour {

	public bool isSelected;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
        transform.Rotate(Vector3.forward * 1f * Time.deltaTime, Space.Self);
    }
}
