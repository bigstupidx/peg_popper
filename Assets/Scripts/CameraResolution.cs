using UnityEngine;
using System.Collections;

public class CameraResolution : MonoBehaviour {
	// Set in the inspector
	public float minimumDistance = 10.0f;
	public float maximumDistance = 30.0f;
	public Transform locationToShow = null;

	public GUISkin myGUISkin = null;
	public Texture fadeTexture = null;

	// GUI regions
	private Rect	PauseBox_ScreenRect	=	new Rect( 200, 490, 680, 660);	
	private Rect    Continue_ScreenRect	=	new Rect( 230, 640, 620, 100 ); 	
	private Rect    Quit_ScreenRect		=	new Rect( 230, 760, 620, 100);	
	private Rect    Ach_ScreenRect		=	new Rect( 230, 880, 620, 100);
	private Rect    Leader_ScreenRect	=	new Rect( 230, 1000, 620, 100);
	//private Rect   	Signout_ScreenRect	=	new Rect( 230, 1120, 620, 100);
	
	private float currentAspect;

	private BoardManager boardManager = null;
	private BoardManagerSquare boardManagerSquare = null;
	private BoardManagerEuropean boardManagerEuropean = null;

	private SceneManager_Base scene = null;
	private PersistentData data = null;

	void Awake(){
		boardManager = (BoardManager)FindObjectOfType(typeof(BoardManager));
		if (boardManager == null){
			boardManagerSquare = (BoardManagerSquare)FindObjectOfType(typeof(BoardManagerSquare));
		}

		if (boardManagerSquare == null){
			boardManagerEuropean = (BoardManagerEuropean)FindObjectOfType(typeof(BoardManagerEuropean));
		}
		scene = SceneManager_Base.Instance;
		data = PersistentData.Instance;
	}

	void Start(){
		//AdjustFOVToShowPoint(new Vector3(9.75f, 2.4f, 4.6f));
		AdjustDistanceToShowPoint(locationToShow.position);
		currentAspect = camera.aspect;
	}

	void Update(){
		if (camera.aspect != currentAspect){
			//AdjustFOVToShowPoint(new Vector3(9.75f, 2.4f, 4.6f));
			AdjustDistanceToShowPoint(locationToShow.position);
			currentAspect = camera.aspect;
		}
	}

	void AdjustFOVToShowPoint(Vector3 worldPointToShow){
		Vector3 viewPos = camera.WorldToViewportPoint(worldPointToShow);
		
		while(viewPos.x < -0.031f){
			camera.fieldOfView += 1.0f;
			viewPos = camera.WorldToViewportPoint(worldPointToShow);
		}
		while(viewPos.x > -0.029f){
			camera.fieldOfView -= 1.0f;
			viewPos = camera.WorldToViewportPoint(worldPointToShow);
		}

		if(viewPos.y < 0.229f){
			while(viewPos.y < 0.229f){
				camera.transform.Translate(-0.01f*Vector3.up, Space.Self);
				viewPos = camera.WorldToViewportPoint(worldPointToShow);
			}
		}else if(viewPos.y > 0.231f){
			while(viewPos.y > 0.231f){
				camera.transform.Translate(0.01f*Vector3.up, Space.Self);
				viewPos = camera.WorldToViewportPoint(worldPointToShow);
			}
		}
	}

