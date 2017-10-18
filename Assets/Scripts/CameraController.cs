using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

	public float scrollingSpeed;
	public float moveCameraSpeed;

	private Camera cam;

	void Start() {
		cam = GetComponent<Camera>();
	}

	// Update is called once per frame
	void Update () {
		// scrolling
		cam.orthographicSize -= cam.orthographicSize * Input.GetAxis("Mouse ScrollWheel") * scrollingSpeed * Time.deltaTime;
		cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, 3.0f, 10.0f);

		// moving camera
		float moveCameraHorizontal = GetKeyToFloat(KeyCode.D) - GetKeyToFloat(KeyCode.A);
		float moveCameraVertical = GetKeyToFloat(KeyCode.W) - GetKeyToFloat(KeyCode.S);
		Vector3 translateCamera = new Vector3(moveCameraHorizontal * Time.deltaTime, moveCameraVertical * Time.deltaTime, 0.0f);
		gameObject.transform.position += translateCamera * moveCameraSpeed * cam.orthographicSize;
	}

	float GetKeyToFloat(KeyCode keycode) {
		if (Input.GetKey(keycode)) {
			return 1.0f;
		}
		return 0.0f;
	}
}
