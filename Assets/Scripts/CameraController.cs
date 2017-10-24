using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

	public float scrollingSpeed;
	public float moveCameraSpeed;
	private int theScreenWidth;
	private int theScreenHeight;
	private Camera cam;

	void Start() {
		cam = GetComponent<Camera>();
		theScreenWidth = Screen.width;
		theScreenHeight = Screen.height;
	}

	// Update is called once per frame
	void Update () {
		// scrolling
		cam.orthographicSize -= cam.orthographicSize * Input.GetAxis("Mouse ScrollWheel") * scrollingSpeed * Time.deltaTime;
		cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, 10.0f, 25.0f);

		//follow camera bound

		float moveCameraHorizontalMouse=0.0f;
		float moveCameraVerticaMouse = 0.0f;
		if (Input.mousePosition.x > theScreenWidth - theScreenWidth)
		{
			moveCameraHorizontalMouse +=Time.deltaTime;
		}

		if (Input.mousePosition.x < 0 + theScreenWidth)
		{
			moveCameraHorizontalMouse -=Time.deltaTime;
		}

		if (Input.mousePosition.y > theScreenHeight - theScreenHeight)
		{
			moveCameraVerticaMouse += Time.deltaTime;
		}

		if (Input.mousePosition.y < 0 + theScreenHeight)
		{
			moveCameraVerticaMouse -=Time.deltaTime;
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
