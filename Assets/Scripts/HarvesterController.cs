using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HarvesterController : MonoBehaviour
{
    public float maxCapacity = 100.0f;
    public float resource;
    public StationController station;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}


    void AddResource()
    {
        resource += 0.01f;
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Asteroid"))
        {    
            AddResource();
        }

        if (other.gameObject.CompareTag("SpaceStation"))
        {
            station.Ore += resource;
            resource = 0.0f;
        }
    }
}
