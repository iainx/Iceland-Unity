using UnityEngine;
using System.Collections;

public class ObjectClickHandler : MonoBehaviour, IClickableObject {
	private UIController uiController;

	void Awake() {
		GameObject go = GameObject.Find ("UIController");
		uiController = go.GetComponent<UIController>();
	}

	public void HandleClick(Vector3 clickPosition) {
		Debug.Log("Handling click in NPCClickHandler");
		uiController.ShowNPCMenu(gameObject);
	} 
}
