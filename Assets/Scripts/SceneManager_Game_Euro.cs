using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

using GoogleMobileAds.Api;


// ---------------------------------------------------------------------------------------
// Class : SceneManager_TitleScreen
// Desc	 : Manages the sequence of events on the title screen and reacts to button 
//		   button selections
// ---------------------------------------------------------------------------------------
public class SceneManager_Game_Euro : SceneManager_Base 
{
	// set via inspector
	public List<Material> skyboxes;
	public List<GameObject> particles;
	public TextMesh gameOverText = null;
	public float gameOverTextOffset = 15;
	public TextMesh stopWatchText = null;

	public GUISkin myGUISkin = null;
	public Texture fadeTexture = null;

	private AdMobScript bannerAd = null;
	private AdMobInterstitialScript interstitialAd = null;

	private BoardManagerEuropean boardManager = null;
	private PersistentData data = null;

	private bool sceneEnded = false;
	private float stopWatch = 0.0f;
	private bool startTimer = false;

	//Achievement strings
	private string rookieAchievement = "CgkIgP7b-ckPEAIQAQ";
	private string threeSquaredAchievement = "CgkIgP7b-ckPEAIQDQ";
	private string twiceAsNiceAchievement = "CgkIgP7b-ckPEAIQDg";
	private string holeInOneAchievement = "CgkIgP7b-ckPEAIQDw";
	private string quickDrawAchievement = "CgkIgP7b-ckPEAIQEA";
	private string proPlayerAchievement = "CgkIgP7b-ckPEAIQDA";

	private string currentAchievement = null;
	private List<string> waitingAchievements = new List<string>();
	private bool showAchievementGUI = false;

	private string leaderboard1 = "CgkIgP7b-ckPEAIQEg";
	private string leaderboard2 = "CgkIgP7b-ckPEAIQCQ";
	private string leaderboard3 = "CgkIgP7b-ckPEAIQFA";
	private LeaderboardValue currentLeaderboard = null	;
	private List<LeaderboardValue> waitingLeaderboards = new List<LeaderboardValue>();
	private bool showLeaderboardGUI = false;

	private bool waitingToShowLeaderboard = false;
	private bool waitingToShowAchievements = false;
	private bool showReallyNever = false;

	// GUI regions
	private Rect	Achievement_ScreenRect	=	new Rect( 200, 620, 680, 680);	
	private Rect    Sure_ScreenRect			=	new Rect( 220, 1190, 200, 80); 	
	private Rect    NotNow_ScreenRect		=	new Rect( 440, 1190, 200, 80);	
	private Rect    Never_ScreenRect		=	new Rect( 660, 1190, 200, 80);	

	// stats array
	string[] stats;

	void Awake(){
		boardManager = (BoardManagerEuropean)FindObjectOfType(typeof(BoardManagerEuropean));
		data = PersistentData.Instance;

		// get a reference to the Millenial Ads
		bannerAd = FindObjectOfType<AdMobScript>();
		interstitialAd = FindObjectOfType<AdMobInterstitialScript>();

		// initialize the stats dictionary
		stats = new string[3];
		stats[2] = "threeLeft_euro";
		stats[1] = "twoLeft_euro";
		stats[0] = "oneLeft_euro";

	}

	void Start(){
		// move the banner to the top
		bannerAd.SetPosition(AdPosition.Top);

		int activeIndex = data.ActiveEnvironment;

		// show only the selected particle system and 
		// skybox
		RenderSettings.skybox = skyboxes[activeIndex];
		for(int i = 0; i < particles.Count; i++){
			particles[i].SetActive(i==activeIndex);
		}

		// make sure that the gameover text is hidden
		gameOverText.gameObject.SetActive(false);

		// get the stopwatch ready
		startTimer = false;
		stopWatch = 0.00f;

		// increase the games played count
		data.showAdCountdown-=2;
		PlayerPrefs.SetInt("played_euro",++data.gameStatsEuro[0]);
		PlayerPrefs.SetInt("showAd",data.showAdCountdown);
		PlayerPrefs.SetInt("showAchievment",--data.showAchievementCountdown);

		//start the game
		StartCoroutine(FadeInScene());
	}

