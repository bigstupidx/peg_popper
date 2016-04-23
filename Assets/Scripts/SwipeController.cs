using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SwipeController : MonoBehaviour {

	public Transform swipeContainer = null;
	public float swipeSensitivity = 20.0f; //min movement to be considered a swipe
	public int activeItem = 0;
	public float snapSpeed = 10.0f;

	private float startPosX;

	private Vector3 homePos;
	private List<Transform> childTransforms = new List<Transform>();

	private float itemSpacing;
	public float cumulativeTranslation = 0.0f;

//#if UNITY_EDITOR
	private bool dragStarted = false;
//#endif

	void Start(){
		swipeContainer = transform;
		//get the spacing between the child items
		itemSpacing = 0.0f;
		float prevX = 0.0f;

		// get and sort a list of the container children
		for (int i = 0; i < swipeContainer.childCount; ++i){
			childTransforms.Add (swipeContainer.GetChild(i));
		}
		childTransforms.Sort((x, y) => x.position.x.CompareTo(y.position.x));

		Transform child;
		for (int i = 0; i < childTransforms.Count; ++i){
			child = childTransforms[i];
			if (i == activeItem){
				homePos = child.position;
			}

			if (i==0){
				prevX = child.position.x;
			}
			else{
				itemSpacing += child.position.x - prevX;
				prevX = child.position.x;
			}
		}
		if (childTransforms.Count>1){
			itemSpacing /= (float)(childTransforms.Count - 1);
		}
	}

	void Update () {

//#if UNITY_EDITOR
		float deltaX = 0.0f;
		float screenDeltaX = 0.0f;

		if (Input.GetMouseButton(0)){
			if(!dragStarted){
				dragStarted = true;
				startPosX = Input.mousePosition.x;
			}
			else{
				deltaX = Input.mousePosition.x - startPosX;
				screenDeltaX = deltaX/Screen.width;
				cumulativeTranslation += screenDeltaX;

				if (swipeContainer != null){
					Transform child = null;
					for (int i = 0; i < childTransforms.Count; ++i){
						child = childTransforms[i];
						child.Translate(screenDeltaX, 0.0f, 0.0f);
					}
				}
				startPosX = Input.mousePosition.x;
			}

		}
		else{
			// finger was removed
			if (dragStarted==true){
				if(cumulativeTranslation > itemSpacing/2.0f ){
					//swipe right
					if (activeItem > 0){
						activeItem--;
					}
				}
				else if(Mathf.Abs(cumulativeTranslation) > itemSpacing/2.0f){
					//swipe left
					if (activeItem < swipeContainer.childCount - 1){
						activeItem++;
					}
				}
			}
			dragStarted = false;
			cumulativeTranslation = 0.0f;

			//lock the children to their final positions
			// should this be a coroutine to lerp to the position
			Transform child = null;
			for (int i = 0; i < childTransforms.Count; ++i){
				child = childTransforms[i];
				child.position = Vector3.Lerp (child.position, new Vector3(homePos.x+itemSpacing*((float)(i-activeItem)),homePos.y, homePos.z), Time.deltaTime*snapSpeed);
			}
		}
//#elif 0
//		foreach (var T in Input.touches){
//			Vector2 p = T.position;
//			if (T.phase == TouchPhase.Began %% fingerID == -1){
//				//this is the first swipe touch
//			 	fingerID = T.fingerId;
//				startPosX = p.x;
//			}
//			else if (T.fingerId == fingerID){
//				// this is the touch we are looking for
//				float deltaX = p.x - startPosX;
//				if (swipeContainer != null){
//					Transform child = null;
//					for (int i = 0; i < swipeContainer.childCount; ++i){
//						child = swipeContainer.GetChild(i);
//						child.position.x += deltaX;
//					}
//				}
//
//
//				if (deltaX/T.deltaTime > swipeSensitivity){
//					//we have a swipe in the X direction
//
//				}
//			}
//		}
//#endif
	}
}
