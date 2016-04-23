using UnityEngine;
using System.Collections;

public class BoardselectScript : MonoBehaviour {

	public GUITexture mainBoardImage = null;

	private GUITexture myGUITexture = null;
	private SceneManager_Base scene = null;

	void Start(){
		myGUITexture = GetComponent<GUITexture>();
		scene = SceneManager_Base.Instance;
	}
	void Update(){
		if(myGUITexture.GetScreenRect().Contains(Input.mousePosition)){
			// Was left mouse button pressed while howevering over us
			if (Input.GetMouseButtonDown(0)){
				mainBoardImage.texture = myGUITexture.texture;

				// send the onbutton message
				scene.SetLevelToLoad(name);
			}
		}
	}
}
