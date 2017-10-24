using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PiratesFighterController : MonoBehaviour {

	public float speed;
	public bool isSelected;
	public List<Vector3> occupiedPositions;
	private GameController gameController;
	// Use this for initialization
	void Start () {
		occupiedPositions = new List<Vector3>();
		CalculateOccupiedPositions(transform.position);

	}
	
	// Update is called once per frame
	void Update () {

		
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
