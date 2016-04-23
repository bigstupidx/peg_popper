using UnityEngine;
using System.Collections;

public class DeleteThisButtonController : MonoBehaviour {

	public Texture2D hoverTexture = null;

	private GUITexture myGUITexture = null;
	private Texture2D defaultTexture = null;

	void Start(){
		myGUITexture = GetComponent<GUITexture>();
		defaultTexture = (Texture2D)myGUITexture.texture;
	}
	
	IEnumerator OnMouseEnter()
	{
		float totalElapsedTime = 0.0f;
		float sectionElapsedTime = 0.0f;
		int textureIndex = 0;

		while(totalElapsedTime < 2.0f){
			if (sectionElapsedTime > 0.25f)
			{
				// animated the texture for 2 seconds
				if(textureIndex == 0)
				{
					textureIndex = 1;
					myGUITexture.texture = hoverTexture;
				} 
				else{
					textureIndex = 0;
					myGUITexture.texture = defaultTexture;
				}
				sectionElapsedTime = 0.0f;
			}
			sectionElapsedTime += Time.deltaTime;
			totalElapsedTime += Time.deltaTime;
			yield return null;
		}
	}

	void OnMouseExit()
	{
		StopCoroutine("OnMouseEnter");
		myGUITexture.texture = defaultTexture;
	}
}
