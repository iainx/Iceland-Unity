using UnityEngine;
using System.Collections;
using SimpleJSON;

public class ActionTrigger {
	public enum TriggerType {
		Look,
		Talk,
		Collect,
		Give,
		Push,
		Pull,
		Use
	};

	public TriggerType type;
	public ConversationAction[] actions;

	public ActionTrigger(JSONNode triggerJSON) {
		switch (triggerJSON["type"]) {
		case "talk":
			type = TriggerType.Talk;
			break;

		case "look":
			type = TriggerType.Look;
			break;

		default:
			type = TriggerType.Collect;
			break;
		}

		var actionsJSON = triggerJSON["actions"];
		if (actionsJSON == null) {
			actions = null;
		} else {
			actions = new ConversationAction[actionsJSON.Count];
			for (int i = 0; i < actionsJSON.Count; i++) {
				ConversationAction action = new ConversationAction(actionsJSON[i]);
				actions[i] = action;
			}
		}
	}
}
