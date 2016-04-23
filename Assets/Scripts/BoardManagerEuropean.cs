using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


public class BoardManagerEuropean : MonoBehaviour {

	public List<GameObject> pegs;
	public GameState state = GameState.Playing;

	private HoleController[][] holes;

	public HoleController GetHole(int row, int col){
		HoleController temp = holes[row][col];
		return temp;
	}

	// Use this for initialization
	void Start () {
		holes = new HoleController[7][];
		for (int i = 0; i < holes.Length; i++){
			holes[i] = new HoleController[7];
		}

		// get references to all of the holes
		List<HoleController> tempHoles = new List<HoleController> (GameObject.FindObjectsOfType<HoleController>());

		foreach (HoleController hole in tempHoles){
			holes[hole.row][hole.col] = hole;
		}

		DestroyImmediate(pegs[Random.Range(0,37)]);
	}

	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.Escape)) {
			if (state == GameState.OtherGUI) return;
			Time.timeScale = 0.0f;
			state = GameState.Paused;
		}

		if (state != GameState.Playing) return;	

		bool validMove = false;
		foreach(GameObject peg in pegs){
			if (peg && CheckValidMoves(peg)){
				validMove = true;
				break;
			}
		}

		if(!validMove)
		{

			state = GameState.GameOver;
		}
	}
	
	bool CheckValidMoves(GameObject peg){
		PegController pegCont = peg.GetComponent<PegController>();
		HoleController hole = pegCont.originalHole;
		HoleController nextRow = getNextRow(hole);
		HoleController prevRow = getPrevRow(hole);
		HoleController nextCol = getNextCol(hole);
		HoleController prevCol = getPrevCol(hole);


		if(nextRow && nextRow.currentPeg)
		{
			HoleController nextNextRow = getNextRow(nextRow);
			if(nextNextRow && !nextNextRow.currentPeg)
			{
				return true;
			}
		}
		if(prevRow && prevRow.currentPeg)
		{
			HoleController prevPrevRow = getPrevRow(prevRow);
			if(prevPrevRow && !prevPrevRow.currentPeg)
			{
				return true;
			}
		}
		if(nextCol && nextCol.currentPeg)
		{
			HoleController nextNextCol = getNextCol(nextCol);
			if(nextNextCol && !nextNextCol.currentPeg)
			{
				return true;
			}
		}
		if(prevCol && prevCol.currentPeg)
		{
			HoleController prevPrevCol = getPrevCol(prevCol);
			if(prevPrevCol && !prevPrevCol.currentPeg)
			{
				return true;
			}
		}

		return false;
	}

	#region HOLE GETTING FUNCTIONS
	HoleController getPrevRow(HoleController current){
		if(current.row > 0){
			if(current.row==1){
				if(current.col==1 || current.col==5){
					return null;
				}
			}
			if(current.row==2){
				if(current.col==0 || current.col==6){
					return null;
				}
			}
			return holes[current.row-1][current.col];
		}
		else {
			return null;
		}
	}
	HoleController getNextRow(HoleController current){
		if(current.row < holes.Length-1){
			if(current.row==4){
				if(current.col==0 || current.col==6){
					return null;
				}
			}
			if(current.row==5){
				if( current.col==1 || current.col==5){
					return null;
				}
			}
			return holes[current.row+1][current.col];
		}
		else {
			return null;
		}
	}
	HoleController getPrevCol(HoleController current){
		if(current.col >0 ){
			if(current.col==1){
				if(current.row==1 || current.row==5){
					return null;
				}
			}
			if(current.col==2){
				if(current.row==0 || current.row==6){
					return null;
				}
			}
			return holes[current.row][current.col - 1];
		}
		else {
			return null;
		}
	}
	HoleController getNextCol(HoleController current){
		if(current.col < holes[current.row].Length-1 ){
			if(current.col==4){
				if(current.row==0 || current.row==6){
					return null;
				}
			}
			if(current.col==5){
				if(current.row==1 || current.row==5){
					return null;
				}
			}
			return holes[current.row][current.col+1];
		}
		else {
			return null;
		}
	}
	#endregion
}
