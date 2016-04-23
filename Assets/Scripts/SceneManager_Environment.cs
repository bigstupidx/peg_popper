using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// ---------------------------------------------------------------------------------------
// Class : SceneManager_TitleScreen
// Desc	 : Manages the sequence of events on the title screen and reacts to button 
//		   button selections
// ---------------------------------------------------------------------------------------
public class SceneManager_Environment : SceneManager_Base 
{
	// set by inspector
	public List<Transform> EnvironmentList;
	public Vector3 prevPosition = new Vector3(0.0f, 0.0f, 0.0f);	// where the prev texture is
	public Vector3 nextPosition = new Vector3(0.0f, 0.0f, 0.0f);	// where the next texture is
	public Vector3 curPosition = new Vector3(0.0f, 0.0f, 0.0f);

	private int activeIndex = 0;		// active index into the list
	private List<Vector3> StartingPositions;

	private PersistentData data = null;

	void Awake(){
		Debug.Log(EnvironmentList.Count);
		StartingPositions = new List<Vector3>(EnvironmentList.Count);
		for (int x = 0; x < EnvironmentList.Count; x++){
			StartingPositions.Add(EnvironmentList[x].transform.position);
		}

		data = PersistentData.Instance;
	}

	void Start(){
		StartCoroutine(FadeInScene());
	}

	
	// ------------------------------------------------------------------------------------
	// Name	:	OnButtonSelect
	// Desc	:	Called by the Button3DObject's when they are selected with the mouse. The
	//			name of the button is passed.
	// ------------------------------------------------------------------------------------
	public override void OnButtonSelect( string buttonName )
	{
		// If the play buttton has been selected start the coroutine that animates the
		// camera through the hole in the wall and then load the next scene.
		if (buttonName=="Next")
		{
			StartCoroutine(SetEvironmentImage(1, 1.0f));
		}
		// If the play buttton has been selected start the coroutine that animates the
		// camera through the hole in the wall and then load the next scene.
		else if (buttonName=="Prev")
		{
			StartCoroutine(SetEvironmentImage(-1, 1.0f));
		}
	} 
	
	// ------------------------------------------------------------------------------------
	// Name	:	NextEnvironmentImage
	// Desc	:	Slides the next image in and the current image to the previous image position
	// ------------------------------------------------------------------------------------
	private IEnumerator SetEvironmentImage(int index, float time)
	{		
		if (index > 0 && activeIndex < EnvironmentList.Count-1){
			activeIndex++;
		}
		else if (index < 0 && activeIndex > 0){
			activeIndex--;
		}
		else{
			Debug.Log("break");
			yield break;
		}

		// set the activeEnvironment in the persistent data storage class
		data.ActiveEnvironment = activeIndex;

		float elapsedTime = 0.0f;

		for (int x = 0; x < StartingPositions.Count; x++){
			StartingPositions[x] = EnvironmentList[x].position;
		}

		while(elapsedTime < time){
			for (int i = 0; i < EnvironmentList.Count; i++){
				if (i < activeIndex){ // lerp it to the left
					EnvironmentList[i].position = Vector3.Lerp(StartingPositions[i], prevPosition, elapsedTime/time);
				}
				else if (i > activeIndex){
					EnvironmentList[i].position = Vector3.Lerp(StartingPositions[i], nextPosition, elapsedTime/time);
				}
				else {
					EnvironmentList[i].position = Vector3.Lerp(StartingPositions[i], curPosition, elapsedTime/time);
				}

			}
			elapsedTime += Time.deltaTime;
			yield return null;
		} 

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
	}

}
