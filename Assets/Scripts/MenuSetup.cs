using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuSetup : MonoBehaviour {

    public static List<Texture2D> UnitIconTextures = new List<Texture2D>();
    public static List<Texture2D> UnitIconTexturesRo = new List<Texture2D>();
    public static List<string> UnitNames = new List<string>();
    public static List<string> UnitPaths = new List<string>();

    public Texture2D IconContainer;

	void OnGUI() {
        GUIStyle Container = new GUIStyle();
        Container.normal.background = IconContainer;
        GUI.Box(new Rect(Screen.width / 2 - 200, Screen.height - 40, 400, 50),"",Container);
    }
}
