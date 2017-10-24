using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {

	public GameObject selectRectanglePrefab;
	public List<GameObject> allUnits;
	public List<Vector3> lockedPositions;

	private bool isSelecting;
	private Vector3 startSelectionMousePosition;
	private Vector3 endSelectionMousePosition;
	private Vector3 startSelectRectanglePosition;
	private Vector3 endSelectRectanglePosition;
	private GameObject selectRectangle;
	private GameObject spaceStation;
    private GameObject unit;

    public StationController SS;

	public List<GameObject> enemyUnits;
	private GameObject piratesStation;

    private float InterfaceX;
    private float InterfaceY;
    private float InterfaceWidth;
    private float InterfaceHeight;


    // Use this for initialization
    void Start () {

		isSelecting = false;

		allUnits = new List<GameObject>();
		enemyUnits = new List<GameObject>();

		lockedPositions = new List<Vector3>();
		// not sure if GameController Start method will always run AFTER creation of all GameObjects
		allUnits.AddRange(GameObject.FindGameObjectsWithTag("Unit"));
        allUnits.AddRange(GameObject.FindGameObjectsWithTag("Unit1"));
        allUnits.AddRange(GameObject.FindGameObjectsWithTag("Unit2"));
        allUnits.AddRange(GameObject.FindGameObjectsWithTag("Unit3"));

		enemyUnits.AddRange(GameObject.FindGameObjectsWithTag("Enemy"));
		spaceStation = GameObject.FindGameObjectWithTag("SpaceStation");
		piratesStation = GameObject.FindGameObjectWithTag("EnemyStation");

		// lock units positions
		allUnits.ForEach(unit => { 
			unit.GetComponent<ShipController>().occupiedPositions.ForEach(position => {
				lockedPositions.Add(position);
			});
		});
		enemyUnits.ForEach(unit => { 
			unit.GetComponent<PiratesFighterController>().occupiedPositions.ForEach(position => {
				lockedPositions.Add(position);
			});
		});

		// lock space station positions
		float x = spaceStation.transform.position.x - spaceStation.GetComponent<Attributes>().SizeX;
		float y = spaceStation.transform.position.y - spaceStation.GetComponent<Attributes>().SizeY;
		float finalX = spaceStation.transform.position.x + spaceStation.GetComponent<Attributes>().SizeX;
		float finalY = spaceStation.transform.position.y + spaceStation.GetComponent<Attributes>().SizeY;

		for (int xx = (int)x; xx < finalX; xx++) {
			for (int yy = (int)y; yy < finalY; yy++) {
				lockedPositions.Add(new Vector3(xx, yy, 0.0f));
			}
		}

		// lock pirate space station positions
		x = piratesStation.transform.position.x - piratesStation.GetComponent<Attributes>().SizeX;
		y = piratesStation.transform.position.y - piratesStation.GetComponent<Attributes>().SizeY;
		finalX = piratesStation.transform.position.x + piratesStation.GetComponent<Attributes>().SizeX;
		finalY = piratesStation.transform.position.y + piratesStation.GetComponent<Attributes>().SizeY;

		for (int xx = (int)x; xx < finalX; xx++) {
			for (int yy = (int)y; yy < finalY; yy++) {
				lockedPositions.Add(new Vector3(xx, yy, 0.0f));
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		// if left mouse button down
		if (Input.GetMouseButtonDown(0)) {
			// then start selecting
			isSelecting = true;
			// calculationg mouse position in world
			startSelectionMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			startSelectionMousePosition.z = 0.0f;

			selectRectangle = GameObject.Instantiate(selectRectanglePrefab, startSelectionMousePosition, new Quaternion());
		}
		if (isSelecting) {
			// calculationg mouse position in world
			endSelectionMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			// calculating select rectangle coordinates
			CalculateSelectRectangle();

			// calculating new scale for select rectangle
			Vector3 newScale = new Vector3(endSelectRectanglePosition.x - startSelectRectanglePosition.x,
										   endSelectRectanglePosition.y - startSelectRectanglePosition.y,
										   1.0f);
			
			// calculating new position for select rectangle
			Vector3 newPosition = new Vector3((endSelectRectanglePosition.x + startSelectRectanglePosition.x) * 0.5f,
				                      		  (endSelectRectanglePosition.y + startSelectRectanglePosition.y) * 0.5f,
				                      		  0.0f);
			
			selectRectangle.transform.localScale = newScale;
			selectRectangle.transform.localPosition = newPosition;

			if (Input.GetMouseButtonUp(0)) {
				// then select units
				SelectObjects();

				isSelecting = false;
				GameObject.Destroy(selectRectangle);
			}
		}

	}

	void SelectObjects() {
		bool isAnyUnitSelected = false;
		// checking all units coordinates in order to select the right ones
		allUnits.ForEach(unit => {
			if (IsInSelectRectangle(unit)) {
				unit.GetComponent<ShipController>().isSelected = true;
				// temporary
				unit.GetComponent<SpriteRenderer>().color = Color.green;

				isAnyUnitSelected = true;

				// we need to deselect space station in case if there is station selected
				DeselectSpaceStation();
			} else {
				unit.GetComponent<ShipController>().isSelected = false;
				// temporary
				unit.GetComponent<SpriteRenderer>().color = Color.white;
			}
		});

		// if there are no units selected
		if (isAnyUnitSelected == false) {
			// then check if there is space station in select rectangle
			if (IsInSelectRectangle(spaceStation)) {
				spaceStation.GetComponent<StationController>().isSelected = true;
				// temporary
				spaceStation.GetComponent<SpriteRenderer>().color = Color.green;
			} else {
				DeselectSpaceStation();
			}
		}
	}

	void DeselectSpaceStation() {
		spaceStation.GetComponent<StationController>().isSelected = false;
		// temporary
		spaceStation.GetComponent<SpriteRenderer>().color = Color.grey;
	}

	void CalculateSelectRectangle() {
		float greaterX, greaterY, lowerX, lowerY;
		greaterX = Mathf.Max(startSelectionMousePosition.x, endSelectionMousePosition.x);
		greaterY = Mathf.Max(startSelectionMousePosition.y, endSelectionMousePosition.y);
		lowerX = Mathf.Min(startSelectionMousePosition.x, endSelectionMousePosition.x);
		lowerY = Mathf.Min(startSelectionMousePosition.y, endSelectionMousePosition.y);
		// left bottom corner of select rectangle
		startSelectRectanglePosition.Set(lowerX, lowerY, 0.0f);
		// right top corner of select rectangle
		endSelectRectanglePosition.Set(greaterX, greaterY, 0.0f);
	}

	bool IsInSelectRectangle(GameObject obj) {
		int objDirection = obj.GetComponent<Attributes>().direction;
		float objSizeX;
		float objSizeY;
		if (objDirection == 0 || objDirection == 2) {
			objSizeX = obj.GetComponent<Attributes>().SizeX;
			objSizeY = obj.GetComponent<Attributes>().SizeY;
		} else {
			objSizeX = obj.GetComponent<Attributes>().SizeY;
			objSizeY = obj.GetComponent<Attributes>().SizeX;
		}

		Vector3 leftBottom = new Vector3(obj.transform.position.x - objSizeX, obj.transform.position.y - objSizeY, 0.0f);
		Vector3 leftTop = new Vector3(obj.transform.position.x - objSizeX, obj.transform.position.y + objSizeY, 0.0f);
		Vector3 rightBottom = new Vector3(obj.transform.position.x + objSizeX, obj.transform.position.y - objSizeY, 0.0f);
		Vector3 rightTop = new Vector3(obj.transform.position.x + objSizeX, obj.transform.position.y + objSizeY, 0.0f);

		// if place where we started select rectangle is on given unit
		if (IsBetweenTwoVectors(startSelectionMousePosition, leftBottom, rightTop)) {
			return true;
		}
		// if place where we ended select rectangle is on given unit
		if (IsBetweenTwoVectors(endSelectionMousePosition, leftBottom, rightTop)) {
			return true;
		}

		// if any corner of given unit is within select rectangle
		// leftBottom corner
		if (IsBetweenTwoVectors(leftBottom, startSelectRectanglePosition, endSelectRectanglePosition)) {
			return true;
		}
		// leftTop corner
		if (IsBetweenTwoVectors(leftTop, startSelectRectanglePosition, endSelectRectanglePosition)) {
			return true;
		}
		// rightBottom corner
		if (IsBetweenTwoVectors(rightBottom, startSelectRectanglePosition, endSelectRectanglePosition)) {
			return true;
		}
		// rightTop corner
		if (IsBetweenTwoVectors(rightTop, startSelectRectanglePosition, endSelectRectanglePosition)) {
			return true;
		}

		return false;
	}

	bool IsBetweenTwoVectors(Vector3 checkVector, Vector3 firstVector, Vector3 secondVector) {
		return IsInRange(checkVector.x, firstVector.x, secondVector.x)
			&& IsInRange(checkVector.y, firstVector.y, secondVector.y);
	}

	bool IsInRange(float number, float startRange, float endRange) {
		if (number >= startRange && number <= endRange) {
			return true;
		}
		return false;
	}

    void OnGUI()
    {
    	if (spaceStation.GetComponent<StationController>().isSelected) {
            GUI.BeginGroup(new Rect(300, 300, 400, 100));

            GUI.Box(new Rect(0, 0, 400, 100), "Space Station");
            if (GUI.Button(new Rect(10, 40, 70, 40), "Fr-5 Ore"))
            {
                if (SS.Ore >= 5.0f)
                {
                    unit = GameObject.Instantiate(GameObject.FindGameObjectWithTag("Unit"), new Vector3(1, 0, 0),
                        Quaternion.identity);
                    allUnits.Add(unit);
                    SS.Ore -= 5.0f;
                }
            }
            if (GUI.Button(new Rect(80, 40, 70, 40), "Ft-10 Ore"))
            {
                if (SS.Ore >= 10.0f)
                {
                    unit = GameObject.Instantiate(GameObject.FindGameObjectWithTag("Unit1"), new Vector3(1, 0, 0),
                        Quaternion.identity);
                    allUnits.Add(unit);
                    SS.Ore -= 10.0f;
                }
            }
            if (GUI.Button(new Rect(150, 40, 70, 40), "Dt-30 Ore"))
            {               
                if (SS.Ore >= 30.0f)
                {
                    unit = GameObject.Instantiate(GameObject.FindGameObjectWithTag("Unit2"), new Vector3(1, 0, 0),
                        Quaternion.identity);
                    allUnits.Add(unit);
                    SS.Ore -= 30.0f;
                }
            }
            if (GUI.Button(new Rect(220, 40, 70, 40), "Hv-1 Ore"))
            {
                if (SS.Ore >= 1.0f)
                {
                    unit = GameObject.Instantiate(GameObject.FindGameObjectWithTag("Unit3"), new Vector3(1, 0, 0),
                        Quaternion.identity);
                    allUnits.Add(unit);
                    SS.Ore -= 1.0f;
                }
            }

            GUI.Label(new Rect(300, 40, 50, 40), SS.Ore.ToString("0.##") + " Ore");

            GUI.EndGroup();
        }
    }

    bool IsMouseOnInterface () {
        //to do 
        return true;
    }
}