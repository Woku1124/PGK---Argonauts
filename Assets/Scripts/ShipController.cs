using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipController : MonoBehaviour {

	public float speed;
	public bool isSelected;
	public GameObject shot;
	[HideInInspector]
	public List<Vector3> occupiedPositions;

	private float shotMomentum = 2000.0f;
	private bool isMoving;
	private bool isAttacking;
	private float nextFire;
	private Vector3 movePosition;
	private GameController gameController;
	private GameObject attackingTarget;
	private Attributes myAttributes;

	// Use this for initialization
	void Start () {
		isMoving = false;
		isAttacking = false;
		isSelected = false;
		movePosition = new Vector3();
		occupiedPositions = new List<Vector3>();
		CalculateOccupiedPositions(transform.position);
		myAttributes = GetComponent<Attributes>();
		gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
	}

	// Update is called once per frame
	void Update() {
		if (myAttributes.hp <= 0) {
			occupiedPositions.ForEach(position => gameController.lockedPositions.Remove(position));
			gameController.allUnits.Remove(gameObject);
			Destroy(gameObject);
		}

		// if this unit is attacking
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

	void OnTriggerStay2D(Collider2D other) {
		// TODO ten if jest za duzy, do przerobienia
		if (other.gameObject.tag.Equals("Shot") || other.gameObject.tag.Equals("Asteroid") || gameObject.tag.Equals("Unit3")) {
			return;
		}
		// box colliders are used for actual ships and circle colliders for range
		if (other.GetType().ToString() == "UnityEngine.BoxCollider2D" && other.gameObject.GetComponent<Attributes>().owner != myAttributes.owner) {
			isAttacking = true;
			attackingTarget = other.gameObject;
		}
	}

	void OnTriggerExit2D(Collider2D other) {
		// TODO ten if jest za duzy, do przerobienia
		if (other.gameObject.tag.Equals("Shot") || other.gameObject.tag.Equals("Asteroid") || gameObject.tag.Equals("Unit3")) {
			return;
		}
		// box colliders are used for actual ships and circle colliders for range
		if (other.GetType().ToString() == "UnityEngine.BoxCollider2D" && other.gameObject.GetComponent<Attributes>().owner != myAttributes.owner) {
			isAttacking = false;
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

	// TODO: Optymalizacja (teraz to bardzo wolno dziala)
	Vector3 FindFreeSpot(Vector3 mousePos) {
		Vector3 freeSpot = mousePos;

		int iteration = 0;
		int translation = 0;
		int sign = 0;
		while (isEnoughRoom(freeSpot) == false && freeSpot.Equals(transform.position) == false) {
			int finalTranslation;

			if (iteration % 2 == 0) {
				translation++;
			}
			finalTranslation = (translation % 2 != 0) ? translation : -translation;
			sign = (translation % 2 != 0) ? 1 : -1;

			if (iteration % 2 == 0) {
				for (int i = 0; i != finalTranslation; i += sign) {
					freeSpot.y += i;
					if (isEnoughRoom(freeSpot) || freeSpot.Equals(transform.position))
						return freeSpot;
				}
			} else {
				for (int i = 0; i != finalTranslation; i += sign) {
					freeSpot.x += i;
					if (isEnoughRoom(freeSpot) || freeSpot.Equals(transform.position))
						return freeSpot;
				}
			}
			iteration++;
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
				if (gameController.lockedPositions.Contains(new Vector3(xx, yy, 0.0f))) {
					return false;
				}
			}
		}
		return true;
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
