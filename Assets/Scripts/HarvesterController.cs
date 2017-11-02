using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HarvesterController : MonoBehaviour
{
    public float maxCapacity = 100.0f;
    public float resource;

    private StationController spaceSpationController;

	// Use this for initialization
	void Start () {
		spaceSpationController = GameObject.FindGameObjectWithTag("SpaceStation").GetComponent<StationController>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
		
    void AddResource() {
        resource += 0.01f;
    }

    void OnTriggerStay2D(Collider2D other) {
        if (other.gameObject.CompareTag("Asteroid")) {    
            AddResource();
        }

        if (other.gameObject.CompareTag("SpaceStation")) {
			spaceSpationController.Ore += resource;
            resource = 0.0f;
        }
    }
}
