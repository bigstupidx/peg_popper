using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using GoogleMobileAds.Api;

// ---------------------------------------------------------------------------------------
// Class : SceneManager_TitleScreen
// Desc	 : Manages the sequence of events on the title screen and reacts to button 
//		   button selections
// ---------------------------------------------------------------------------------------
public class SceneManager_TitleScreen : SceneManager_Base 
{
	// set by inspector
	public GameObject MenuObject = null;
	public GameObject previousButton = null;
	public GameObject nextButton = null;
	public Vector3 prevPosition = new Vector3(0.0f, 0.0f, 0.0f);	// where the prev texture is
	public Vector3 nextPosition = new Vector3(0.0f, 0.0f, 0.0f);	// where the next texture is
	public Vector3 curPosition = new Vector3(0.0f, 0.0f, 0.0f);

	public BoardselectScript boardSelector = null;
	public SwipeController swipeController = null;

	private AdMobScript bannerAd = null;

	private PersistentData data = null;

	//GPG plugin details
	private bool waitingToShowLeaderboard = false;

	// GUI Related
	private bool userPressedBack = false;
	public Rect box_ScreenRect = new Rect(270, 825, 540, 270);
	public GUISkin myGUISkin = null;
	public Texture2D fadeTexture = null;

	void Awake(){
		data = PersistentData.Instance;

		// get a reference to the Millenial Ads
		bannerAd = FindObjectOfType<AdMobScript>();
	}

	void Start(){
		bannerAd.SetPosition(AdPosition.Bottom);

		swipeController.activeItem = data.ActiveEnvironment;

		// Select the Google Play Games platform as our social platform implementation
		GooglePlayGames.PlayGamesPlatform.Activate();

		if(PlayerPrefs.GetInt("logonStatus") == 1 && loginState==GPLoginState.loggedout && !Social.localUser.authenticated){
			Social.localUser.Authenticate(OnAuthCB);  //sign user in automatically
		}

		if (data.userWantsAchievements == 0) {
			GameObject otherGO = GameObject.Find ("Leaders");
			MenuButtonController button = (MenuButtonController) otherGO.GetComponent(typeof(MenuButtonController));
			if (button != null) {
				button.disabled = true;
			}
		}
	}

	void Update(){
		if (Input.GetKeyDown(KeyCode.Escape)) {
			if(userPressedBack){
				Application.Quit();
			}
			else{
				userPressedBack = true;
				Invoke ("ClearUserPressedBack", 1.75f);
			}
		}

		data.ActiveEnvironment = swipeController.activeItem;
	}

	void OnGUI(){
		if (!userPressedBack) return;
					
		Color oldGUIColor = GUI.color;
		// fade the game screen behind the GUI menu
		
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
		GUI.Box(box_ScreenRect, "Press Back Again to Exit the Game");

		GUI.matrix = oldMat;
	}

	void ClearUserPressedBack(){
		userPressedBack = false;
	}
	// ------------------------------------------------------------------------------------
	// Name	:	OnButtonSelect
	// Desc	:	Called by the Button3DObject's when they are selected with the mouse. The
	//			name of the button is passed.
	// ------------------------------------------------------------------------------------
	public override void OnButtonSelect( string buttonName )
	{
		// If this has been called and we are already processing an action return
		if (ActionSelected) return;
		
		// If the play buttton has been selected start the coroutine that animates the
		// camera through the hole in the wall and then load the next scene.
		if (buttonName=="Play")
		{
			ActionSelected = true;
			StartCoroutine(	LoadGameScene() );
		}
		// If the Quit buttton has been selected start the coroutine that fades out
		// and quites the game
		else if (buttonName=="Quit")
		{
			ActionSelected = true;
			StartCoroutine(	QuitGame() );
		}
//		else if (buttonName=="Next")
//		{
//			ActionSelected = true;
//			StartCoroutine(SetEvironmentImage(1, 0.375f));
//		}
//		else if (buttonName=="Prev")
//		{
//			ActionSelected = true;
//			StartCoroutine(SetEvironmentImage(-1, 0.375f));
//		}
		else if (buttonName=="Leaders")
		{
			if(loginState == GPLoginState.loggedout && !Social.localUser.authenticated){
				waitingToShowLeaderboard = true;
				// the user needs to sign in
				Social.localUser.Authenticate(OnAuthCB);
			}
			else{
				// show the actual leaderboard because we are signed in
				GooglePlayGames.PlayGamesPlatform.Instance.ShowLeaderboardUI();
			}
		}
		else if (buttonName=="Rate")
		{
			Application.OpenURL("https://play.google.com/store/apps/details?id=com.forwardthinking.peg");
			PlayerPrefs.SetInt("rated",1);
		}
		else if (buttonName=="Stats")
		{
			ActionSelected = true;
			StartCoroutine(	LoadStatsScene() );
		}
	} 
	
	// ------------------------------------------------------------------------------------
	// Name	:	LoadGameScene
	// Desc	:	Fades out the menu, animates the camera through the hole in the wall, and
	//			then load in the main game.
	// ------------------------------------------------------------------------------------
	private IEnumerator LoadGameScene()
	{	
		// Perform a 1 second fade-out of the menu
		float timer = 1.0f;
		
		// While the 1 second has still not expired
		while (timer>0.0f)
		{
			// Update the timer
			timer-=Time.deltaTime;
			
			// Fade out the menu
			ScreenFade = timer;
			
			// Yield control
			yield return null;
		}

		// We have now faded out the menu and moved the camera between the hole in the wall
		// so we are now in a black-out and can load the game scene.
		Application.LoadLevel(data.LevelToLoad);
	}

