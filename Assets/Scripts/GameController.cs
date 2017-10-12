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
		// if left mouse button down
		if (Input.GetMouseButtonDown(0)) {
			// then select units
			SelectUnits();
		}
	}

	void SelectUnits() {
		// calculationg mouse position in world
		Vector3 mousePosition3D = Camera.main.ScreenToWorldPoint(Input.mousePosition);

		// checking all units coordinates in order to select the right ones
		allUnits.ForEach(unit => {
			if (Mathf.Abs(mousePosition3D.x - unit.transform.position.x) < 0.5f && 
				Mathf.Abs(mousePosition3D.y - unit.transform.position.y) < 0.5f) {
				unit.GetComponent<FighterController>().isSelected = true;
				// temporary
				unit.GetComponent<SpriteRenderer>().color = Color.green;
			} else {
				unit.GetComponent<FighterController>().isSelected = false;
				// temporary
				unit.GetComponent<SpriteRenderer>().color = Color.white;
			}
		});
	}
}
