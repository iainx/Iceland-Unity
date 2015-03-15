using UnityEngine;
using System.Collections;

public class PropertyFactory : MonoBehaviour {
	public static PropertyFactory Instance {get; private set;}
	private Hashtable properties;

	void Awake() {
		Instance = this;
		properties = new Hashtable ();
	}

	public string PropertyValueForKey(string key) {
		return (string)properties[key];
	}

	public void SetPropertyValueForKey(string key, string value) {
		properties[key] = value;
	}
}
