using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

	public float scrollingSpeed;
	public float moveCameraSpeed;

	private int theScreenWidth;
	private int theScreenHeight;
	private Camera cam;
    // margin frame inside the camera view for triggering the scrolling (in pixels)
    private int mouseScrollOffsetPx = 5;

	void Start() {
		cam = GetComponent<Camera>();
		theScreenWidth = Screen.width;
		theScreenHeight = Screen.height;
	}

	// Update is called once per frame
	void Update () {
		// scrolling
		cam.orthographicSize -= cam.orthographicSize * Input.GetAxis("Mouse ScrollWheel") * scrollingSpeed * Time.deltaTime;
		cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, 10.0f, 30.0f);

		//follow camera bound

		float moveCameraHorizontalMouse=0.0f;
		float moveCameraVerticaMouse = 0.0f;
        // move left
		if (Input.mousePosition.x < 0 + mouseScrollOffsetPx)
		{
            moveCameraHorizontalMouse -= Time.deltaTime;
		}
        // move right
        if (Input.mousePosition.x > theScreenWidth - mouseScrollOffsetPx)
		{
            moveCameraHorizontalMouse += Time.deltaTime;
		}

        // move down
        if (Input.mousePosition.y < 0 + mouseScrollOffsetPx)
		{
            moveCameraVerticaMouse -= Time.deltaTime;
		}
        // move down
        if (Input.mousePosition.y > theScreenHeight - mouseScrollOffsetPx)
		{
            moveCameraVerticaMouse += Time.deltaTime;
		}
        
        // moving camera
        float moveCameraHorizontal = GetKeyToFloat(KeyCode.D) - GetKeyToFloat(KeyCode.A);
		float moveCameraVertical = GetKeyToFloat(KeyCode.W) - GetKeyToFloat(KeyCode.S);
		Vector3 translateCamera = new Vector3(moveCameraHorizontal * Time.deltaTime + moveCameraHorizontalMouse, moveCameraVertical * Time.deltaTime +moveCameraVerticaMouse, 0.0f);
		gameObject.transform.position += translateCamera * moveCameraSpeed * cam.orthographicSize;
	}

	float GetKeyToFloat(KeyCode keycode) {
		if (Input.GetKey(keycode)) {
			return 1.0f;
		}
		return 0.0f;
	}
}
