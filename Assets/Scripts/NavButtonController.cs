using UnityEngine;
using System.Collections;

public class NavButtonController : MonoBehaviour {

	// set via inspector
	public float hoverScale = 1.0f;
	public float waitTime = 1.0f;

	// Has the button been selected
	private bool Selected =	false;

	private GUITexture myGUITexture = null;
	private Transform myTransform = null;
	private Vector3 hoverVector = new Vector3(1.0f, 1.0f, 1.0f);
	
	private SceneManager_Base scene = null;

	void Start(){
		scene = SceneManager_Base.Instance;

		myGUITexture = GetComponent<GUITexture>();
		myTransform = myGUITexture.transform;

		// set up the proper button aspect
		float aspect = Camera.main.aspect;
		Vector3 startingScale = myTransform.localScale;
		startingScale.y = startingScale.x*aspect;
		myTransform.localScale = startingScale;

		hoverVector.x = hoverScale;
		hoverVector.y = hoverScale*aspect;
	}

	void Update(){
		// set up the proper button aspect
		float aspect = Camera.main.aspect;
		Vector3 startingScale = myTransform.localScale;
		startingScale.y = startingScale.x*aspect;
		myTransform.localScale = startingScale;

		if(!Selected){
			if(myGUITexture.GetScreenRect().Contains(Input.mousePosition)){
				// Was left mouse button pressed while howevering over us
				if (Input.GetMouseButtonDown(0)){
					// grow the image
					myTransform.localScale += hoverVector;

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

		myTransform.localScale -= hoverVector;
	}
}
