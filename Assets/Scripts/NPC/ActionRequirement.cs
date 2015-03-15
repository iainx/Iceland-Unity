using UnityEngine;
using System.Collections;
using SimpleJSON;

public class ActionRequirement {
	public string flag;
	public string value;
	public enum RequirementType {
		Equals,
		NotEquals,
		GreaterThan,
		LessThan
	};

	public RequirementType type;

	public ActionRequirement(JSONNode requirementJSON) {
		flag = requirementJSON["flag"];
		value = requirementJSON["value"];
		switch(requirementJSON["type"]) {
		case "equals":
			type = RequirementType.Equals;
			break;

		case "not-equals":
			type = RequirementType.NotEquals;
			break;

		case "greater-than":
			type = RequirementType.GreaterThan;
			break;

		case "less-than":
			type = RequirementType.LessThan;
			break;

		default:
			type = RequirementType.Equals;
			break;
		}
	}

	public bool IsValid() {
		string propVal = PropertyFactory.Instance.PropertyValueForKey(flag);
		if (propVal == null) {
			propVal = "false";
		}

		Debug.Log ("Checking requirement " + flag + " " + type + " " + value + " -> " + propVal);
		switch (type) {
		case RequirementType.Equals:
			return (propVal == value);

		case RequirementType.NotEquals:
			return (propVal != value);

		case RequirementType.GreaterThan:
			return true;

		case RequirementType.LessThan:
			return true;
		}

		return false;
	}
}
