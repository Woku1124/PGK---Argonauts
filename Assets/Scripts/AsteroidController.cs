using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidController : MonoBehaviour {
<<<<<<< HEAD
	public float speed;
	public Rigidbody2D r;

=======
>>>>>>> 5e27f67fff9dff26ef9cdd5e313b28da25433a4c

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
<<<<<<< HEAD
		
		//transform.Rotate(Vector3.forward * 20f * Time.deltaTime, Space.Self);

	}
	void FixedUpdate () {

		transform.Translate (transform.up * speed * Time.deltaTime);
		}
=======
        transform.Rotate(Vector3.forward * 5f * Time.deltaTime, Space.Self);
    }
>>>>>>> 5e27f67fff9dff26ef9cdd5e313b28da25433a4c
}
