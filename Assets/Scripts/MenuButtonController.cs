using UnityEngine;
using System.Collections;

public class MenuButtonController : MonoBehaviour {

	// set via inspector
	public Texture2D pressedTexture = null;
	public Texture2D disabledTexture = null;
	public float waitTime = 1.0f;
	public bool disabled = false;

	// Has the button been selected
	private bool Selected =	false;

	private Texture2D originalTexture = null;
	private GUITexture myGUITexture = null;
	
	private SceneManager_Base scene = null;

	void Start(){
		scene = SceneManager_Base.Instance;

		myGUITexture = GetComponent<GUITexture>();
		originalTexture = (Texture2D) myGUITexture.texture;
	}

	void Update(){
		if (disabled) {
			if (disabledTexture != null) {
				myGUITexture.texture = disabledTexture;
			}
			return;
		}
		if(!Selected){
			if(myGUITexture.GetScreenRect().Contains(Input.mousePosition)){
				// Was left mouse button pressed while howevering over us
				if (Input.GetMouseButtonDown(0)){
					// grow the image
					myGUITexture.texture = pressedTexture;

					// send the onbutton message
					scene.OnButtonSelect(name);
					Selected = true;

					// don't allow button to be repressed.
					if (waitTime > 0.0f) Invoke ("Unselect", waitTime);
				}
			}
		}
	}

	void Unselect(){
		scene.ActionSelected = false;
		Selected = false;

		myGUITexture.texture = originalTexture;
	}
}
