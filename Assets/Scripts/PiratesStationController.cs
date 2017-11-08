using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PiratesStationController : MonoBehaviour {
	
	public bool isSelected;
	public bool isUnderAttack;
	public float Ore;

	private Attributes myAttributes;
	private int lastHp;
	private float lastAttackTime;

	// Use this for initialization
	void Start () {
		isUnderAttack = false;
		lastAttackTime = 0.0f;
		Ore = 200.0f;
		myAttributes = GetComponent<Attributes>();
		lastHp = myAttributes.hp;
	}
	
	// Update is called once per frame
	void Update () {
		if (myAttributes.hp < lastHp) {
			isUnderAttack = true;
			lastAttackTime = Time.time;
		} else if (Time.time - lastAttackTime > 3.0f) {
			isUnderAttack = false;
		}
		lastHp = myAttributes.hp;

		if (myAttributes.hp <= 0) {
			Destroy(gameObject);
		}

		transform.Rotate(Vector3.back * 2f * Time.deltaTime, Space.Self);
		GetComponent<SpriteRenderer>().color = Color.red;
	}
}
