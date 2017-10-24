﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PiratesFighterController : MonoBehaviour {
	
	public GameObject shot;
	public float speed;
	public List<Vector3> occupiedPositions;

	private bool isAttacking;
	private float nextFire;
	private Attributes myAttributes;
	private GameController gameController;
	private GameObject attackingTarget;
	private float shotMomentum = 2000.0f;

	// Use this for initialization
	void Start () {
		isAttacking = false;
		occupiedPositions = new List<Vector3>();
		myAttributes = GetComponent<Attributes>();
		gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
		CalculateOccupiedPositions(transform.position);
	}
	
	// Update is called once per frame
	void Update () {
		if (myAttributes.hp <= 0) {
			occupiedPositions.ForEach(position => gameController.lockedPositions.Remove(position));
			gameController.allUnits.Remove(gameObject);
			Destroy(gameObject);
		}

		if (isAttacking && Time.time > nextFire) {
			nextFire = Time.time + myAttributes.reloadTime;
			GameObject newShot = Instantiate(shot, transform.position, transform.rotation);
			// temporary
			newShot.transform.localScale = new Vector3(3.0f*myAttributes.reloadTime, 3.0f*myAttributes.reloadTime, 3.0f*myAttributes.reloadTime);
			newShot.GetComponent<ShotController>().damage = myAttributes.damage;
			newShot.GetComponent<ShotController>().owner = myAttributes.owner;
			Vector2 force = new Vector2(attackingTarget.transform.position.x - transform.position.x,
				attackingTarget.transform.position.y - transform.position.y);
			force.Normalize();
			force = new Vector2(force.x * shotMomentum, force.y * shotMomentum);
			newShot.GetComponent<Rigidbody2D>().AddForce(force);
		}
	}

	void OnTriggerStay2D(Collider2D other) {
		if (other.gameObject.tag.Equals("Shot")) {
			return;
		}
		// box colliders are used for actual ships and circle colliders for range
		if (other.GetType().ToString() == "UnityEngine.BoxCollider2D" && other.gameObject.GetComponent<Attributes>().owner != myAttributes.owner) {
			isAttacking = true;
			attackingTarget = other.gameObject;
		}
	}

	void OnTriggerExit2D(Collider2D other) {
		if (other.gameObject.tag.Equals("Shot")) {
			return;
		}
		// box colliders are used for actual ships and circle colliders for range
		if (other.GetType().ToString() == "UnityEngine.BoxCollider2D" && other.gameObject.GetComponent<Attributes>().owner != myAttributes.owner) {
			isAttacking = false;
		}
	}

	void CalculateOccupiedPositions(Vector3 position) {
		float x = position.x - myAttributes.SizeX;
		float y = position.y - myAttributes.SizeY;
		float finalX = position.x + myAttributes.SizeX;
		float finalY = position.y + myAttributes.SizeY;

		for (int xx = (int)x; xx < finalX; xx++) {
			for (int yy = (int)y; yy < finalY; yy++) {
				occupiedPositions.Add(new Vector3(xx, yy, 0.0f));
			}
		}
	}
}
