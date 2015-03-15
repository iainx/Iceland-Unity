using UnityEngine;
using System.Collections;

public class ConversationItem {
	public int id;
	public string[] playerDialog;
	public string[] characterDialog;
	public int[] responseIDs;

	public ConversationDependancy[] dependancies;
	public ConversationAction[] actions;
}
