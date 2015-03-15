using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using SimpleJSON;

public class ConversationLoader {
	public static Conversation LoadConversation(string filename) {
		TextAsset asset = Resources.Load ("Conversations/" + filename) as TextAsset;

		var conversationJSON = JSON.Parse(asset.text);

		Conversation items = new Conversation();

		for (int i = 0; i < conversationJSON.Count; i++) {
			var conversation = conversationJSON[i];

			ConversationItem item = new ConversationItem();
			item.id = conversation["id"].AsInt;

			item.playerDialog = JSONToStringArray(conversation, "player");
			item.characterDialog = JSONToStringArray(conversation, "character");

			var responseIDs = conversation["responseIDs"];
			if (responseIDs == null) {
				item.responseIDs = null;
			} else {
				item.responseIDs = new int[responseIDs.Count];
				for (int j = 0; j < responseIDs.Count; j++) {
					item.responseIDs[j] = responseIDs[j].AsInt;
				}
			}

			var actions = conversation["actions"];
			if (actions == null) {
				item.actions = null;
			} else {
				item.actions = new ConversationAction[actions.Count];
				for (int j = 0; j < actions.Count; j++) {
					item.actions[j] = new ConversationAction(actions[j]);
				}
			}

			var dependancies = conversation["dependencies"];
			if (dependancies == null) {
				item.dependancies = null;
			} else {
				item.dependancies = new ConversationDependancy[dependancies.Count];
				for (int j = 0; j < dependancies.Count; j++) {
					item.dependancies[j] = new ConversationDependancy(dependancies[j]);
				}
			}

			ConversationFactory.Instance.AddItemForID(item.id, item);
		}

		return items;
	}
	
	public static string[] JSONToStringArray(JSONNode node, string key) {
		var jsonLines = node[key];

		if (jsonLines == null) {
			return null;
		}

		string[] lines = new string[jsonLines.Count];
		for (int i = 0; i < jsonLines.Count; i++) {
			lines[i] = jsonLines[i];
		}

		return lines;
	}
}
