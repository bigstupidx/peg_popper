using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// --------------------------------------------------------------------------------------------
// Class : SceneManager_Base
// Desc	 : Abtract base class that all our scene managers are derived from. It specifies the
//		   interface and implements the singleton design pattern for finding any objects
//		   derived from this type in the scene.
// --------------------------------------------------------------------------------------------
public class PersistentData : MonoBehaviour 
{
	private int activeEnvironment = 0;
	private string levelToLoad = "Main Game";

	// game stats
	public int[] fastestTimes;
	public int[] gameStats;
	public int[] fastestTimesSquare;
	public int[] gameStatsSquare;
	public int[] fastestTimesEuro;
	public int[] gameStatsEuro;
	public int[] achievements;

	// social countdowns and flags
	public int showAdCountdown;
	public int showAchievementCountdown;
	public int userWantsAchievements;

	// Static Singleton Instance
	private static PersistentData _Instance		= null;

	// Property to get instance
	public static PersistentData Instance
	{
		get { 
				// If we don't an instance yet find it in the scene hierarchy
			if (_Instance==null) { _Instance = (PersistentData)FindObjectOfType(typeof(PersistentData)); }
				
				// Return the instance
				return _Instance;
			}
	}

	void Awake(){
		PersistentData[] objects = FindObjectsOfType<PersistentData>();
		if (objects.Length > 1)
		{
			// destroy the existing one
			DestroyImmediate(objects[0].gameObject);
		}

		fastestTimes = new int[5];
		gameStats = new int[5];

		fastestTimesSquare = new int[5];
		gameStatsSquare = new int[5];

		fastestTimesEuro = new int[5];
		gameStatsEuro = new int[5];

		achievements = new int[10];

//		PlayerPrefs.SetInt("fastest",0);
//		PlayerPrefs.SetInt("second",0);
//		PlayerPrefs.SetInt("third",0);
//		PlayerPrefs.SetInt("fourth",0);
//		PlayerPrefs.SetInt("fifth",0);
//
//		PlayerPrefs.SetInt("played",0);
//		PlayerPrefs.SetInt("completed",0);
//		PlayerPrefs.SetInt("threeLeft",0);
//		PlayerPrefs.SetInt("twoLeft",0);
//		PlayerPrefs.SetInt("oneLeft",0);
//
//		PlayerPrefs.SetInt("showAd",5);
//		PlayerPrefs.SetInt("showAchievment",0);
//		PlayerPrefs.SetInt("userAchievements",1);
	}

	void Start(){
		DontDestroyOnLoad(gameObject);

		// load fastest times
		fastestTimes[0] = PlayerPrefs.GetInt("fastest");
		fastestTimes[1] = PlayerPrefs.GetInt("second");
		fastestTimes[2] = PlayerPrefs.GetInt("third");
		fastestTimes[3] = PlayerPrefs.GetInt("fourth");
		fastestTimes[4] = PlayerPrefs.GetInt("fifth");

		// load the game times
		gameStats[0] = PlayerPrefs.GetInt("played");
		gameStats[1] = PlayerPrefs.GetInt("completed");
		gameStats[2] = PlayerPrefs.GetInt("threeLeft");
		gameStats[3] = PlayerPrefs.GetInt("twoLeft");
		gameStats[4] = PlayerPrefs.GetInt("oneLeft");

		fastestTimesSquare[0] = PlayerPrefs.GetInt("fastest_square");
		fastestTimesSquare[1] = PlayerPrefs.GetInt("second_square");
		fastestTimesSquare[2] = PlayerPrefs.GetInt("third_square");
		fastestTimesSquare[3] = PlayerPrefs.GetInt("fourth_square");
		fastestTimesSquare[4] = PlayerPrefs.GetInt("fifth_square");
		
		// load the game times
		gameStatsSquare[0] = PlayerPrefs.GetInt("played_square");
		gameStatsSquare[1] = PlayerPrefs.GetInt("completed_square");
		gameStatsSquare[2] = PlayerPrefs.GetInt("threeLeft_square");
		gameStatsSquare[3] = PlayerPrefs.GetInt("twoLeft_square");
		gameStatsSquare[4] = PlayerPrefs.GetInt("oneLeft_square");

		fastestTimesEuro[0] = PlayerPrefs.GetInt("fastest_euro");
		fastestTimesEuro[1] = PlayerPrefs.GetInt("second_euro");
		fastestTimesEuro[2] = PlayerPrefs.GetInt("third_euro");
		fastestTimesEuro[3] = PlayerPrefs.GetInt("fourth_euro");
		fastestTimesEuro[4] = PlayerPrefs.GetInt("fifth_euro");
		
		// load the game times
		gameStatsEuro[0] = PlayerPrefs.GetInt("played_euro");
		gameStatsEuro[1] = PlayerPrefs.GetInt("completed_euro");
		gameStatsEuro[2] = PlayerPrefs.GetInt("threeLeft_euro");
		gameStatsEuro[3] = PlayerPrefs.GetInt("twoLeft_euro");
		gameStatsEuro[4] = PlayerPrefs.GetInt("oneLeft_euro");

		//Social stats
		showAdCountdown = PlayerPrefs.GetInt("showAd",5);
		showAchievementCountdown = PlayerPrefs.GetInt("showAchievment",0);
		userWantsAchievements = PlayerPrefs.GetInt("userAchievements",1);

		achievements[0] = PlayerPrefs.GetInt("userHasRookie");
		achievements[1] = PlayerPrefs.GetInt("userHasTripod");
		achievements[2] = PlayerPrefs.GetInt("userHasBipeggle");
		achievements[3] = PlayerPrefs.GetInt("userHasNumeroUno");
		achievements[4] = PlayerPrefs.GetInt("userHasTheBestAround");
		achievements[5] = PlayerPrefs.GetInt("userHasProPlayer");
		achievements[6] = PlayerPrefs.GetInt("userHasThreeSquared");
		achievements[7] = PlayerPrefs.GetInt("userHasTwiceAsNice");
		achievements[8] = PlayerPrefs.GetInt("userHasHoleInOne");
		achievements[9] = PlayerPrefs.GetInt("userHasQuickDraw");

	}

	public int ActiveEnvironment{
		get{ return activeEnvironment; }
		set{
			//if (value >= 0 && value < skyboxList.Count){
				activeEnvironment = value;
			//}
		}
	}

	public string LevelToLoad{ 
		get{ return levelToLoad; } 
		set { levelToLoad = value; } 
	}
}