	void AdjustDistanceToShowPoint(Vector3 worldPointToShow){
		// get the ray coming out of the center of the screen
		// and create a plane at the point to show normal to that ray
		Ray cameraRay = camera.ScreenPointToRay(new Vector3(camera.pixelWidth*0.5f ,camera.pixelHeight*0.5f));

		Plane boardPlane = new Plane(-cameraRay.direction, worldPointToShow);

		// get the required width to show
		float width = worldPointToShow.x*2.0f;

		float currentDistance;
		boardPlane.Raycast(cameraRay, out currentDistance);

		float requiredDistance = width/(camera.aspect*2*Mathf.Tan (camera.fieldOfView*0.5f*Mathf.Deg2Rad));

		// translate the camera back to show the entire board
		// don't move the camera too close
		if (requiredDistance <= minimumDistance )
		{
			requiredDistance = minimumDistance;
		} else if(requiredDistance > maximumDistance){
			requiredDistance = maximumDistance;
		}
		camera.transform.Translate(Vector3.forward*(currentDistance-requiredDistance),Space.Self);
		Vector3 viewPos = camera.WorldToViewportPoint(worldPointToShow);

		if(viewPos.y < 0.229f){
			while(viewPos.y < 0.229f){
				camera.transform.Translate(-0.01f*Vector3.up, Space.Self);
				viewPos = camera.WorldToViewportPoint(worldPointToShow);
			}
		}else if(viewPos.y > 0.231f){
			while(viewPos.y > 0.231f){
				camera.transform.Translate(0.01f*Vector3.up, Space.Self);
				viewPos = camera.WorldToViewportPoint(worldPointToShow);
			}
		}
	}

	void OnGUI(){
		Color oldGUIColor = GUI.color;
		// fade the game screen behind the GUI menu
		if (fadeTexture){
			GUI.color = new Color(0, 0, 0, 1.0f-scene.GetScreenFade());
			GUI.DrawTexture( new Rect(-1, -1, Screen.width*2, Screen.height*2 ), fadeTexture );
			
			GUI.color = oldGUIColor;
		}

		if (scene.showingAd) return;

		if (boardManager!=null)
			if (boardManager.state != GameState.Paused) return;
		
		if (boardManagerSquare!=null)
			if (boardManagerSquare.state != GameState.Paused) return;

		if (boardManagerEuropean!=null)
			if (boardManagerEuropean.state != GameState.Paused) return;
		
		// Assign the custom GUI Skin used by our game
		if (myGUISkin) GUI.skin = myGUISkin;

		oldGUIColor = GUI.color;
		// fade the game screen behind the GUI menu
		if (fadeTexture){
			GUI.color = new Color(0, 0, 0, 0.7f);
			GUI.DrawTexture( new Rect(0, 0, Screen.width, Screen.height ), fadeTexture );

			GUI.color = oldGUIColor;
		}

		//Set up scaling matrix so we can work in EASY coordinates
		float rx = Screen.width/1080.0f;
		float ry =  Screen.height/1920.0f;
		
		// Backup the current GUI matrix as we are about to change it so we can work
		// in standard coordinates for all resolutions
		Matrix4x4 oldMat = GUI.matrix;
		
		// Set up the scaling matrix
		GUI.matrix = Matrix4x4.TRS (new Vector3(0, 0, 0), Quaternion.identity, new Vector3 (rx, ry, 1));	
		
		// Create the transparent options box below the header
		GUI.Box(PauseBox_ScreenRect, "Game Paused");
		
		// Create and react to the Continue button being pressed
		if (GUI.Button( Continue_ScreenRect, "Continue"))		
		{
			scene.OnButtonSelect("Continue");
		}
		// Create and react to the Quit button
		if (GUI.Button( Quit_ScreenRect, "Main Menu")) 
		{
			scene.OnButtonSelect("Main Menu");
		}
		GUI.enabled = (data.userWantsAchievements == 1);
		// Create and react to the Quit button
		if (GUI.Button( Ach_ScreenRect, "Achievements")) 
		{
			scene.OnButtonSelect("Achievements");
		}
		// Create and react to the Quit button
		if (GUI.Button( Leader_ScreenRect, "Leaderboards")) 
		{
			scene.OnButtonSelect("Leaders");
		}
		GUI.enabled = true;

//		// Create and react to the Quit button
//		if (GUI.Button( Signout_ScreenRect, "Signout")) 
//		{
//			scene.OnButtonSelect("Signout");
//		}

		// Restore the matrix
		GUI.matrix = oldMat;
		
	}
}
