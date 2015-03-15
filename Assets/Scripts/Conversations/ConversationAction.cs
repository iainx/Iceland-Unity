using UnityEngine;
using System.Collections;
using SimpleJSON;

public class ConversationAction {
	public string[] objects;
	public enum ActionType {
		AddItem,
		RemoveItem,
		ReplaceResponses,
		AddResponse,
		RemoveResponse,
		WalkTo,
		RunConversation,
		SetFlag,
		Say,
	};

	public ActionType type;
	public ActionRequirement[] requirements;

	public ConversationAction[] nextActions;

	public ConversationAction(JSONNode actionJSON) {
		string JSONtype = actionJSON["action"];
		switch (JSONtype) {
		case "remove-item":
			type = ActionType.RemoveItem;
			break;

		case "add-item":
			type = ActionType.AddItem;
			break;

		case "replace-responses":
			type = ActionType.ReplaceResponses;
			break;

		case "walk-to":
			type = ActionType.WalkTo;
			break;

		case "run-conversation":
			type = ActionType.RunConversation;
			break;

		case "set-flag":
			type = ActionType.SetFlag;
			break;

		case "say":
			type = ActionType.Say;
			break;

		default:
			type = ActionType.AddItem;
			break;
		}

		objects = ConversationLoader.JSONToStringArray(actionJSON, "objects");

		var requirementsJSON = actionJSON["requirements"];
		if (requirementsJSON == null) {
			requirements = null;
		} else {
			requirements = new ActionRequirement[requirementsJSON.Count];
			for (int i = 0; i < requirementsJSON.Count; i++) {
				requirements[i] = new ActionRequirement(requirementsJSON[i]);
			}
		}

		var actionsJSON = actionJSON["actions"];
		if (actionsJSON == null) {
			nextActions = null;
		} else {
			nextActions = new ConversationAction[actionsJSON.Count];
			for (int i = 0; i < actionsJSON.Count; i++) {
				nextActions[i] = new ConversationAction(actionsJSON[i]);
			}
		}
	}

	public bool CanExecute() {
		if (requirements == null) {
			return true;
		}

		foreach (ActionRequirement requirement in requirements) {
			if (requirement.IsValid() == false) {
				return false;
			}
		}
		return true;
	}
}