	void Update(){
		if (boardManager.state == GameState.GameOver){
			if(!sceneEnded){
				if (data.showAdCountdown <= 0){
#if UNITY_EDITOR	
					Debug.Log("showing interstitial");
#elif UNITY_ANDROID
					StartCoroutine(ShowInterstitial());
#endif
				}

				bool wantsAchievements = false;
				if((data.userWantsAchievements != 0) && (data.showAchievementCountdown <= 0)){
					wantsAchievements = true;
					data.showAchievementCountdown = 0;
				}
				
				// increase the games played count
				PlayerPrefs.SetInt("completed_euro",++data.gameStatsEuro[1]);
				
				// check for game completed achievements
				// No achievements for the EURO board
//				if ( wantsAchievements ){
//					if ( (data.gameStats[1] + data.gameStatsSquare[1]) >= 5){
//						if (data.achievements[0] == 0) 
//							waitingAchievements.Add(rookieAchievement);
//					}
//					if ( (data.gameStats[1] >= 5) && (data.gameStatsSquare[1]) >= 5){
//						if (data.achievements[5] == 0) 
//							waitingAchievements.Add(proPlayerAchievement);
//					}
//				}
				
				// how many pegs are left andset the text
				PegController[] pegs = FindObjectsOfType<PegController>();
				
				// update the game stats
				int index = pegs.Length;
				if (index <= 3){
					Debug.Log("setting peg count: " + index);
					PlayerPrefs.SetInt(stats[index-1], ++data.gameStatsEuro[5-index]);
				}
				
				//if only one is left see if it is top 5 best time
				if (index == 1){
					int time = (int)(stopWatch*1000.0f);

					for (int i = 4; i >= 0; i--){
						if (time < data.fastestTimesEuro[i] || data.fastestTimesEuro[i] == 0){
							data.fastestTimesEuro[i] = time;
							Array.Sort(data.fastestTimesEuro, CustomSort);

							PlayerPrefs.SetInt("fastest_euro", data.fastestTimesEuro[0]);
							PlayerPrefs.SetInt("second_euro", data.fastestTimesEuro[1]);
							PlayerPrefs.SetInt("third_euro", data.fastestTimesEuro[2]);
							PlayerPrefs.SetInt("fourth_euro", data.fastestTimesEuro[3]);
							PlayerPrefs.SetInt("fifth_euro", data.fastestTimesEuro[4]);

							// break the loop so we don't overwrite them all
							break;
						}
					}
				}
				// peg count achievements
				// less than 3
				if (wantsAchievements) {
					//games completed leaderboard
					waitingLeaderboards.Add (new LeaderboardValue(leaderboard2, data.gameStats[1]+data.gameStatsSquare[1]+data.gameStatsEuro[1]));

					// no achievements for this board yet
//					if (index <= 3){
//						if(data.achievements[6]==0)
//							waitingAchievements.Add(threeSquaredAchievement);
//					}
//					if (index <= 2){
//						if(data.achievements[7]==0)
//							waitingAchievements.Add(twiceAsNiceAchievement);
//					}
					if (index <= 1){
//						if(data.achievements[8]==0)
//							waitingAchievements.Add(holeInOneAchievement);

						// fastest times leaderboard
						waitingLeaderboards.Add (new LeaderboardValue(leaderboard1, (int)(stopWatch*1000.0f)));
						// most times leaving one peg leaderboard
						waitingLeaderboards.Add (new LeaderboardValue(leaderboard3, data.gameStatsEuro[4]));

						// no achievements for this board style yet
//						if (stopWatch <= 60.0f)
//						{
//							//1 peg  under 60 seconds achievmenet
//							if(data.achievements[9]==0)
//								waitingAchievements.Add (quickDrawAchievement);
//						}
					}
					if(waitingAchievements.Count > 0){
						if (!Social.localUser.authenticated){
							//ask user to sign in
							currentAchievement = waitingAchievements[0];
							waitingAchievements.Remove(waitingAchievements[0]);

							showAchievementGUI = true;
							boardManager.state = GameState.OtherGUI;
						}
						else {
							currentAchievement = waitingAchievements[0];
							waitingAchievements.Remove(waitingAchievements[0]);
							UserUnlockedAchievement(currentAchievement);
						}
					}
					if(waitingLeaderboards.Count > 0){
						if (!Social.localUser.authenticated){
							//ask user to sign in
							currentLeaderboard = waitingLeaderboards[0];
							waitingLeaderboards.Remove(waitingLeaderboards[0]);
							
							showLeaderboardGUI = true;
							boardManager.state = GameState.OtherGUI;
						}
						else {
							currentLeaderboard = waitingLeaderboards[0];
							waitingLeaderboards.Remove(waitingLeaderboards[0]);
							SubmitLeaderboardScore(currentLeaderboard);
						}
					}


					data.showAchievementCountdown = 0;
				}
				
				// animate peg left count
				gameOverText.gameObject.SetActive(true);
				gameOverText.text = pegs.Length+ " Left\nTry Again";
				StartCoroutine(AnimateText());
				sceneEnded = true;
			}
		}
		else if (boardManager.state != GameState.OtherGUI) {
			if(startTimer){
				stopWatch += Time.deltaTime;
				stopWatchText.text = stopWatch.ToString("0.0");
			}
		}
	}

