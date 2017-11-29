using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HarvesterController : MonoBehaviour
{
    public float maxCapacity = 100.0f;
    public float resource = 0f;

    private Animator animator;

    private bool canGathering = true;

    [SerializeField]
    public Image capacityBar;   
    [SerializeField]
    public Image resourceBar;
    [SerializeField]    
    public Canvas capacityCanvas;    

    private StationController spaceSpationController;
  

	// Use this for initialization
	void Start () {
		spaceSpationController = GameObject.FindGameObjectWithTag("SpaceStation").GetComponent<StationController>();
	    capacityBar.fillAmount = resource;
	    animator = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void FixUpdate ()
	{
	    
	}

    void CapacityBar()
    {
        capacityBar.fillAmount = resource / 100f;
    }

    void AddResource() {
        resource += 0.15f;
        if (resource >= maxCapacity)
        {
            resource = maxCapacity;
            canGathering = false;
        }
        if(canGathering == true)
        resourceBar.fillAmount -= 0.001f;
    }

    void OnTriggerStay2D(Collider2D other) {
        if (other.gameObject.CompareTag("Asteroid"))
        {
            
            AddResource();  
            CapacityBar();
        }
      
   

    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Asteroid"))
        {
            animator.SetTrigger("Exit");
            capacityCanvas.gameObject.SetActive(false);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Asteroid"))
        {
            animator.SetTrigger("Enter");
            capacityCanvas.gameObject.SetActive(true);
        }

        if (other.gameObject.CompareTag("SpaceStation"))
        {
            spaceSpationController.Ore += resource;
            if(resource >0f)
				// FIXME: trzeba to poprawic, cos nie dziala
                FloatingTextController.CreatingFloatingText(resource.ToString("0.##"), transform);
            resource = 0.0f;
            canGathering = true;
        }
    }
}
