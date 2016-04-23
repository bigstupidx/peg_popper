using UnityEngine;
using System.Collections;

public class PegController : MonoBehaviour {

	public Material openMaterial = null;
	public Material occupiedMaterial = null;
	private Material ogMaterial = null;
	
	public HoleController originalHole = null;
	private HoleController currentHole = null;
	private GameObject validNeighbor = null;

	private Renderer myRenderer = null;

	private GameObject gameManager = null;
	

	public Material originalMaterial{
		get {return ogMaterial;}
	}

	public HoleController hole{
		get {return currentHole;}
		set {hole = value;}
	}

	public bool Valid(){
		return validNeighbor;
	}

	public void DestroyNeighbor(GameObject explosionPrefab){
		if(validNeighbor){
			Vector3 explPosition = validNeighbor.transform.position;
			explPosition += validNeighbor.transform.up*2.5f;
			Object tempExplosion = Instantiate(explosionPrefab, explPosition, Quaternion.identity);
			Destroy(tempExplosion, 0.75f);

			Destroy(validNeighbor);
		}
	}

	void Start(){
		gameManager = GameObject.Find ("Game Manager");
		myRenderer = renderer;
		ogMaterial = myRenderer.material;
	}

	//added for peg game
	void OnTriggerEnter(Collider other){
		currentHole = other.GetComponent<HoleController>();

		// check if we are in a valid position
		if (Application.loadedLevelName == "Main Game")
			validNeighbor = ValidMove();
		else if(Application.loadedLevelName == "Main Game Square" ) 
			validNeighbor = ValidMoveSquare();
		else if ( Application.loadedLevelName == "Main Game European" )
			validNeighbor = ValidMoveEuro();

		if (currentHole.currentPeg || !validNeighbor){
			if (currentHole.currentPeg == gameObject){
				myRenderer.material = ogMaterial;
			} else {
				myRenderer.material = occupiedMaterial;
			}
		} else {
			myRenderer.material = openMaterial;
		}

	}
	
	void OnTriggerExit(Collider other){
		currentHole = null;
		myRenderer.material = ogMaterial;
	}

	GameObject ValidMove(){
		char magnitude1 = (char)(originalHole.row + originalHole.col);
		char magnitude2 = (char)(currentHole.row + currentHole.col);

		BoardManager board = gameManager.GetComponent<BoardManager>();

		if (magnitude1 == magnitude2){
			// the positions are at the same level
			if (originalHole.position - currentHole.position == 2){
				//moved 2 slots away
				HoleController neighbor = board.GetHole(originalHole.row+1,originalHole.col-1);
				return neighbor.currentPeg;
			}
			if (originalHole.position - currentHole.position == -2){
				//moved 2 slots away
				HoleController neighbor = board.GetHole(originalHole.row-1,originalHole.col+1);
				return neighbor.currentPeg;
			}
		}
		else if ( originalHole.row == currentHole.row){
			// diagonal NW<>SE
			if(originalHole.col - currentHole.col == 2){
				// moved 2 slots away
				HoleController neighbor = board.GetHole(originalHole.row,originalHole.col-1);
				return neighbor.currentPeg;
			}
			if(originalHole.col - currentHole.col == -2){
				// moved 2 slots away
				HoleController neighbor = board.GetHole(originalHole.row,originalHole.col+1);
				return neighbor.currentPeg;
			}
		}
		else if ( originalHole.col == currentHole.col){
			// diagonal SW<>NE
			if(originalHole.row - currentHole.row == 2){
				// moved 2 slots away
				HoleController neighbor = board.GetHole(originalHole.row-1,originalHole.col);
				return neighbor.currentPeg;
			}
			if(originalHole.row - currentHole.row == -2){
				// moved 2 slots away
				HoleController neighbor = board.GetHole(originalHole.row+1,originalHole.col);
				return neighbor.currentPeg;
			}
		}
		return null;
	}

	GameObject ValidMoveSquare(){
		BoardManagerSquare board = gameManager.GetComponent<BoardManagerSquare>();

		if ( originalHole.row == currentHole.row){
			// diagonal NW<>SE
			if(originalHole.col - currentHole.col == 2){
				// moved 2 slots away
				HoleController neighbor = board.GetHole(originalHole.row,originalHole.col-1);
				return neighbor.currentPeg;
			}
			if(originalHole.col - currentHole.col == -2){
				// moved 2 slots away
				HoleController neighbor = board.GetHole(originalHole.row,originalHole.col+1);
				return neighbor.currentPeg;
			}
		}
		else if ( originalHole.col == currentHole.col){
			// diagonal SW<>NE
			if(originalHole.row - currentHole.row == 2){
				// moved 2 slots away
				HoleController neighbor = board.GetHole(originalHole.row-1,originalHole.col);
				return neighbor.currentPeg;
			}
			if(originalHole.row - currentHole.row == -2){
				// moved 2 slots away
				HoleController neighbor = board.GetHole(originalHole.row+1,originalHole.col);
				return neighbor.currentPeg;
			}
		}
		return null;
	}

	GameObject ValidMoveEuro(){
		BoardManagerEuropean board = gameManager.GetComponent<BoardManagerEuropean>();
		
		if ( originalHole.row == currentHole.row){
			// diagonal NW<>SE
			if(originalHole.col - currentHole.col == 2){
				// moved 2 slots away
				HoleController neighbor = board.GetHole(originalHole.row,originalHole.col-1);
				return neighbor.currentPeg;
			}
			if(originalHole.col - currentHole.col == -2){
				// moved 2 slots away
				HoleController neighbor = board.GetHole(originalHole.row,originalHole.col+1);
				return neighbor.currentPeg;
			}
		}
		else if ( originalHole.col == currentHole.col){
			// diagonal SW<>NE
			if(originalHole.row - currentHole.row == 2){
				// moved 2 slots away
				HoleController neighbor = board.GetHole(originalHole.row-1,originalHole.col);
				return neighbor.currentPeg;
			}
			if(originalHole.row - currentHole.row == -2){
				// moved 2 slots away
				HoleController neighbor = board.GetHole(originalHole.row+1,originalHole.col);
				return neighbor.currentPeg;
			}
		}
		return null;
	}
}
