using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minimap : MonoBehaviour {
public class TrackableObject  {
	public GameObject objectt;
	public Transform blip;
	public TrackableObject()
	{
		
	}
	public TrackableObject(GameObject arg_object,Transform arg_blip)
	{
		objectt=arg_object;
		blip=arg_blip;
	}
	public void SetObject(GameObject obj)
	{
		objectt = obj;
	}
}
	public  class TrackableTaggedObjectInfo
	{
	public string tagName;
	public Texture objectBlipTexture;
	public float objectBlipSize;
	public bool trackInRuntime=false;
	private List<GameObject> objectList=new List<GameObject>();

	public List<GameObject> GetObjectList(){
		return objectList;
	}
	public void SetObjectList(List<GameObject> list){
		objectList=list;
	}
}
	public class TrackableColliderInfo{
	public int colliderLayer;
	public Texture objectBlipTexture;
	public float objectBlipSize;
	public bool trackInRuntime=false;
	private List<GameObject> objectList=new List<GameObject>();

	public List<GameObject> GetObjectList(){
		return objectList;
	}
	public void SetObjectList(List<GameObject> list){
		objectList=list;
	}
}



	public int minimapLayer=12;
	public Texture mapTexture;
	public Vector2 mapSize;

	public List<TrackableTaggedObjectInfo> TrackableTaggedObjects;
	public List<TrackableColliderInfo> TrackableColliders;
	private Rect mapPositionOnScreen=new Rect (Screen.width-160,0, 160, 160);

	private Transform minimap;

	bool enableMapRotateOption=false;
	private bool rotateMap=false;

	public bool enableMapClickMove=false;

	public bool enableTrackCamera=true;
	public Transform trackCameraObject;
	public Texture trackCameraObjectTexture;
	public float trackCameraObjectBlipSize=1;
	private Transform trackCameraObjectBlip;

	private Shader shader1;
	private Color colour=new Color(1, 1, 1, 1);

	private Transform cam;
	private Camera camCom;
	private Transform camMain;

	private RaycastHit hit ;


	// Use this for initialization
	void Start () {
		shader1 = Shader.Find( "Particles/Alpha Blended" );

		cam=new GameObject ("camera_minimap").transform;
		cam.gameObject.AddComponent<Camera>();
		camCom=cam.gameObject.GetComponent("Camera") as Camera;
		cam.rotation.eulerAngles.Set(0,0,0);
		camCom.orthographic=true;
		camCom.orthographicSize=Mathf.Max(mapSize.x, mapSize.y)*0.25f; 
		camCom.backgroundColor=new Color(0, 0, 0, 1);
		camCom.clearFlags=CameraClearFlags.SolidColor;
		camCom.depth = Camera.main.depth + 1;
		camCom.rect = new Rect (mapPositionOnScreen.x/Screen.width, (1-mapPositionOnScreen.y/Screen.height)-mapPositionOnScreen.width/Screen.height, 
			mapPositionOnScreen.height/Screen.width, mapPositionOnScreen.width/Screen.height);
		camCom.cullingMask = 1<<minimapLayer;
		cam.transform.parent=transform;
		Debug.Log("Player's Parent: " + cam.transform.parent);


		Transform map=GameObject.CreatePrimitive(PrimitiveType.Plane).transform;
		map.transform.position=new Vector3(0, 0, -10);
		map.rotation.eulerAngles.Set(90,0,0);


		map.transform.localScale=new Vector3(0.1f*mapSize.x, 0, 0.1f*mapSize.y);
		map.GetComponent<Renderer>().material.shader=shader1;
		map.GetComponent<Renderer>().material.mainTexture=mapTexture;
		map.gameObject.layer=minimapLayer;
		map.gameObject.name="map";
		map.transform.parent=transform;
		Destroy(map.GetComponent<Collider>());

		camMain=Camera.main.transform.parent;
		var camMainCom=Camera.main.gameObject.GetComponent("Camera") as Camera;
		camMainCom.cullingMask = ~(1<<minimapLayer);

		if(enableTrackCamera){
			if(trackCameraObject==null) trackCameraObject=Camera.main.transform;
			trackCameraObjectBlip=GameObject.CreatePrimitive(PrimitiveType.Plane).transform;
			trackCameraObjectBlip.transform.localScale=new Vector3(trackCameraObjectBlipSize, trackCameraObjectBlipSize,0 );
			trackCameraObjectBlip.GetComponent<Renderer>().material.shader=shader1;
			trackCameraObjectBlip.GetComponent<Renderer>().material.mainTexture=trackCameraObjectTexture;
			trackCameraObjectBlip.gameObject.layer=minimapLayer;
			trackCameraObjectBlip.gameObject.name="camera";
			trackCameraObjectBlip.transform.position=new Vector3(0, 0, -30);
			trackCameraObjectBlip.transform.parent=transform;
			Destroy(trackCameraObjectBlip.GetComponent<Collider>());
		}

		if(!enableMapRotateOption) rotateMap=false;
	}


	void OnGUI(){
		if(GUI.Button(new Rect((mapPositionOnScreen.x), (mapPositionOnScreen.width)+5, 25, 25), new GUIContent("+", "Zoom in"))){
			if(camCom.orthographicSize>10) camCom.orthographicSize-=20;
		}
		if(GUI.Button(new Rect((mapPositionOnScreen.x+30), (mapPositionOnScreen.width)+5, 25, 25),new  GUIContent("-", "Zoom out"))){
			if(camCom.orthographicSize<Mathf.Max(mapSize.x, mapSize.y)/2) camCom.orthographicSize+=20;
		}

		if(enableMapRotateOption){
			if(rotateMap){
				if(GUI.Button(new Rect((mapPositionOnScreen.x+mapPositionOnScreen.width), (mapPositionOnScreen.x+mapPositionOnScreen.width)-30, 25, 25),new GUIContent("S", "Static Map"))){
					rotateMap=false;
				}
			}
			else{
				if(GUI.Button(new Rect((mapPositionOnScreen.x+mapPositionOnScreen.width), (mapPositionOnScreen.x+mapPositionOnScreen.width)-30, 25, 25),new GUIContent("R", "Rotating Map"))){
					rotateMap=true;
				}
			}
		}

		GUI.Label(new Rect(mapPositionOnScreen.x+5, (mapPositionOnScreen.width)-25, 100, 25), GUI.tooltip);
		
	}
	
	// Update is called once per frame
	void Update () {
		if(enableTrackCamera){
			trackCameraObjectBlip.position=trackCameraObject.position+new Vector3(0, 0, 0);
			trackCameraObjectBlip.rotation.eulerAngles.Set(0,0,trackCameraObject.rotation.eulerAngles.z);

			cam.position=trackCameraObject.position+new Vector3(0, 0, -300);
			if(rotateMap){
				cam.rotation.eulerAngles.Set(0,0,trackCameraObject.rotation.eulerAngles.z);
			}
			else cam.rotation.eulerAngles.Set(0,0,0);
		}

		ScanTaggedTrackable();
		ScanTrackableColliders();

		DrawTaggedTrackable();
		DrawTrackableColliders();

		if(enableMapClickMove){
			if(Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1)){
				Ray ray = camCom.ScreenPointToRay (Input.mousePosition);
				if (Physics.Raycast(ray, out hit, Mathf.Infinity)) {
					trackCameraObject.position.Set(hit.point.x,hit.point.y,0);
				}
			}
		}

	}

	void DrawTaggedTrackable(){
		for(var n=0; n<TrackableTaggedObjects.Count; n++){
			if(TrackableTaggedObjects[n].trackInRuntime){
				TrackableObject tempTrackable=new TrackableObject();
				List<GameObject> list=TrackableTaggedObjects[n].GetObjectList();

				if(list.Count>0){
					for(var i=0; i<list.Count; i++){
						tempTrackable.SetObject(list[i]);
						if(tempTrackable.objectt!=null){
							tempTrackable.blip.position=tempTrackable.objectt.transform.position;
							tempTrackable.blip.rotation=tempTrackable.objectt.transform.rotation;
							tempTrackable.blip.rotation=Quaternion.Euler(90,0,0);

						}
					}
				}
			}
		}
	}

	void DrawTrackableColliders(){
		for(var n=0; n<TrackableColliders.Count; n++){
			if(TrackableColliders[n].trackInRuntime){
				TrackableObject tempTrackable=new TrackableObject();
				List<GameObject> list=TrackableTaggedObjects[n].GetObjectList();

				if(list.Count>0){
					for(var i=0; i<list.Count; i++){
						tempTrackable.SetObject(list[i]);
						if(tempTrackable.objectt!=null){
							tempTrackable.blip.position=tempTrackable.objectt.transform.position;
							tempTrackable.blip.rotation=tempTrackable.objectt.transform.rotation;
							tempTrackable.blip.rotation=Quaternion.Euler(90,0,0);
						}
					}
				}
			}
		}
	}

	void ScanTaggedTrackable(){
		for(var n=0; n<TrackableTaggedObjects.Count; n++){
			TrackableObject tempTrackable=new TrackableObject();
			int i;
			List<GameObject> list=TrackableTaggedObjects[n].GetObjectList();

			if(list.Count>0){
				for(i=0; i<list.Count; i++){
					tempTrackable.SetObject(list[i]);
					if(tempTrackable.objectt==null){
						Destroy(tempTrackable.blip.gameObject);
						list.RemoveAt(i);
					}
				}
			}

			GameObject[] Objects = GameObject.FindGameObjectsWithTag(TrackableTaggedObjects[n].tagName);
			foreach(GameObject obj in Objects){
				bool match=false;
				for(i=0; i<list.Count; i++){
					tempTrackable.SetObject(list[i]);
					if(tempTrackable.objectt==obj) {
						match=true;
						break;
					}
				}

				if(!match){

					Transform objectBlip=GameObject.CreatePrimitive(PrimitiveType.Plane).transform;

					float scaleSize=TrackableTaggedObjects[n].objectBlipSize;
					if(scaleSize==0) objectBlip.localScale=(obj.transform.localScale)*0.1f;
					else
						objectBlip.localScale=new Vector3(TrackableTaggedObjects[n].objectBlipSize, 0, TrackableTaggedObjects[n].objectBlipSize);
					objectBlip.transform.GetComponent<Renderer>().material.mainTexture=TrackableTaggedObjects[n].objectBlipTexture;
					objectBlip.transform.GetComponent<Renderer>().material.shader=shader1;
					objectBlip.transform.GetComponent<Renderer>().material.color=colour;
					objectBlip.gameObject.layer=minimapLayer;

					objectBlip.gameObject.name=TrackableTaggedObjects[n].tagName;
					objectBlip.transform.parent=transform;


					objectBlip.position=obj.transform.position;
					objectBlip.rotation=obj.transform.rotation;

					Destroy(objectBlip.GetComponent<Collider>());
					TrackableObject TAO=new TrackableObject (obj, objectBlip);
					list.Add(TAO.objectt);
				}
			}
			TrackableTaggedObjects[n].SetObjectList(list);
		}
	}


	void ScanTrackableColliders(){
		for(var n=0; n<TrackableColliders.Count; n++){
			TrackableObject tempTrackable=new TrackableObject();
			int i;
			List<GameObject> list=TrackableColliders[n].GetObjectList();

			if(list.Count>0){
				for(i=0; i<list.Count; i++){
					tempTrackable.SetObject(list[i]);
					if(tempTrackable.objectt==null){
						Destroy(tempTrackable.blip.gameObject);
						list.RemoveAt(i);
					}
				}
			}

			var layerMask=1 << TrackableColliders[n].colliderLayer;
			Collider[] Objects = Physics.OverlapSphere(Vector3.zero, Mathf.Infinity, layerMask);
			foreach(Collider obj in Objects){
				bool match=false;
				for(i=0; i<list.Count; i++){
					tempTrackable.SetObject(list[i]);
					if(tempTrackable.objectt==obj.gameObject) {
						match=true;
						break;
					}
				}

				if(!match){

					Transform objectBlip=GameObject.CreatePrimitive(PrimitiveType.Plane).transform;

					float scaleSize=TrackableColliders[n].objectBlipSize;
					if(scaleSize==0) objectBlip.localScale=obj.transform.localScale*0.1f;
					else
						objectBlip.localScale=new Vector3(TrackableColliders[n].objectBlipSize, 0, TrackableColliders[n].objectBlipSize);
					objectBlip.transform.GetComponent<Renderer>().material.mainTexture=TrackableColliders[n].objectBlipTexture;
					objectBlip.transform.GetComponent<Renderer>().material.shader=shader1;
					objectBlip.transform.GetComponent<Renderer>().material.color=colour;
					objectBlip.gameObject.layer=minimapLayer;

					objectBlip.gameObject.name=TrackableColliders[n].colliderLayer.ToString();
					objectBlip.transform.parent=transform;


					objectBlip.position=obj.transform.position;
					objectBlip.rotation=obj.transform.rotation;

					Destroy(objectBlip.GetComponent<Collider>());
					TrackableObject TAO=new TrackableObject (obj.gameObject, objectBlip);
					list.Add(TAO.objectt);
				}
			}
			TrackableColliders[n].SetObjectList(list);
		}
		
	}
}
