using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonManager : MonoBehaviour
{
    

    public void CreateFighter() {
        if (GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>().SS.Ore >= 5.0f)
        {
            GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>().allUnits.Add(GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>().unit = GameObject.Instantiate(GameObject.FindGameObjectWithTag("Unit"), new Vector3(1, 0, 0), Quaternion.identity));
            GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>().SS.Ore -= 5.0f;
        }
    }

    public void CreateFrigate() {
        if (GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>().SS.Ore >= 10.0f)
        {
            GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>().allUnits.Add(GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>().unit = GameObject.Instantiate(GameObject.FindGameObjectWithTag("Unit1"), new Vector3(1, 0, 0), Quaternion.identity));
            GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>().SS.Ore -= 10.0f;
        }
    }

    public void CreateDestroyer() {
        if (GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>().SS.Ore >= 30.0f) {
            GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>().allUnits.Add(GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>().unit = GameObject.Instantiate(GameObject.FindGameObjectWithTag("Unit2"), new Vector3(1, 0, 0), Quaternion.identity));
            GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>().SS.Ore -= 30.0f;
        }
    }

    public void CreateHarvester()
    {
        if (GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>().SS.Ore >= 1.0f) {
            GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>().allUnits.Add(GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>().unit = GameObject.Instantiate(GameObject.FindGameObjectWithTag("Unit3"), new Vector3(1, 0, 0), Quaternion.identity));
            GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>().SS.Ore -= 1.0f;
        }
    }
}
