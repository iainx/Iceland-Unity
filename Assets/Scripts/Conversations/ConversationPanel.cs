using UnityEngine;
using UnityEngine.UI;

using System;
using System.Collections;

public class ConversationPanel : MonoBehaviour {
	public GameObject[] dialogSlots;
	public GameObject upButton;
	public GameObject downButton;

	private Conversation currentConversation;
	private ConversationItem[] responses;
	
	private TextController controller;

	// Use this for initialization
	void Start () {
	
	}

	public void SetResponses(TextController textController, ConversationItem item) {
		GameObject playerObject = GameObject.Find ("Player");

		controller = textController;

		responses = new ConversationItem[item.responseIDs.Length];

		for (int i = 0; i < item.responseIDs.Length; i++) {
			responses[i] = ConversationFactory.Instance.ItemForID(item.responseIDs[i]);
		}

		// Process dependancies
		if (item.dependancies != null) {
			foreach (ConversationDependancy dependancy in item.dependancies) {
				if (dependancy.Check()) {
					foreach (ConversationAction action in dependancy.actions) {
						if (action.type == ConversationAction.ActionType.ReplaceResponses) {
							responses = new ConversationItem[action.objects.Length];

							for (int i = 0; i < action.objects.Length; i++) {
								int id = Convert.ToInt32(action.objects[i]);
								responses[i] = ConversationFactory.Instance.ItemForID(id);
							}
						}
					}
					break;
				}
			}
		}

		for (int i = 0; i < 3 && i < responses.Length; i++) {
			GameObject slot = dialogSlots[i];
			Text text = slot.GetComponentInChildren<Text>() as Text;
			text.text = responses[i].playerDialog[0];
		}
	}

	public void DialogSlot1Clicked() {
		ConversationItem item = responses[0];
		controller.ContinueConversation(item);
	}

	public void DialogSlot2Clicked() {
		ConversationItem item = responses[1];
		controller.ContinueConversation(item);
	}

	public void DialogSlot3Clicked() {
		ConversationItem item = responses[2];
		controller.ContinueConversation(item);
	}
}
