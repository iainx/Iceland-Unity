using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NPCController : MonoBehaviour, ITalkable, ILookable {

	MapPoint talkMapPoint = new MapPoint {row = 7, column = 3};
	public MapPoint TalkMapPoint {
		get { return talkMapPoint; }
	}

	public MapPoint LookMapPoint {
		get { return talkMapPoint; }
	}

	// Use this for initialization
	void Start () {
	
	}

	public Conversation conversation;
	public Conversation TalkTo() {
		return conversation;
	}

	public string LookAt() {
		return "It looks like a living skeleton... with no clothes on.";
	}
}
