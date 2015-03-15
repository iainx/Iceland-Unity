using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

public class Controller : MonoBehaviour {
	public Map map;
	public GameObject skeletonPrefab;
	public GameObject itemPrefab;
	public GameObject player;
	public GameObject TextControllerObject;

	private PersonController playerPersonController;
	private bool uiIsOnScreen = false;
	private TextController textController;

	void Awake() {
		Debug.Log("Started controller");

		textController = (TextController)TextControllerObject.GetComponent<TextController>();

		map = MapLoader.ImportMap("Assets/Resources/Pond.tmx");

		//Rect mapRect = new Rect(0, 0, map.width, map.height * 0.5f);

		GameObject mapObject = GameObject.Find ("Map");
		mapObject.transform.position = new Vector3 (0, map.height / 4, 0);

		MapLayout layout = mapObject.GetComponent<MapLayout>();
		layout.SetMap(map);

		playerPersonController = player.GetComponent<PersonController>();

		List<GameObject> npcs = NPCFactory.Instance.LoadNPCFile();
		foreach(GameObject npcObject in npcs) {
			npcObject.transform.parent = mapObject.transform;

			NPC npc = npcObject.GetComponent<NPC>();
			Vector2 npcPosition = map.ObjectPositionAtMapPoint(npc.position);
			npcObject.transform.localPosition = new Vector3(npcPosition.x, npcPosition.y, 0);
		}

		// Listen for the UI to be on the screen
		GameObject uigo = GameObject.Find ("UIController");
		UIController uic = uigo.GetComponent<UIController>();
		uic.OnScreenHandler += (bool onscreen) => {
			uiIsOnScreen = onscreen;
		};

		Item coat = new Item {name = "Jacket", 
							  description = "It looks like it belongs to Robin Hood",
							  location = new MapPoint {row = 1, column = 5},
							  assetFile = "robin-hood-jacket"};
		GameObject coatObject = (GameObject)Instantiate(itemPrefab);
		coatObject.transform.parent = mapObject.transform;

		MapPoint itemPoint = new MapPoint{row = 1, column = 6};
		Vector2 itemPosition = map.ObjectPositionAtMapPoint(itemPoint);

		coatObject.transform.localPosition = new Vector3(itemPosition.x, itemPosition.y, 0);

		ItemController itemController = coatObject.GetComponent<ItemController>();
		itemController.Item = coat;
	}

	void Update() {
		if (!uiIsOnScreen && Input.GetMouseButtonDown(0)) {
			Vector3 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			Vector2 mousePosition = new Vector2(worldPoint.x, worldPoint.y);
			RaycastHit2D hit = Physics2D.Raycast(mousePosition, -Vector2.up);
			if (hit.collider != null) {
				GameObject hitObject = hit.collider.gameObject;
				// Check if it's something that we can hit
				IClickableObject clickableObject = hitObject.GetComponent(typeof(IClickableObject)) as IClickableObject;

				if (clickableObject == null) {
					Debug.Log ("Cannot click object " + hitObject.name);
				} else {
					clickableObject.HandleClick(worldPoint);
				}
			} else {
				Debug.Log ("Missed");
			}
		}
	}

	public void RunActions(ConversationAction[] actions, GameObject otherParty) {
		foreach (ConversationAction action in actions) {
			if (action.CanExecute()) {
				// Run action.
				switch(action.type) {
				case ConversationAction.ActionType.WalkTo:
					MapPoint mapPoint = new MapPoint {row = Convert.ToInt32(action.objects[0]), 
						column = Convert.ToInt32(action.objects[1])};
					WalkTo(mapPoint, delegate(bool completed) {
						if (!completed) {
							return;
						}
						
						if (action.nextActions != null) {
							RunActions(action.nextActions, otherParty);
						}
					});
					
					break;
					
				case ConversationAction.ActionType.RunConversation:
					RunConversation(action.objects[0], otherParty, delegate(bool completed) {
						if (!completed) {
							return;
						}
						
						if (action.nextActions != null) {
							RunActions(action.nextActions, otherParty);
						}
					});
					break;
					
				case ConversationAction.ActionType.SetFlag:
					PropertyFactory.Instance.SetPropertyValueForKey(action.objects[0], action.objects[1]);
					break;

				case ConversationAction.ActionType.RemoveItem:
					foreach (string o in action.objects) {
						Inventory.Instance.RemoveItem(o);
					}
					break;

				case ConversationAction.ActionType.Say:
					textController.SetText(action.objects[0], player);
					break;

				default:
					break;
				}
			}
		}
	}
	
	private void WalkTo(MapPoint destinationPoint, Action<bool>completionHandler) {
		if (playerPersonController.CurrentPosition == destinationPoint) {
			completionHandler(true);
			return;
		}
		
		List<MovementStep> progression = map.GeneratePath(playerPersonController.CurrentPosition, destinationPoint);
		if (progression == null) {
			Debug.LogError("No path to " + destinationPoint);
			completionHandler(false);
			return;
		}
		
		playerPersonController.MoveAlongPath(progression, completionHandler);
	}
	
	private void RunConversation(string conversationID, GameObject otherCharacter, Action<bool> completionHandler) {
		textController.RunConversationTree(Convert.ToInt32(conversationID), otherCharacter, completionHandler);
	}

	// FIXME: should these move to their own scripts on the player?
	public void MovePlayerTo(MapPoint destinationPoint, Action<bool> completionHandler) {
	}
}
