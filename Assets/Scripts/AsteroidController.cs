using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidController : MonoBehaviour {
	public float speed;
	public Rigidbody2D r;


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
		//transform.Rotate(Vector3.forward * 20f * Time.deltaTime, Space.Self);

	}
	void FixedUpdate () {

		transform.Translate (transform.up * speed * Time.deltaTime);
		}
}
