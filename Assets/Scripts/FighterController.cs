using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FighterController : MonoBehaviour {

	public float speed;

	private bool isMoving;
	private Vector3 movePosition;

	// Use this for initialization
	void Start () {
		isMoving = false;
		movePosition = new Vector3();
	}
	
	// Update is called once per frame
	void Update() {
		if (Input.GetMouseButtonDown(1)) {
			isMoving = true;

			Vector3 mousePosition3D = Camera.main.ScreenToWorldPoint(Input.mousePosition);

			// rounding because we want to end in grid
			movePosition.x = Mathf.Round(mousePosition3D.x);
			movePosition.y = Mathf.Round(mousePosition3D.y);
		}

		if (isMoving && movePosition.Equals(transform.position)) {
			isMoving = false;
		}
	}

	void FixedUpdate () {
		if (isMoving) {
			transform.position = Vector2.MoveTowards(transform.position, movePosition, speed * Time.deltaTime);
		}
	}
}
