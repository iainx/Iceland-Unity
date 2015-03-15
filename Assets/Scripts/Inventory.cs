using UnityEngine;
using System.Collections;

public class Inventory : MonoBehaviour {
	public static Inventory Instance {get; private set;}

	private Hashtable inventory;

	void Awake() {
		Instance = this;

		inventory = new Hashtable();
	}

	public void AddItem(Item item) {
		inventory[item.name] = item;
	}

	public Item ItemForName(string name) {
		return (Item)inventory[name];
	}

	public void RemoveItem(string name) {
		inventory.Remove(name);
	}
}
