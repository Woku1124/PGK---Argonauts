using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour {

	public GameObject selectRectanglePrefab;
    public GameObject fighterPrefab;
	public GameObject frigatePrefab;
	public GameObject destroyerPrefab;
	public GameObject harvesterPrefab;
	[HideInInspector]
	public List<GameObject> allUnits;
	[HideInInspector]
	public List<Vector3> lockedPositions;
	[HideInInspector]
	public List<GameObject> enemyUnits;

    private bool isSelecting;
	private float InterfaceX;
	private float InterfaceY;
	private float InterfaceWidth;
	private float InterfaceHeight;
    private bool gamePausedByESC;
    private bool gamePausedByP;
    private GUIStyle guiStyle = new GUIStyle();

    private Vector3 startSelectionMousePosition;
	private Vector3 endSelectionMousePosition;
	private Vector3 startSelectRectanglePosition;
	private Vector3 endSelectRectanglePosition;
	private GameObject selectRectangle;
	private GameObject spaceStation;
	private GameObject piratesStation;
    private StationController spaceStationController;

    // Use this for initialization
    void Start () {
        gamePausedByESC = false;
        gamePausedByP = false;
        isSelecting = false;
        InterfaceX = 300.0f;
		InterfaceY = 300.0f;
		InterfaceWidth = 400.0f;
		InterfaceHeight = 100.0f;
        guiStyle.fontSize = 20;
        guiStyle.normal.textColor = Color.white;

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

		spaceStationController = spaceStation.GetComponent<StationController>();

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
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (gamePausedByESC)
            {
                Time.timeScale = 1;
                gamePausedByP = false;
                gamePausedByESC = false;
            }
            else
            {
                Time.timeScale = 0;
                gamePausedByESC = true;
                gamePausedByP = false;
            }
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (gamePausedByP)
            {
                Time.timeScale = 1;
                gamePausedByESC = false;
                gamePausedByP = false;
            }
            else
            {
                Time.timeScale = 0;
                gamePausedByP = true;
                gamePausedByESC = false;
            }
        }
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
		if (spaceStation) {
			spaceStation.GetComponent<StationController>().isSelected = false;
			// temporary
			spaceStation.GetComponent<SpriteRenderer>().color = Color.grey;
		}
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
		if (obj == null) {
			return false;
		}
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
        //showing sources in the left corner
        GUI.BeginGroup(new Rect(0, 0, 110, 30));
        Color background = Color.black;
        background.a = 0.3f;
        DrawQuad(new Rect(0, 0, Screen.width, Screen.height), background);
        GUI.Label(new Rect(5, 5, 100, 20), spaceStationController.Ore.ToString("0.##") + " Ore");
        GUI.EndGroup();

        if (gamePausedByESC)
        {
            background = Color.black;
            background.a = 0.3f;
            DrawQuad(new Rect(0, 0, Screen.width, Screen.height), background);
            DrawMenu();
        }

        if (gamePausedByP)
        {
            background = Color.black;
            background.a = 0.3f;
            DrawQuad(new Rect(0, 0, Screen.width, Screen.height), background);
            GUI.Label(new Rect(Screen.width / 2 - 100, Screen.height / 2 - 25, 200, 50), "Game Paused", guiStyle);
        }

        if (GUI.Button(new Rect(Screen.width - 120, Screen.height-70, 100, 50), "MENU"))
        {
            Time.timeScale = 0;
            gamePausedByESC = true;
        }

    }

    void DrawMenu()
    {
        Color background = Color.black;
        background.a = 1f;
        DrawQuad(new Rect(Screen.width / 2 - 150, Screen.height / 2 - 250, 300, 500), background);
        GUI.BeginGroup(new Rect(Screen.width / 2 - 150, Screen.height / 2 - 250, 300, 500));
        GUI.Label(new Rect(50, 20, 200, 50), "MENU- Game Paused", guiStyle);
        if (GUI.Button(new Rect(50, 80, 200, 50), "Return"))
        {
            Time.timeScale = 1;
            gamePausedByESC = false;
        }
        /*if (GUI.Button(new Rect(50, 140, 200, 50), "Save Game"))
        {

        }*/
        /*if (GUI.Button(new Rect(50, 200, 200, 50), "Load Game"))
        {

        }*/
        if (GUI.Button(new Rect(50, 260-120, 200, 50), "Restart Game"))
        {
            Application.LoadLevel(1);
            Time.timeScale = 1;
        }
        if (GUI.Button(new Rect(50, 320-120, 200, 50), "Main Menu"))
        {
            SceneManager.LoadScene(0);
        }
        if (GUI.Button(new Rect(50, 380-120, 200, 50), "Exit"))
        {
            Application.Quit();
        }
        GUI.EndGroup();
    }
    void DrawQuad(Rect position, Color color)
    {
        Texture2D texture = new Texture2D(1, 1);
        texture.SetPixel(0, 0, color);
        texture.Apply();
        GUI.skin.box.normal.background = texture;
        GUI.Box(position, GUIContent.none);
    }
}