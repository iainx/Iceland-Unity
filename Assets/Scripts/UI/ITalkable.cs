using UnityEngine;
using System.Collections;

public interface ITalkable {
	Conversation TalkTo ();
	MapPoint TalkMapPoint {get;}
}