	// ------------------------------------------------------------------------------------
	// Name	:	LoadStatsScene
	// Desc	:	Fades out the menu and loads the stats scene
	// ------------------------------------------------------------------------------------
	private IEnumerator LoadStatsScene()
	{	
		// Perform a 1 second fade-out of the menu
		float timer = 1.0f;
		
		// While the 1 second has still not expired
		while (timer>0.0f)
		{
			// Update the timer
			timer-=Time.deltaTime;
			
			// Fade out the menu
			ScreenFade = timer;
			
			// Yield control
			yield return null;
		}
		
		// We have now faded out the menu and moved the camera between the hole in the wall
		// so we are now in a black-out and can load the game scene.
		Application.LoadLevel("Stats Scene");
	}

	// ------------------------------------------------------------------------------------
	// Name	:	LoadGameScene
	// Desc	:	Fades out the menu, animates the camera through the hole in the wall, and
	//			then load in the main game.
	// ------------------------------------------------------------------------------------
	private IEnumerator LoadEnvironmentMenu()
	{	
		Debug.Log("load environment");
		// Perform a 1 second fade-out of the menu
		float timer = 1.0f;
		
		// While the 1 second has still not expired
		while (timer>0.0f)
		{
			// Update the timer
			timer-=Time.deltaTime;
			
			// Fade out the menu
			ScreenFade = timer;
			
			// Yield control
			yield return null;
		}
		
		// We have now faded out the menu and moved the camera between the hole in the wall
		// so we are now in a black-out and can load the game scene.
		Application.LoadLevel("Environment Menu");
	}

	// ------------------------------------------------------------------------------------
	// Name	:	QuitGame
	// Desc	:	Fades out the menu, and then quits the game
	// ------------------------------------------------------------------------------------
	private IEnumerator QuitGame(){
		// Perform a 1 second fade-out of the menu
		float timer = 1.0f;
		
		// While the 1 second has still not expired
		while (timer>0.0f)
		{
			// Update the timer
			timer-=Time.deltaTime;
			
			// Fade out the menu
			ScreenFade = timer;
			
			// Yield control
			yield return null;
		}

		// We have now faded out the menu and moved the camera between the hole in the wall
		// so we are now in a black-out and can load the game scene.
		Application.Quit();
	}

	public override void SetLevelToLoad(string levelToLoad){
		Debug.Log (levelToLoad);
		data.LevelToLoad = levelToLoad;
	}

//
//	// ------------------------------------------------------------------------------------
//	// Name	:	NextEnvironmentImage
//	// Desc	:	Slides the next image in and the current image to the previous image position
//	// ------------------------------------------------------------------------------------
//	private IEnumerator SetEvironmentImage(int index, float time)
//	{		
//		if (index > 0 && activeIndex < EnvironmentList.Count-1){
//			activeIndex++;
//		}
//		else if (index < 0 && activeIndex > 0){
//			activeIndex--;
//		}
//		else{
//			Debug.Log("break");
//			yield break;
//		}
//	
//		// set the activeEnvironment in the persistent data storage class
//		data.ActiveEnvironment = activeIndex;
//		HideButtons(activeIndex);
//
//		float elapsedTime = 0.0f;
//		
//		for (int x = 0; x < StartingPositions.Count; x++){
//			StartingPositions[x] = EnvironmentList[x].position;
//		}
//		
//		while(elapsedTime <= time){
//			for (int i = 0; i < EnvironmentList.Count; i++){
//				if (i < activeIndex){ // lerp it to the left
//					EnvironmentList[i].position = Vector3.Lerp(StartingPositions[i], prevPosition, elapsedTime/time);
//				}
//				else if (i > activeIndex){
//					EnvironmentList[i].position = Vector3.Lerp(StartingPositions[i], nextPosition, elapsedTime/time);
//				}
//				else {
//					EnvironmentList[i].position = Vector3.Lerp(StartingPositions[i], curPosition, elapsedTime/time);
//				}
//				
//			}
//			elapsedTime += Time.deltaTime;
//			yield return null;
//		} 
//		// force the active selection to the exact spot
//		EnvironmentList[activeIndex].position = curPosition;
//		
//	}
//
//	private void HideButtons(int index){
//		if (index == 0){
//			previousButton.SetActive(false);
//			nextButton.SetActive(true);
//		}
//		else if (index == EnvironmentList.Count-1){
//			previousButton.SetActive(true);
//			nextButton.SetActive(false);
//		}
//		else{
//			previousButton.SetActive(true);
//			nextButton.SetActive(true);
//		}
//	}

	#region GPG_INTEGRATION_CALLBACKS
	void OnAuthCB(bool result)
	{
		Debug.Log("GPGUI: Got Login Response: " + result);
		if (result)
		{
			loginState = GPLoginState.loggedin;
			PlayerPrefs.SetInt("logonStatus",1);

			if (waitingToShowLeaderboard){
				// show the actual leaderboard because we are signed in
				GooglePlayGames.PlayGamesPlatform.Instance.ShowLeaderboardUI();
				waitingToShowLeaderboard = false;
			}
		}
		else{
			loginState = GPLoginState.loggedout;
			PlayerPrefs.SetInt("logonStatus",0);
		}
	}
	#endregion
}
