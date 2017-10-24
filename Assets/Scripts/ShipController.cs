using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipController : MonoBehaviour {

	public float speed;
	public bool isSelected;

	public List<Vector3> occupiedPositions;

	private bool isMoving;
	private Vector3 movePosition;
	private GameController gameController;
	private Attributes myAttributes;

	// Use this for initialization
	void Start () {
		isMoving = false;
		isSelected = false;
		movePosition = new Vector3();
		occupiedPositions = new List<Vector3>();
		CalculateOccupiedPositions(transform.position);
		myAttributes = GetComponent<Attributes>();
		gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
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
			// we are moving
			isMoving = true;

			// calculationg mouse position in world
			Vector3 mousePosition3D = Camera.main.ScreenToWorldPoint(Input.mousePosition);

			// rounding because we want to end in grid
			movePosition.x = Mathf.Round(mousePosition3D.x);
			movePosition.y = Mathf.Round(mousePosition3D.y);

			// clear locked position in game controller
			occupiedPositions.ForEach(position => gameController.lockedPositions.Remove(position));
			// clear occupied positions
			occupiedPositions.Clear();

			// find spot as close to our click as possible but without any units there
			movePosition = FindFreeSpot(movePosition);
			// calculate direction based on movePosition
			CalculateDirection();

			// calculate new occupied position based on movePosition
			CalculateOccupiedPositions(movePosition);
			// add new locked positions to game controller based on occupiedPositions
			gameController.lockedPositions.AddRange(occupiedPositions);

			// change direction
			ChangeDirection();
		}
	}

	//TODO: It should be done better later.
	Vector3 FindFreeSpot(Vector3 mousePos) {
		Vector3 freeSpot = mousePos;

		if (isEnoughRoom(freeSpot) == false && freeSpot.Equals(transform.position) == false) {
			// temporary solution
			freeSpot.x++;
			freeSpot = FindFreeSpot(freeSpot);
		}
		return freeSpot;
	}


	void CalculateDirection() {
		if (movePosition.x >= transform.position.x && movePosition.y >= transform.position.y) {
			if (movePosition.x - transform.position.x > movePosition.y - transform.position.y) {
				myAttributes.direction = 1;
			} else {
				myAttributes.direction = 0;
			}
		} else if (movePosition.x >= transform.position.x && movePosition.y <= transform.position.y) {
			if (movePosition.x - transform.position.x > transform.position.y - movePosition.y) {
				myAttributes.direction = 1;
			} else {
				myAttributes.direction = 2;
			}
		} else if (movePosition.x <= transform.position.x && movePosition.y >= transform.position.y) {
			if (transform.position.x - movePosition.x > movePosition.y - transform.position.y) {
				myAttributes.direction = 3;
			} else {
				myAttributes.direction = 0;
			}
		} else if (movePosition.x <= transform.position.x && movePosition.y <= transform.position.y) {
			if (transform.position.x - movePosition.x > transform.position.y - movePosition.y) {
				myAttributes.direction = 3;
			} else {
				myAttributes.direction = 2;
			}
		}
	}
	// this is garbage but don't have time to make it better atm
	void ChangeDirection() {
		if (myAttributes.direction == 0) {
			transform.SetPositionAndRotation(transform.position, Quaternion.Euler(new Vector3(0.0f, 0.0f, 0.0f)));
		} else if (myAttributes.direction == 1) {
			transform.SetPositionAndRotation(transform.position, Quaternion.Euler(new Vector3(0.0f, 0.0f, 270.0f)));
		} else if (myAttributes.direction == 2) {
			transform.SetPositionAndRotation(transform.position, Quaternion.Euler(new Vector3(0.0f, 0.0f, 180.0f)));
		} else if (myAttributes.direction == 3) {
			transform.SetPositionAndRotation(transform.position, Quaternion.Euler(new Vector3(0.0f, 0.0f, 90.0f)));
		}
	}

	bool isEnoughRoom(Vector3 position) {
		bool enough = true;

		float horizontalSize;
		float verticalSize;

		if (myAttributes.direction == 0 || myAttributes.direction == 2) {
			horizontalSize = myAttributes.SizeX;
			verticalSize = myAttributes.SizeY;
		} else {
			horizontalSize = myAttributes.SizeY;
			verticalSize = myAttributes.SizeX;
		}

		float x = position.x - horizontalSize;
		float y = position.y - verticalSize;
		float finalX = position.x + horizontalSize;
		float finalY = position.y + verticalSize;

		for (int xx = (int)x; xx < finalX; xx++) {
			for (int yy = (int)y; yy < finalY; yy++) {
				gameController.lockedPositions.ForEach(lockedPos => {
					if (new Vector3(xx, yy, 0.0f).Equals(lockedPos)) {
						enough = false;
					}
				});
			}
		}
		return enough;
	}

	void CalculateOccupiedPositions(Vector3 position) {
		float x = position.x - GetComponent<Attributes>().SizeX;
		float y = position.y - GetComponent<Attributes>().SizeY;
		float finalX = position.x + GetComponent<Attributes>().SizeX;
		float finalY = position.y + GetComponent<Attributes>().SizeY;

		for (int xx = (int)x; xx < finalX; xx++) {
			for (int yy = (int)y; yy < finalY; yy++) {
				occupiedPositions.Add(new Vector3(xx, yy, 0.0f));
			}
		}
	}
}
