using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingTextController : MonoBehaviour
{

    private static FloatingText popupText;
    private static GameObject canvas;

    void Start()
    {
        Initialize();
    }

    public static void Initialize()
    {
        canvas = GameObject.Find("PopupCanvas");       
        popupText = Resources.Load<FloatingText>("Prefabs/PopupTextParent");
    }

    public static void CreatingFloatingText(string text, Transform location)
    {
        FloatingText instance = Instantiate(popupText);
        if (instance == null)
            Debug.Log("Load Object Fail");
        instance.transform.SetParent(canvas.transform,false);
        instance.SetText(text);
    }
}
