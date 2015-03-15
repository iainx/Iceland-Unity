using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class PersonController : MonoBehaviour {	
	private Animator animator;
	private Controller controller;

	private MapPoint currentPosition;
	public MapPoint CurrentPosition {
		get { return currentPosition; }
		set { 
			currentPosition = value; 
		}
	}
	
	public float walkingSpeed = 0.5f;
	
	private List<MovementStep> currentPath;
	private Action<bool> currentCompletionHandler = null;
	
	private Map.Direction currentDirection;

	public bool walking = false;

	void Awake() {
		animator = GetComponent<Animator>();
		if (animator == null) {
			Debug.LogError("No animator");
		}
		
		GameObject go = GameObject.Find ("Controller");
		controller = go.GetComponent<Controller>();
		if (controller == null) {
			Debug.LogError("No controller");
		}
	}

	// Use this for initialization
	public void MoveAlongPath(List<MovementStep> path, Action<bool> completionHandler) {
		if (currentCompletionHandler != null) {
			currentCompletionHandler(false);
		}
		StopCoroutine("WalkAlongPath");

		currentPath = new List<MovementStep>(path);
		currentCompletionHandler = completionHandler;
		walking = true;
	
		StartCoroutine("WalkAlongPath");
	}

	private struct WalkingStep {
		public MapPoint fromPoint;
		public MovementStep step;
		public Map.Direction direction;
	};

	IEnumerator WalkAlongPath() {
		// currentPath could be replaced underneath us if the user
		// changes destination mid-walk.

		StopCoroutine("WalkToStep");
		while (currentPath.Count > 0) {
			MovementStep step = currentPath[0];
			currentPath.RemoveAt(0);
			
			animator.SetBool("Walking", true);

			Map.Direction newDirection = Map.CalculateDirectionTravelling(currentPosition, step.MapPoint);
			animator.SetInteger("Direction", (int)newDirection);
			
			MapPoint fromPoint = currentPosition;
			currentPosition = step.MapPoint;
	
			WalkingStep ws = new WalkingStep {
				fromPoint = fromPoint,
				step = step,
				direction = newDirection
			};

			yield return StartCoroutine("WalkToStep", ws);
		}
		
		animator.SetBool("Walking", false);
		walking = false;

		if (currentCompletionHandler != null) {
			currentCompletionHandler(true);
		}
	}
	
	IEnumerator WalkToStep(WalkingStep ws) {
		var startPosition = transform.localPosition;
		Vector2 destinationWorldPosition = MapLayout.ScreenPointFromMapPoint(ws.step.MapPoint);
		Vector2 fromWorldPosition = MapLayout.ScreenPointFromMapPoint(ws.fromPoint);
		
		Tile fromTile, toTile;
		fromTile = controller.map.TileAtMapPoint(ws.fromPoint);
		toTile = controller.map.TileAtMapPoint(currentPosition);
		
		float timer = 0.0f;
		
		Vector2 edge = fromTile.edgeForDirection(ws.direction);
		float divisor = 1.0f;
		
		if (edge.x != -1 && edge.y != -1) {
			var edgeDestination = new Vector3(fromWorldPosition.x + ((edge.x - 50.0f) / 100.0f), fromWorldPosition.y + (edge.y / 100.0f), 0);
			while (transform.localPosition != edgeDestination) {
				transform.localPosition = Vector3.MoveTowards (startPosition, edgeDestination, walkingSpeed * timer);
				timer += Time.fixedDeltaTime;

				yield return new WaitForEndOfFrame();
			}
			
			divisor = 2.0f;
			startPosition = edgeDestination;
		}
		
		var destination = new Vector3(destinationWorldPosition.x, destinationWorldPosition.y + (toTile.centreY / 100.0f));
		
		timer = 0.0f;
		
		while (transform.localPosition != destination) {
			transform.localPosition = Vector3.MoveTowards (startPosition, destination, walkingSpeed * timer);
			timer += Time.fixedDeltaTime;

			yield return new WaitForEndOfFrame();
		}
	}
}
