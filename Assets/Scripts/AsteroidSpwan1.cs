using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidSpwan1 : MonoBehaviour {

	public Vector2 pos;
	public GameObject asteroid;

	// Use this for initialization
	void Start () {
		Spawn();
	}

	// Update is called once per frame
	void Update () {

	}

	void Spawn() {
		Vector2 p= new Vector2(Random.Range(-pos.x,pos.x),Random.Range(-pos.y,pos.y));
		Instantiate(asteroid,p,Quaternion.identity);
	}
}