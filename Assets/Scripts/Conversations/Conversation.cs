using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Conversation {
	private Dictionary<int, ConversationItem>items;

	public Conversation() {
		items = new Dictionary<int, ConversationItem>();
	}

	public void AddItem(int id, ConversationItem item) {
		items[id] = item;
	}

	public ConversationItem ItemForID(int id) {
		return items[id];
	}
}
