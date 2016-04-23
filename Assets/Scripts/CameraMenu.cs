using UnityEngine;
using System.Collections;

public class CameraMenu : MonoBehaviour {
	//set by inspector
	public Texture fadeTexture = null;

	// cached components
	private SceneManager_Base scene = null;

	void Awake(){
		scene = SceneManager_Base.Instance;

	}
	void OnGUI(){		
		Color oldGUIColor = GUI.color;
		// fade the game screen behind the GUI menu
		if (fadeTexture){
			GUI.color = new Color(0, 0, 0, 1.0f-scene.GetScreenFade());
			GUI.DrawTexture( new Rect(0, 0, Screen.width, Screen.height ), fadeTexture );
			
			GUI.color = oldGUIColor;
		}
	}
}
