
#pragma strict
#pragma downcast



class TrackableObject{
	var object:GameObject;
	var blip:Transform;
	
	function TrackableObject(arg_object:GameObject, arg_blip:Transform){
		object=arg_object;
		blip=arg_blip;
	}
}

class TrackableTaggedObjectInfo{
	var tagName:String;
	var objectBlipTexture:Texture;
	var objectBlipSize:float;
	var trackInRuntime:boolean=false;
	private var objectList:Array=new Array();
	
	function GetObjectList(){
		return objectList;
	}
	function SetObjectList(list:Array){
		objectList=list;
	}
}

class TrackableColliderInfo{
	var colliderLayer:int;
	var objectBlipTexture:Texture;
	var objectBlipSize:float;
	var trackInRuntime:boolean=false;
	private var objectList=new Array();
	
	function GetObjectList(){
		return objectList;
	}
	function SetObjectList(list:Array){
		objectList=list;
	}
}

var minimapLayer:int=20;
var mapTexture:Texture;
var mapSize:Vector2;

var TrackableTaggedObjects:TrackableTaggedObjectInfo[];
var TrackableColliders:TrackableColliderInfo[];
private var mapPositionOnScreen:Rect=Rect (Screen.width-300,0, 300, 300);

private var minimap:Transform;

var enableMapRotateOption:boolean=false;
private var rotateMap:boolean=false;

var enableMapClickMove:boolean=false;

var enableTrackCamera:boolean=true;
var trackCameraObject:Transform;
var trackCameraObjectTexture:Texture;
var trackCameraObjectBlipSize:float=3;
private var trackCameraObjectBlip:Transform;

private var shader1:Shader;
private var colour:Color=Color(1, 1, 1, 1);

private var cam:Transform;
private var camCom:Camera;
private var camMain:Transform;

private var hit : RaycastHit;

function Start(){
	shader1 = Shader.Find( "Particles/Alpha Blended" );

	cam=new GameObject ("camera_minimap").transform;
	cam.gameObject.AddComponent.<Camera>();
	camCom=cam.gameObject.GetComponent("Camera") as Camera;
	cam.rotation.eulerAngles.x=0;
	camCom.orthographic=true;
	camCom.orthographicSize=Mathf.Max(mapSize.x, mapSize.y)*0.25; 
	camCom.backgroundColor=Color(0, 0, 0, 1);
	camCom.clearFlags=CameraClearFlags.SolidColor;
	camCom.depth = Camera.main.depth + 1;
	camCom.rect = Rect (mapPositionOnScreen.x/Screen.width, (1-mapPositionOnScreen.y/Screen.height)-mapPositionOnScreen.width/Screen.height, 
							mapPositionOnScreen.height/Screen.width, mapPositionOnScreen.width/Screen.height);
	camCom.cullingMask = 1<<minimapLayer;
	cam.transform.parent=transform;
	  Debug.Log("Player's Parent: " + cam.transform.parent);

	
	var map:Transform=GameObject.CreatePrimitive(PrimitiveType.Plane).transform;
	map.transform.position=Vector3(0, 0, -10);
	map.rotation.eulerAngles.x=90;


	map.transform.localScale=Vector3(0.1*mapSize.x, 0, 0.1*mapSize.y);
	map.GetComponent.<Renderer>().material.shader=shader1;
	map.GetComponent.<Renderer>().material.mainTexture=mapTexture;
	map.gameObject.layer=minimapLayer;
	map.gameObject.name="map";
	map.transform.parent=transform;
	Destroy(map.GetComponent.<Collider>());
	
	camMain=Camera.main.transform.parent;
	var camMainCom=Camera.main.gameObject.GetComponent("Camera") as Camera;
	camMainCom.cullingMask = ~(1<<minimapLayer);
	
	if(enableTrackCamera){
		if(trackCameraObject==null) trackCameraObject=Camera.main.transform;
		trackCameraObjectBlip=GameObject.CreatePrimitive(PrimitiveType.Plane).transform;
		trackCameraObjectBlip.transform.localScale=Vector3(trackCameraObjectBlipSize, trackCameraObjectBlipSize,0 );
		trackCameraObjectBlip.GetComponent.<Renderer>().material.shader=shader1;
		trackCameraObjectBlip.GetComponent.<Renderer>().material.mainTexture=trackCameraObjectTexture;
		trackCameraObjectBlip.gameObject.layer=minimapLayer;
		trackCameraObjectBlip.gameObject.name="camera";
		trackCameraObjectBlip.transform.position=Vector3(0, 0, -30);
		trackCameraObjectBlip.transform.parent=transform;
		Destroy(trackCameraObjectBlip.GetComponent.<Collider>());
	}

	if(!enableMapRotateOption) rotateMap=false;
}


