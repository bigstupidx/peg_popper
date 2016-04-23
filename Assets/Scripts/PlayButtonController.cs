using UnityEngine;
using System.Collections;

public class PlayButtonController : MonoBehaviour {
	private GUITexture myGUITexture = null;

	void Start(){
		myGUITexture = GetComponent<GUITexture>();
	}

	void Update(){
		if (myGUITexture.HitTest(Input.mousePosition))
		{
			if(Input.GetMouseButtonDown(0))
			{
				Application.LoadLevel(Application.loadedLevelName);
			}
		} 
	}

}
