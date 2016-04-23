using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

// ---------------------------------------------------------------------------------------
// Class : SceneManager_TitleScreen
// Desc	 : Manages the sequence of events on the title screen and reacts to button 
//		   button selections
// ---------------------------------------------------------------------------------------
public class SceneManager_Stats : SceneManager_Base 
{
	// set by inspector
	public List<GUIText> timeTexts = null;
	public List<GUIText> timeTextsSquare = null;
	public List<GUIText> timeTextsEuro = null;

	public List<GUIText> stats = null;
	public List<GUIText> statsSquare = null;
	public List<GUIText> statsEuro = null;

	private PersistentData data = null;

	void Awake(){
		data = PersistentData.Instance;
	}

	void Start(){
		if (stats.Count != 5 || statsSquare.Count != 5 || statsEuro.Count != 5){
			Debug.Log("there should be 5 items in stats list");
		}
		else {
			Debug.Log(stats.Count.ToString());
			for(int i = 0; i < stats.Count; i++ ){
				Debug.Log(data.gameStats[i].ToString());
				stats[i].text = data.gameStats[i].ToString();
			}
			for(int i = 0; i < statsSquare.Count; i++ ){
				Debug.Log(data.gameStatsSquare[i].ToString());
				statsSquare[i].text = data.gameStatsSquare[i].ToString();
			}
			for(int i = 0; i < statsEuro.Count; i++ ){
				Debug.Log(data.gameStatsEuro[i].ToString());
				statsEuro[i].text = data.gameStatsEuro[i].ToString();
			}
		}

		if (timeTexts.Count != 5 || timeTextsSquare.Count != 5 || timeTextsEuro.Count != 5){
			Debug.Log("there should be 5 items in times list");
		}
		else{
			TimeSpan t;
			for(int i = 0; i < timeTexts.Count; i++ ){
				if (data.fastestTimes[i] > 0){
					t = TimeSpan.FromMilliseconds((double)data.fastestTimes[i]);
					timeTexts[i].text = string.Format("{0:00}:{1:00}:{2:00}.{3:0}",t.Hours,t.Minutes,t.Seconds,t.Milliseconds/100);
				}
				else{
					timeTexts[i].text = "N/A";
				}
			}
			for(int i = 0; i < timeTextsSquare.Count; i++ ){
				if (data.fastestTimesSquare[i] > 0){
					t = TimeSpan.FromMilliseconds((double)data.fastestTimesSquare[i]);
					timeTextsSquare[i].text = string.Format("{0:00}:{1:00}:{2:00}.{3:0}",t.Hours,t.Minutes,t.Seconds,t.Milliseconds/100);
				}
				else{
					timeTextsSquare[i].text = "N/A";
				}
			}
			for(int i = 0; i < timeTextsEuro.Count; i++ ){
				if (data.fastestTimesEuro[i] > 0){
					t = TimeSpan.FromMilliseconds((double)data.fastestTimesEuro[i]);
					timeTextsEuro[i].text = string.Format("{0:00}:{1:00}:{2:00}.{3:0}",t.Hours,t.Minutes,t.Seconds,t.Milliseconds/100);
				}
				else{
					timeTextsEuro[i].text = "N/A";
				}
			}
		}
	}

	void Update () {
		if (Input.GetKeyDown(KeyCode.Escape)) {
			Application.LoadLevel("Main Menu");
		}
	}
}
