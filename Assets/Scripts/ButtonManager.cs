using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonManager : MonoBehaviour
{
	public GameObject fighterPrefab;
	public GameObject frigatePrefab;
	public GameObject destroyerPrefab;
    public GameObject harvesterPrefab;
    public GameObject catcherPrefab;

    private GameController gameController;
	private StationController spaceStationController;

	void Start () {
		gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
		spaceStationController = GameObject.FindGameObjectWithTag("SpaceStation").GetComponent<StationController>();
	}
		
    public void CreateFighter() {
		if (spaceStationController.Ore >= 5.0f) {
			gameController.allUnits.Add(GameObject.Instantiate(fighterPrefab, new Vector3(1, 0, 0), Quaternion.identity));
			spaceStationController.Ore -= 5.0f;
        }
    }

    public void CreateFrigate() {
		if (spaceStationController.Ore >= 10.0f) {
			gameController.allUnits.Add(GameObject.Instantiate(frigatePrefab, new Vector3(1, 0, 0), Quaternion.identity));
			spaceStationController.Ore -= 10.0f;
		}
    }

    public void CreateDestroyer() {
		if (spaceStationController.Ore >= 30.0f){
			gameController.allUnits.Add(GameObject.Instantiate(destroyerPrefab, new Vector3(1, 0, 0), Quaternion.identity));
			spaceStationController.Ore -= 30.0f;
		}
    }

    public void CreateHarvester() {
		if (spaceStationController.Ore >= 10.0f) {
			gameController.allUnits.Add(GameObject.Instantiate(harvesterPrefab, new Vector3(1, 0, 0), Quaternion.identity));
			spaceStationController.Ore -= 10.0f;
		}
	}
    public void CreateCatcher()
    {
        if (spaceStationController.Ore >= 10.0f)
        {
            gameController.allUnits.Add(GameObject.Instantiate(catcherPrefab, new Vector3(1, 0, 0), Quaternion.identity));
            spaceStationController.Ore -= 10.0f;
        }
    }

    public void NewGame(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void LoadGame()
    {

    }

    public void Authors(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
