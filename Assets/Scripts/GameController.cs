using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {

	public List<GameObject> allUnits;
	public List<Vector3> movePositions;

	// Use this for initialization
	void Start () {
		allUnits = new List<GameObject>();
		movePositions = new List<Vector3>();
		// not sure if GameController Start method will always run AFTER creation of all GameObjects
		allUnits.AddRange(GameObject.FindGameObjectsWithTag("Unit"));
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown(0)) {
			SelectUnits();
		}
	}

	void SelectUnits() {
		allUnits.ForEach(unit => {
			Vector3 mousePosition3D = Camera.main.ScreenToWorldPoint(Input.mousePosition);

			if (Mathf.Abs(mousePosition3D.x - unit.transform.position.x) < 0.5f && 
				Mathf.Abs(mousePosition3D.y - unit.transform.position.y) < 0.5f) {
				unit.GetComponent<FighterController>().isSelected = true;
				unit.GetComponent<SpriteRenderer>().color = Color.green;
			} else {
				unit.GetComponent<FighterController>().isSelected = false;
				unit.GetComponent<SpriteRenderer>().color = Color.white;
			}
		});
	}
}
