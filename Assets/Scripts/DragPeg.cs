using UnityEngine;
using System.Collections;

public class DragPeg : MonoBehaviour {

	public float speed = 10;
	public bool xAxis = true;
	public bool zAxis = true;

	public AudioClip pickUp = null;
	public AudioClip notValid = null;
	public AudioClip destroy = null;

	public GameObject explosionPrefab = null;

	private Plane objPlane;
	private Transform objTr;
	private Camera cam;
	private RaycastHit hit;

	private Vector3 originalPosition;
	private bool lifted = false;
	private PegController heldPeg = null;

	private BoardManager boardManager = null;
	private BoardManagerSquare boardManagerSquare = null;
	
	void Start (){
		boardManager = (BoardManager)FindObjectOfType(typeof(BoardManager));
		if (boardManager == null){
			boardManagerSquare = (BoardManagerSquare)FindObjectOfType(typeof(BoardManagerSquare));
		}

		if (camera)
			cam = camera;
		else
			cam = Camera.main;
	}
	
	void Update (){
		if (boardManager!=null)
			if (boardManager.state != GameState.Playing) return;

		if (boardManagerSquare!=null)
			if (boardManagerSquare.state != GameState.Playing) return;


		if (Input.GetMouseButtonDown (0)){ // if left button pressed...
			// and some object is under the mouse pointer...
			if (Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition),  out hit, 500)){
				// and it has the right tag...
				if (hit.transform.tag == "peg"){
					heldPeg = hit.transform.GetComponent<PegController>();
					// raise the peg above the board if not already done
					if (!lifted){
						objTr = hit.transform;
						Vector3 newPos = originalPosition = objTr.position;
						newPos += objTr.up*3.0f;
						objTr.position = newPos;

						AudioSource.PlayClipAtPoint(pickUp, objTr.position);
						lifted = true;
					}
					// starts dragging it
					StartCoroutine ("DragObject");
				}
			}
		}
	}
	
	IEnumerator DragObject(){
		Vector3 newPos;

		// create a plane at object position
		objPlane = new Plane(objTr.up, objTr.position); 
		while (Input.GetMouseButton (0)){ // loop while button pressed
			// create a ray from mouse position perpendicular to screen
			Ray ray = cam.ScreenPointToRay (Input.mousePosition);
			float distance;
			// get the distance from mouse pointer to the plane
			objPlane.Raycast(ray, out distance);
			// define the new position 
			newPos = ray.GetPoint(distance);
			// avoid modification in the disabled axis
			if (!xAxis) newPos.x = objTr.position.x;
			if (!zAxis) newPos.z = objTr.position.z;
			// move to newPos a little step each frame
			objTr.position = Vector3.Lerp(objTr.position, newPos, speed*Time.deltaTime);
			// suspend execution until next frame
			yield return null;
		}
		if(lifted == true){
			newPos = objTr.position;
			newPos.y -= 3.0f;
			objTr.position = newPos;
			lifted = false;
		}
		//added for peg game
		//if it is not colliding witha hole or if the hole it is colliding with has a peg already
		if (heldPeg.hole == null || heldPeg.hole.currentPeg != null || !heldPeg.Valid()){	
			AudioSource.PlayClipAtPoint(notValid, objTr.position);
			objTr.position = originalPosition;
		}
		else{
			// set the original hole to empty and set the reference on the new hole to this peg
			heldPeg.originalHole.currentPeg = null;
			heldPeg.originalHole = heldPeg.hole;
			heldPeg.hole.currentPeg = heldPeg.gameObject;

			newPos = heldPeg.hole.transform.position;
			newPos.x += 0.28f;
			objTr.position = newPos;

			AudioSource.PlayClipAtPoint(destroy,cam.transform.position);
			heldPeg.DestroyNeighbor(explosionPrefab);

		}
		//set pegs mateiral back tonormal.
		heldPeg.renderer.material = heldPeg.originalMaterial;
	}	
}
