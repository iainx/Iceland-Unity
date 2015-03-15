using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using SimpleJSON;

public class NPCFactory : MonoBehaviour {
	public static NPCFactory Instance {
		get; private set;
	}
	
	public string NPCFile;
	void Awake () {
		Instance = this;
	}
	
	private Hashtable stringToNPC = new Hashtable();
	public List<GameObject> NPCList = new List<GameObject>();

	public List<GameObject> LoadNPCFile() {
		Debug.Log("Loading " + NPCFile);

		TextAsset asset = Resources.Load ("Characters/" + NPCFile) as TextAsset;
		
		var NPCJSON = JSON.Parse(asset.text);
		Object prefab = Resources.Load ("Prefabs/Characters/Skeleton");
		if (prefab == null) {
			Debug.LogError ("Prefab is null");
		}

		GameObject npcObj = (GameObject)Instantiate(prefab);
		if (npcObj == null) {
			Debug.LogError ("NPC is null");
		}

		NPC npc = npcObj.GetComponent<NPC>();
		if (npc == null) {
			Debug.LogError ("NPC component is null");
		}

		npc.CreateFromJSON(NPCJSON);
		Debug.Log (npc.name + " - " + npc.initialDirection);

		stringToNPC[npc.id] = npcObj;
		NPCList.Add(npcObj);

		return NPCList;
	}

	public GameObject NPCForName(string name) {
		return null;
	}
}
