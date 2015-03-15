using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NPCMovementController : MonoBehaviour {
	private PersonController personController;

	public List<List<MovementStep>> paths;
	int pathIndex = 0;

	void Awake () {
		personController = gameObject.GetComponent<PersonController>();

		paths = new List<List<MovementStep>>();
	}

	void Update () {
		if (paths.Count > 0 && personController.walking == false) {
			personController.MoveAlongPath(paths[pathIndex], null);
			pathIndex++;

			if (pathIndex >= paths.Count) {
				pathIndex = 0;
			}
		}
	}
}