function OnGUI(){
	if(GUI.Button(Rect((mapPositionOnScreen.x), (mapPositionOnScreen.width)+5, 25, 25), GUIContent("+", "Zoom in"))){
		if(camCom.orthographicSize>30) camCom.orthographicSize-=10;
	}
	if(GUI.Button(Rect((mapPositionOnScreen.x+30), (mapPositionOnScreen.width)+5, 25, 25), GUIContent("-", "Zoom out"))){
		if(camCom.orthographicSize<Mathf.Max(mapSize.x, mapSize.y)) camCom.orthographicSize+=30;
	}
	
	if(enableMapRotateOption){
		if(rotateMap){
			if(GUI.Button(Rect((mapPositionOnScreen.x+mapPositionOnScreen.width), (mapPositionOnScreen.x+mapPositionOnScreen.width)-30, 25, 25), GUIContent("S", "Static Map"))){
				rotateMap=false;
			}
		}
		else{
			if(GUI.Button(Rect((mapPositionOnScreen.x+mapPositionOnScreen.width), (mapPositionOnScreen.x+mapPositionOnScreen.width)-30, 25, 25), GUIContent("R", "Rotating Map"))){
				rotateMap=true;
			}
		}
	}
	
	GUI.Label(Rect(mapPositionOnScreen.x+5, (mapPositionOnScreen.width)-25, 100, 25), GUI.tooltip);
}

function Update(){

	if(enableTrackCamera){
		trackCameraObjectBlip.position=trackCameraObject.position+Vector3(0, 0, 0);
		trackCameraObjectBlip.rotation.eulerAngles.z=trackCameraObject.rotation.eulerAngles.z;

		cam.position=trackCameraObject.position+Vector3(0, 0, -300);
		if(rotateMap){
			cam.rotation.eulerAngles.z=trackCameraObject.rotation.eulerAngles.z;
		}
		else cam.rotation.eulerAngles.z=0;
	}

	ScanTaggedTrackable();
	ScanTrackableColliders();
	
	DrawTaggedTrackable();
	DrawTrackableColliders();
	
	if(enableMapClickMove){
		if(Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1)){
			var ray = camCom.ScreenPointToRay (Input.mousePosition);
			if (Physics.Raycast (ray, hit, Mathf.Infinity)) {
				trackCameraObject.position.x=hit.point.x;
				trackCameraObject.position.y=hit.point.y;
			}
		}
	}

}

function DrawTaggedTrackable(){
	for(var n=0; n<TrackableTaggedObjects.length; n++){
		if(TrackableTaggedObjects[n].trackInRuntime){
			var tempTrackable:TrackableObject;
			var list:Array=TrackableTaggedObjects[n].GetObjectList();
			
			if(list.length>0){
				for(var i=0; i<list.length; i++){
					tempTrackable=list[i] as TrackableObject;
					if(tempTrackable.object!=null){
						tempTrackable.blip.position=tempTrackable.object.transform.position;
						tempTrackable.blip.rotation=tempTrackable.object.transform.rotation;
						tempTrackable.blip.rotation=Quaternion.Euler(90,0,0);
						
					}
				}
			}
		}
	}
}

function DrawTrackableColliders(){
	for(var n=0; n<TrackableColliders.length; n++){
		if(TrackableColliders[n].trackInRuntime){
			var tempTrackable:TrackableObject;
			var list:Array=TrackableTaggedObjects[n].GetObjectList();
			
			if(list.length>0){
				for(var i=0; i<list.length; i++){
					tempTrackable=list[i] as TrackableObject;
					if(tempTrackable.object!=null){
						tempTrackable.blip.position=tempTrackable.object.transform.position;
						tempTrackable.blip.rotation=tempTrackable.object.transform.rotation;
						tempTrackable.blip.rotation=Quaternion.Euler(90,0,0);
					}
				}
			}
		}
	}
}

