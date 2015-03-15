using UnityEngine;
using System.Collections;
using SimpleJSON;

public class ConversationDependancy {
	public enum DependancyType {
		PlayerHasItem,
		PlayerDoesNotHaveItem
	};

	public DependancyType type;
	public string[] objects;
	public ConversationAction[] actions;

	public ConversationDependancy(JSONNode dependancyJSON) {
		var jsonType = dependancyJSON["type"];
		switch(jsonType) {
		case "player-has":
			type = DependancyType.PlayerHasItem;
			break;

		case "player-does-not-have":
			type = DependancyType.PlayerDoesNotHaveItem;
			break;

		default:
			Debug.LogError("Unknown dependancy type");
			return;
		}

		var actionsJSON = dependancyJSON["actions"];
		if (actionsJSON == null) {
			actions = null;
		} else {
			actions = new ConversationAction[actionsJSON.Count];
			for (int i = 0; i < actionsJSON.Count; i++) {
				actions[i] = new ConversationAction(actionsJSON[i]);
			}
		}

		objects = ConversationLoader.JSONToStringArray(dependancyJSON, "objects");
	}
	
	public bool Check() {
		foreach (string itemName in objects) {
			Item item = Inventory.Instance.ItemForName(itemName);
			
			if (type == ConversationDependancy.DependancyType.PlayerHasItem) {
				return item != null;
			} else {
				return item == null;
			}
		}
		return false;
	}
}
