using UnityEngine;
using System.Collections;

public class ConversationFactory : MonoBehaviour {
	public static ConversationFactory Instance {get; private set;}

	private Hashtable IDToConversation;
	public string[] conversations;

	void Awake() {
		Instance = this;

		IDToConversation = new Hashtable();

		foreach (string conversation in conversations) {
			ConversationLoader.LoadConversation(conversation);
		}
	}

	public void AddItemForID(int id, ConversationItem item) {
		IDToConversation[id] = item;
	}

	public ConversationItem ItemForID(int id) {
		return (ConversationItem)IDToConversation[id];
	}
}