	void OnGUI(){
		if (!showAchievementGUI && !showLeaderboardGUI && !showReallyNever) return;

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

		string windowString = "Congratulations!  Sign in with Google+ to unlock Achievements and Leaderboards";
		string sureButton = "Sure Ach";

		if (showAchievementGUI){
			windowString = "Congratulations, you just unlocked an achievement. Sign in with Google+ to unlock the achievement, and join the leaderboards to see how your friends are doing!";
			sureButton = "Sure Ach";
		}
		else if(showLeaderboardGUI)
		{
			windowString = "Hey you are moving up the leaderboards.  Why not sign in with Google+ and see how you compare with all of your friends and circles and unlock Achievements!?";
			sureButton = "Sure Leader";
		}

		if(showReallyNever){
			// Create the transparent options box below the header
			GUI.Box(Achievement_ScreenRect, "Are you sure you don't want to ever sign in? You will not be able to unlock achievements or join the leaderboards.  This cannot be undone.");
			// Create and react to the Continue button being pressed
			if (GUI.Button( Sure_ScreenRect, "I'm Sure"))		
			{
				OnButtonSelect("Really Never");
			}
			// Create and react to the Quit button
			if (GUI.Button( NotNow_ScreenRect, "Later")) 
			{
				OnButtonSelect("Not Now");
			}
			// Create and react to the Quit button
			if (GUI.Button( Never_ScreenRect, "Sign In")) 
			{
				OnButtonSelect(sureButton);
			}
		}		
		else if(showAchievementGUI || showLeaderboardGUI){
			// Create the transparent options box below the header
			GUI.Box(Achievement_ScreenRect, windowString);
			
			// Create and react to the Continue button being pressed
			if (GUI.Button( Sure_ScreenRect, "Sure"))		
			{
				OnButtonSelect(sureButton);
			}
			// Create and react to the Quit button
			if (GUI.Button( NotNow_ScreenRect, "Not Now")) 
			{
				OnButtonSelect("Not Now");
			}
			// Create and react to the Quit button
			if (GUI.Button( Never_ScreenRect, "Never")) 
			{
				OnButtonSelect("Never");
			}
		}

		// Restore the matrix
		GUI.matrix = oldMat;

	}

