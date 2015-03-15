using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;

public class NPC : MonoBehaviour {
	public string name;
	public string id;
	public string description;
	public MapPoint position;
	public Map.Direction initialDirection;

	public Hashtable triggers = new Hashtable ();

	void Awake () {
	}

	public void CreateFromJSON(JSONNode node) {
		name = node["name"];
		description = node["description"];
		id = node["id"];
		position = NPC.MapPointFromJSON(node["position"]);
		initialDirection = NPC.DirectionFromString(node["direction"]);

		var triggersJSON = node["triggers"];
		if (triggersJSON != null) {
			for (int i = 0; i < triggersJSON.Count; i++) {
				ActionTrigger trigger = new ActionTrigger(triggersJSON[i]);
				triggers[trigger.type] = trigger;
			}
		}
	}

	public static MapPoint MapPointFromJSON(JSONNode node) {
		int row, column;

		row = node["row"].AsInt;
		column = node["column"].AsInt;

		return new MapPoint {row = row, column = column};
	}

	public static Map.Direction DirectionFromString(string directionString) {
		switch (directionString) {
		case "north":
			return Map.Direction.North;

		case "south":
			return Map.Direction.South;

		case "east":
			return Map.Direction.East;

		case "west":
			return Map.Direction.West;

		default:
			return Map.Direction.North;
		}
	}

	public ConversationAction[] actionsForTriggerType(ActionTrigger.TriggerType type) {
		ActionTrigger trigger = (ActionTrigger)triggers[type];
		if (trigger == null) {
			return null;
		}

		return trigger.actions;
	}
}
