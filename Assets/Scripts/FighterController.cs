using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FighterController : MonoBehaviour {

	public float speed;
	public bool isSelected;

	private bool isMoving;
	private Vector3 movePosition;
	private GameObject gameController;

	// Use this for initialization
	void Start () {
		isMoving = false;
		isSelected = false;
		movePosition = new Vector3();
		gameController = GameObject.FindGameObjectWithTag("GameController");
	}
	
	// Update is called once per frame
	void Update() {
		// if this unit is Selected
		if (isSelected) {
			// then we are calculating movement
			CalculateMovement();
		}

		// if we are moving and our position is the same as our destination
		if (isMoving && movePosition.Equals(transform.position)) {
			// that means we are not moving anymore
			isMoving = false;
		}
	}

	void FixedUpdate () {
		if (isMoving) {
			transform.position = Vector2.MoveTowards(transform.position, movePosition, speed * Time.deltaTime);
		}
	}

	void CalculateMovement() {
		// if right mouse button down
		if (Input.GetMouseButtonDown(1)) {
			// then we are moving
			isMoving = true;

			// calculationg mouse position in world
			Vector3 mousePosition3D = Camera.main.ScreenToWorldPoint(Input.mousePosition);

			// rounding because we want to end in grid
			movePosition.x = Mathf.Round(mousePosition3D.x);
			movePosition.y = Mathf.Round(mousePosition3D.y);

			// find spot as close to our click as possible but without any units there
			movePosition = FindFreeSpot(movePosition);
		}
	}

	Vector3 FindFreeSpot(Vector3 mousePos) {
		Vector3 freeSpot = mousePos;
		gameController.GetComponent<GameController>().allUnits.ForEach(unit => {
			if (unit.transform.position.Equals(freeSpot) && unit.Equals(gameObject) == false) {
				freeSpot.x++;
				freeSpot = FindFreeSpot(freeSpot);
			}
		});
		return freeSpot;
	}
}