	public override void OnButtonSelect( string buttonName ){
		if(buttonName == "Continue"){
			// Make sure time scale is resumed to normal as this
			// was set to zero for pause mode
			Time.timeScale = 1.0f;
			boardManager.state = GameState.Playing;
		}
		else if (buttonName == "Main Menu"){
			Time.timeScale = 1.0f;
			Application.LoadLevel("Main Menu");
		}
		else if (buttonName == "Sure Ach"){
			showAchievementGUI = false;
			showLeaderboardGUI = false;
			showReallyNever = false;

			// sign the user in and then load the appropriate achievement
			// may have been signed in since this request
			if(!Social.localUser.authenticated)Social.localUser.Authenticate(OnAuthCB);

			boardManager.state = GameState.Playing;
		}
		else if (buttonName == "Sure Leader"){
			showLeaderboardGUI = false;
			showAchievementGUI = false;
			showReallyNever = false;

			// sign the user in and then load the appropriate achievement
			// may have been signed in since this request
			if(!Social.localUser.authenticated)Social.localUser.Authenticate(OnAuthCB);

			boardManager.state = GameState.Playing;
		}
		else if (buttonName == "Not Now"){
			// give the user 5 more games until asking about achievements again
			PlayerPrefs.SetInt("showAchievement", 8);
			data.showAchievementCountdown = 8;

			showAchievementGUI = false;
			showLeaderboardGUI = false;
			showReallyNever = false;
			boardManager.state = GameState.Playing;
		}
		else if (buttonName == "Never"){
			// user doesn't want achievements so don't bother them
			showLeaderboardGUI = false;
			showAchievementGUI = false;

			showReallyNever = true;
		}
		else if (buttonName == "Really Never"){
			// user doesn't want achievements so don't bother them
			showLeaderboardGUI = false;
			showAchievementGUI = false;
			showReallyNever = false;

			currentAchievement = null;
			waitingAchievements.Clear();

			currentLeaderboard = null;
			waitingLeaderboards.Clear();

			PlayerPrefs.SetInt ("userAchievements", 0);
			data.userWantsAchievements = 0;
			boardManager.state = GameState.Playing;
		}
		// testing
		else if (buttonName == "Achievements"){
			if(loginState == GPLoginState.loggedout && !Social.localUser.authenticated){
				waitingToShowAchievements = true;
				// the user needs to sign in
				Social.localUser.Authenticate(OnAuthCB);
			}
			else{
				// show the achievements we are signed in
				Social.ShowAchievementsUI();
			}
		}
		else if (buttonName == "Leaders"){
			if(loginState == GPLoginState.loggedout && !Social.localUser.authenticated){
				waitingToShowLeaderboard = true;
				// the user needs to sign in
				Social.localUser.Authenticate(OnAuthCB);
			}
			else{
				// show the actual leaderboard because we are signed in
				Social.ShowLeaderboardUI();
			}
		}
//		else if (buttonName=="Signout")
//		{
//			((GooglePlayGames.PlayGamesPlatform) Social.Active).SignOut();
//			loginState = GPLoginState.loggedout;
//			PlayerPrefs.SetInt("logonStatus",0);
//		}

	}
	// ------------------------------------------------------------------------------------
	// Name	:	FadeInScene
	// Desc	:	Fades in environment sceen
	// ------------------------------------------------------------------------------------
	private IEnumerator FadeInScene()
	{	
		// Perform a 1 second fade-out of the menu
		float timer = 1.0f;
		
		// While the 1 second has still not expired
		while (timer>0.0f)
		{
			// Update the timer
			timer-=Time.deltaTime;
			
			// Fade out the menu
			ScreenFade = 1.0f - timer;
			
			// Yield control
			yield return null;
		}

		startTimer = true;
	}

	private IEnumerator AnimateText()
	{	
		// Perform a 1 second fade-out of the menu
		float timer = 1.0f;
		Transform textTransform = gameOverText.transform;
		Vector3 destination;

		while (true){
			destination = textTransform.position + new Vector3(0.0f, 0.0f, gameOverTextOffset);

			// While the 1 second has still not expired
			while (timer>0.0f)
			{
				textTransform.position = Vector3.Lerp( textTransform.position, destination, Time.deltaTime*2.0f );
				timer -= Time.deltaTime;

				// Yield control
				yield return null;
			}

			destination = textTransform.position - new Vector3(0.0f, 0.0f, gameOverTextOffset);
			while (timer<1.0f)
			{
				textTransform.position = Vector3.Lerp( textTransform.position, destination, Time.deltaTime*2.0f );
				timer += Time.deltaTime;
				
				// Yield control
				yield return null;
			}
		}
	}

	private IEnumerator ResetAchievement(string achString){

//		// authenticate the user
//		string authString = "https://accounts.google.com/o/oauth2/auth?";
//		authString += "scope=games&";
//		authString += "redirect_uri=urn:ietf:wg:oauth:2.0:oob";
//		authString +=  "response_type=code&";
//		authString += "client_id=535247126272.apps.googleusercontent.com";
//
//		WWW auth = new WWW(authString);
//
//		yield return auth;
//
//		// check for errors
//		if (auth.error == null){
//			Debug.Log("WWW Ok: " + auth.text);
//		} 
//		else {
//			Debug.Log("WWW Error: "+ auth.error);
//		} 

		// reset the achievement using the REST Api
		WWWForm form = new WWWForm();
		form.AddField("na","na");

		string URL = "https://www.googleapis.com/games/v1management/achievements/"+achString+"/reset?key=AIzaSyAPEVyrkb3V9-5dkUeBduGtFjmNAAhEpOg";
		WWW link = new WWW(URL,form);

		yield return link;

		// check for errors
		if (link.error == null){
			Debug.Log("WWW Ok: " + link.text);
		} 
		else {
			Debug.Log("WWW Error: "+ link.error);
		} 
	}

