using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MapMovement : MonoBehaviour, IClickableObject {

	private GameObject player;
	private Controller controller;
	private PlayerController playerController;
	private PersonController playerPersonController;
	private Vector2 dragStart;
	private bool didDrag = false;

	// Use this for initialization
	void Awake () {
		player = GameObject.Find("Player");
		playerController = player.GetComponent<PlayerController>();
		playerPersonController = player.GetComponent<PersonController>();
		if (playerPersonController == null) {
			Debug.LogError("No person");
		}

		GameObject go = GameObject.Find ("Controller");
		controller = go.GetComponent<Controller>();
		if (controller == null) {
			Debug.LogError("No controller");
		}
	}

	public void HandleClick(Vector3 mousePosition) {
		Vector3 mapPosition = mousePosition - transform.position;
		Vector2 mousePosition2D = new Vector2 (mapPosition.x, mapPosition.y);

		MapPoint destinationPoint = MapLayout.MapPointFromWorldPoint(mousePosition2D);
		List<MovementStep> progression = controller.map.GeneratePath(playerPersonController.CurrentPosition, destinationPoint);
		if (progression == null) {
			Debug.LogError("No path to " + destinationPoint);
			return;
		}

		playerPersonController.MoveAlongPath(progression, null);
	}
}
