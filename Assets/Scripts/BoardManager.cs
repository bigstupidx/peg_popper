using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public enum GameState { Playing, GameOver, Paused, OtherGUI };

public class BoardManager : MonoBehaviour {

	public List<GameObject> pegs;
	public GameState state = GameState.Playing;

	private HoleController[][] holes;

	public HoleController GetHole(int row, int col){
		HoleController temp = holes[row][col];
		return temp;
	}

	// Use this for initialization
	void Start () {
		holes = new HoleController[5][];
		for (int i = 0; i < holes.Length; i++){
			holes[i] = new HoleController[5-i];
		}
		// get references to all of the holes
		List<HoleController> tempHoles = new List<HoleController> (GameObject.FindObjectsOfType<HoleController>());

		foreach (HoleController hole in tempHoles){
			holes[hole.row][hole.col] = hole;
		}

		DestroyImmediate(pegs[Random.Range(0,15)]);
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
		HoleController leftHole = getLeft(hole);
		HoleController rightHole = getRight(hole);
		HoleController nextRow = getNextRow(hole);
		HoleController prevRow = getPrevRow(hole);
		HoleController nextCol = getNextCol(hole);
		HoleController prevCol = getPrevCol(hole);

		if(leftHole && leftHole.currentPeg)
		{
			HoleController leftLeft = getLeft(leftHole);
			if(leftLeft && !leftLeft.currentPeg)
			{
				return true;
			}
		}
		if(rightHole && rightHole.currentPeg)
		{
			HoleController rightRight = getRight(rightHole);
			if(rightRight && !rightRight.currentPeg)
			{
				return true;
			}
		}
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
	HoleController getLeft(HoleController current){
		if(current.row <= holes.Length && current.col > 0){
			return holes[current.row+1][current.col-1];
		}
		else {
			return null;
		}
	}

	HoleController getRight(HoleController current){
		if(current.row > 0){
			return holes[current.row-1][current.col+1];
		}
		else {
			return null;
		}
	}
	HoleController getPrevRow(HoleController current){
		if(current.row > 0){
			return holes[current.row-1][current.col];
		}
		else {
			return null;
		}
	}
	HoleController getNextRow(HoleController current){
		if(current.row < holes.Length-1 && current.col < holes[current.row+1].Length){
			return holes[current.row+1][current.col];
		}
		else {
			return null;
		}
	}
	HoleController getPrevCol(HoleController current){
		if(current.col >0 ){
			return holes[current.row][current.col - 1];
		}
		else {
			return null;
		}
	}
	HoleController getNextCol(HoleController current){
		if(current.col < holes[current.row].Length-1 ){
			return holes[current.row][current.col+1];
		}
		else {
			return null;
		}
	}
	#endregion
}
