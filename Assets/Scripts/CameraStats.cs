using UnityEngine;
using System.Collections;

public class CameraStats : MonoBehaviour {

	public int selGridInt = 0;
	public string[] selStrings = new string[] {"Games Played", "99999999", "Games Completed", "99999999"};
	void OnGUI() {
		selGridInt = GUILayout.SelectionGrid(selGridInt, selStrings, 2);
	}
}