	public int CustomSort(int x, int y){
		if (x == 0){
			if (y == 0)	return 0;
			return 1;
		}
		else if(y == 0){
			if (x==0) return 0;
			return -1;
		}
		else{
			return x.CompareTo(y);
		}
	}

	private IEnumerator ShowInterstitial(){
		// reset the ad countdown to 3 games
		ScreenFade = 0.3f;
		
		// hide the GUI
		showingAd = true;
		
		if (interstitialAd.IsLoaded()) {
			data.showAdCountdown = 5;
			PlayerPrefs.SetInt("showAd",5);
			
			interstitialAd.ShowAd();
			while (!interstitialAd.returnToScene) {
				yield return null;
			}
		}

		// Request a banner ad, with optional custom ad targeting.
		AdRequest request = new AdRequest.Builder()
			.AddTestDevice("719B63AE6F8DED7AFB9B7D38EEFAF5A8")
				.Build();
		interstitialAd.LoadAd(request);
		showingAd = false;
		ScreenFade = 1.0f;
	}

	#region GPG_INTEGRATION_CALLBACKS
	void OnAuthCB(bool result)
	{
		Debug.Log("GPGUI: Got Login Response: " + result);
		if (result)
		{
			loginState = GPLoginState.loggedin;
			PlayerPrefs.SetInt("logonStatus",1);

			// if there is an achievement waiting then report it
			if(currentAchievement != null){
				UserUnlockedAchievement(currentAchievement);
			}

			if(currentLeaderboard != null){
				SubmitLeaderboardScore(currentLeaderboard);
			}

			if (waitingToShowLeaderboard){
				// show the actual leaderboard because we are signed in
				GooglePlayGames.PlayGamesPlatform.Instance.ShowLeaderboardUI();
				waitingToShowLeaderboard = false;
			}

			if (waitingToShowAchievements){
				// show the actual leaderboard because we are signed in
				GooglePlayGames.PlayGamesPlatform.Instance.ShowAchievementsUI();
				waitingToShowAchievements = false;
			}

		}
		else{
			loginState = GPLoginState.loggedout;
			PlayerPrefs.SetInt("logonStatus",0);
		}
	}

	void UserUnlockedAchievement(string achievement){
		Social.ReportProgress(achievement, 100.0f, OnReportAchievement);
	}

	void SubmitLeaderboardScore(LeaderboardValue leaderboardScore){
		Social.ReportScore(leaderboardScore.score, leaderboardScore.leaderboardID, OnReportScore);
	}

	private void OnReportAchievement(bool result){
		Debug.Log(result);
		if (result){
			if (currentAchievement == rookieAchievement){
				data.achievements[0] = 1;
				PlayerPrefs.SetInt("userHasRookie",1);
			}
			else if (currentAchievement == threeSquaredAchievement){
				data.achievements[6] = 1;
				PlayerPrefs.SetInt("userHasThreeSquared",1);
			}
			else if (currentAchievement == twiceAsNiceAchievement){
				data.achievements[7] = 1;
				PlayerPrefs.SetInt("userHasTwiceAsNice",1);
			}
			else if (currentAchievement == holeInOneAchievement){
				data.achievements[8] = 1;
				PlayerPrefs.SetInt("userHasHoleInOne",1);
			}
			else if (currentAchievement == quickDrawAchievement){
				data.achievements[9] = 1;
				PlayerPrefs.SetInt("userHasQuickDraw",1);
			}
			else if (currentAchievement == proPlayerAchievement){
				data.achievements[5] = 1;
				PlayerPrefs.SetInt("userHasProPlayer",1);
			}
		}
		if(waitingAchievements.Count > 0){
			currentAchievement = waitingAchievements[0];
			waitingAchievements.Remove(waitingAchievements[0]);
			UserUnlockedAchievement(currentAchievement);
		}
		else{
			currentAchievement = null;
		}

	}

	private void OnReportScore(bool result){
		Debug.Log(result);
		if(waitingLeaderboards.Count > 0){
			currentLeaderboard = waitingLeaderboards[0];
			waitingLeaderboards.Remove(waitingLeaderboards[0]);
			SubmitLeaderboardScore(currentLeaderboard);
		}
		else{
			currentLeaderboard = null;
		}
		
	}
	#endregion
}
