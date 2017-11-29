using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AsteroidController : MonoBehaviour {

    public float speed;
    public Rigidbody2D r;

    public Sprite[] asterSprites;

    public Image ResourceBar;
    public GameObject aster;

    public GameObject catcher;

    public bool trigger = true;


    void Awake()
    {
        asterSprites = Resources.LoadAll<Sprite>("Sprites/AsteroidPhase");
    }

    // Use this for initialization
    void Start() {
        aster.GetComponent<SpriteRenderer>().sprite = asterSprites[0];
    }

    // Update is called once per frame
    void Update()
    {
        if (trigger == true && Time.timeScale ==1)
        {
            transform.Translate(0.01f, -0.01f, 0);
        }

        if (ResourceBar.fillAmount < 0.75f && ResourceBar.fillAmount > 0.51f)
        {
            aster.GetComponent<SpriteRenderer>().sprite = asterSprites[1];
            Debug.Log("0.75");

        }


        if (ResourceBar.fillAmount < 0.50f && ResourceBar.fillAmount > 0.26f)
        {
            aster.GetComponent<SpriteRenderer>().sprite = asterSprites[2];
            Debug.Log("0.50");

        }


        if (ResourceBar.fillAmount < 0.25f)
        {
            aster.GetComponent<SpriteRenderer>().sprite = asterSprites[3];
            Debug.Log("0.25");

        }

        if (ResourceBar.fillAmount == 0f)
        {
            Destroy(gameObject);

        }
    }


    void OnTriggerStay2D(Collider2D other)
    {
        
        if (other.gameObject.tag == "Catcher" && trigger)
        {
            gameObject.transform.position = other.transform.position;
        }

        if (other.gameObject.name == "Field")
        {
            trigger = false;
        }
    }
    void FixedUpdate() {
        transform.Translate(transform.up * speed * Time.deltaTime);
    }
}
