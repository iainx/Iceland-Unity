using UnityEngine;
using UnityEngine.UI;

using System;
using System.Collections;
using System.Collections.Generic;

public class TextController : MonoBehaviour {
	public GameObject ControllerObject;
	public GameObject OnScreenText;
	public GameObject conversationPanelPrefab;
	private GameObject playerObject;
	private Text textComponent;

	//private Conversation currentConversation;
	private int currentID = 0;
	private GameObject currentPeer;
	private GameObject conversationPanel;
	private Action<bool> currentCompletionHandler;
	private Controller controller;

	// Use this for initialization
	void Awake () {
		textComponent = OnScreenText.GetComponent<Text>();
		playerObject = GameObject.Find("Player");

		controller = ControllerObject.GetComponent<Controller>();
	}

	private void PositionText(GameObject forObject) {
		OnScreenText.transform.position = new Vector3(forObject.transform.position.x, 
		                                              forObject.transform.position.y + 0.64f,
		                                              forObject.transform.position.z);
	}

	public void RunConversationTree(int id, GameObject peer, Action<bool> completionHandler) {
		//currentConversation = conversation;
		currentID = id;
		currentPeer = peer;
		currentCompletionHandler = completionHandler;

		StartCoroutine("RunConversation", id);
	}

	public void ContinueConversation(ConversationItem item) {
		currentID = item.id;

		if (conversationPanel != null) {
			Destroy(conversationPanel);
		}
		StartCoroutine("RunConversation", item.id);
	}

	IEnumerator RunConversation(int id) {
		OnScreenText.SetActive(true);

		while (true) {
			ConversationItem item = ConversationFactory.Instance.ItemForID(currentID);
			yield return StartCoroutine("RunConversationItem", item);

			if (item.responseIDs == null) {
				// Conversation is over
				currentCompletionHandler(true);
				break;
			}

			if (item.responseIDs.Length == 1) {
				currentID = item.responseIDs[0];
				continue;
			}

			// Show the conversation responses and wait until we start the next conversation
			conversationPanel = (GameObject)Instantiate(conversationPanelPrefab);
			conversationPanel.transform.SetParent(transform);

			ConversationPanel panel = conversationPanel.GetComponent<ConversationPanel>();
			panel.SetResponses(this, item);

			break;
		}

		OnScreenText.SetActive(false);
	}

	IEnumerator RunConversationItem(ConversationItem item) {
		// Display the player's lines
		int index = 0;
		if (item.playerDialog != null) {
			while (index < item.playerDialog.Length) {
				textComponent.text = item.playerDialog[index];
				PositionText(playerObject);
				index++;

				yield return new WaitForSeconds(2);
			}
		}

		if (item.characterDialog == null) {
			// No character dialog is finished
			return true;
		}

		index = 0;
		while (index < item.characterDialog.Length) {
			textComponent.text = item.characterDialog[index];
			PositionText(currentPeer);

			index++;

			yield return new WaitForSeconds(2);
		}

		if (item.actions != null) {
			controller.RunActions(item.actions, currentPeer);
		}
		/*
		// Process any actions this item may have
		if (item.actions != null) {
			foreach (ConversationAction action in item.actions) {
				if (action.type == ConversationAction.ActionType.RemoveItem) {
					foreach (string o in action.objects) {
						inventory.RemoveItem(o);
					}
				}
			}
		}
		*/
	}

	public void SetText(string text, GameObject forObject){
		textComponent.text = text;
		
		PositionText(forObject);

		OnScreenText.SetActive(true);

		Invoke ("ClearText", 5);
	}
	
	void ClearText() {
		OnScreenText.SetActive(false);
		textComponent.text = "";
	}
}
