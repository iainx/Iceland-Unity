using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour {
	private Controller controller;
	private PersonController playerPersonController;

	void Awake () {
		GameObject gameObj = GameObject.Find ("Controller");
		controller = gameObj.GetComponent<Controller>();

		playerPersonController = gameObject.GetComponent<PersonController>();
	}

}