function ScanTaggedTrackable(){
	for(var n=0; n<TrackableTaggedObjects.length; n++){
		var tempTrackable:TrackableObject;
		var i:int;
		var list:Array=TrackableTaggedObjects[n].GetObjectList();

		if(list.length>0){
			for(i=0; i<list.length; i++){
				tempTrackable=list[i] as TrackableObject;
				if(tempTrackable.object==null){
					Destroy(tempTrackable.blip.gameObject);
					list.RemoveAt(i);
				}
			}
		}
		
		var Objects:GameObject[] = GameObject.FindGameObjectsWithTag(TrackableTaggedObjects[n].tagName);
		for(var object:GameObject in Objects){
			var match:boolean=false;
			for(i=0; i<list.length; i++){
				tempTrackable=list[i] as TrackableObject;
				if(tempTrackable.object==object) {
					match=true;
					break;
				}
			}
			
			if(!match){

				var objectBlip:Transform=GameObject.CreatePrimitive(PrimitiveType.Plane).transform;
				
				var scaleSize:float=TrackableTaggedObjects[n].objectBlipSize;
				if(scaleSize==0) objectBlip.localScale=object.transform.localScale*0.1;
				else
				objectBlip.localScale=Vector3(TrackableTaggedObjects[n].objectBlipSize, 0, TrackableTaggedObjects[n].objectBlipSize);
				objectBlip.transform.GetComponent.<Renderer>().material.mainTexture=TrackableTaggedObjects[n].objectBlipTexture;
				objectBlip.transform.GetComponent.<Renderer>().material.shader=shader1;
				objectBlip.transform.GetComponent.<Renderer>().material.color=colour;
				objectBlip.gameObject.layer=minimapLayer;
				
				objectBlip.gameObject.name=TrackableTaggedObjects[n].tagName;
				objectBlip.transform.parent=transform;
				
				
				objectBlip.position=object.transform.position;
				objectBlip.rotation=object.transform.rotation;
				
				Destroy(objectBlip.GetComponent.<Collider>());
				
				list.Push(new TrackableObject(object, objectBlip));
			}
		}
		TrackableTaggedObjects[n].SetObjectList(list);
	}
}


function ScanTrackableColliders(){
	for(var n=0; n<TrackableColliders.length; n++){
		var tempTrackable:TrackableObject;
		var i:int;
		var list:Array=TrackableColliders[n].GetObjectList();

		if(list.length>0){
			for(i=0; i<list.length; i++){
				tempTrackable=list[i] as TrackableObject;
				if(tempTrackable.object==null){
					Destroy(tempTrackable.blip.gameObject);
					list.RemoveAt(i);
				}
			}
		}
		
		var layerMask=1 << TrackableColliders[n].colliderLayer;
		var Objects:Collider[] = Physics.OverlapSphere(Vector3.zero, Mathf.Infinity, layerMask);
		for(var object:Collider in Objects){
			var match:boolean=false;
			for(i=0; i<list.length; i++){
				tempTrackable=list[i] as TrackableObject;
				if(tempTrackable.object==object.gameObject) {
					match=true;
					break;
				}
			}
			
			if(!match){

				var objectBlip:Transform=GameObject.CreatePrimitive(PrimitiveType.Plane).transform;
				
				var scaleSize:float=TrackableColliders[n].objectBlipSize;
				if(scaleSize==0) objectBlip.localScale=object.transform.localScale*0.1;
				else
				objectBlip.localScale=Vector3(TrackableColliders[n].objectBlipSize, 0, TrackableColliders[n].objectBlipSize);
				objectBlip.transform.GetComponent.<Renderer>().material.mainTexture=TrackableColliders[n].objectBlipTexture;
				objectBlip.transform.GetComponent.<Renderer>().material.shader=shader1;
				objectBlip.transform.GetComponent.<Renderer>().material.color=colour;
				objectBlip.gameObject.layer=minimapLayer;
				
				objectBlip.gameObject.name=TrackableColliders[n].colliderLayer.ToString();
				objectBlip.transform.parent=transform;
				
				
				objectBlip.position=object.transform.position;
				objectBlip.rotation=object.transform.rotation;
				
				Destroy(objectBlip.GetComponent.<Collider>());
				
				list.Push(new TrackableObject(object.gameObject, objectBlip));
			}
		}
		TrackableColliders[n].SetObjectList(list);
	}
}
