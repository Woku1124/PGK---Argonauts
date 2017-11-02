using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotController : MonoBehaviour {

	public int damage;
	public int owner;
	public float lifeTime;

	private float startLife;

	// Use this for initialization
	void Start () {
		lifeTime = 1.0f;
		startLife = Time.time;
	}
	
	// Update is called once per frame
	void Update () {
		if (Time.time > startLife + lifeTime) {
			Destroy(gameObject);
		}
	}

	void OnTriggerEnter2D(Collider2D other) {
		// TODO ten if jest za duzy, do przerobienia
		if (other.gameObject.tag.Equals("Shot") || other.gameObject.tag.Equals("Asteroid")) {
			return;
		}
		// box colliders are used for actual ships and circle colliders for range
		if (other.GetType().ToString() == "UnityEngine.BoxCollider2D" && other.gameObject.GetComponent<Attributes>().owner != owner) {
			other.gameObject.GetComponent<Attributes>().hp -= damage;
			Destroy(gameObject);
		}
	}
}
