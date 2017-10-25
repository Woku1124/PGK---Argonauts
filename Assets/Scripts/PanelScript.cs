using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelScript : MonoBehaviour {

    public GameObject Panel;
	
	// Update is called once per frame
	public void HidePanel () {
        Panel.gameObject.SetActive(false);
	}

    public void ShowPanel()
    {
        Panel.gameObject.SetActive(true);
    }
}
