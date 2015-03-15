using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIController : MonoBehaviour {
	public GameObject NPCMenu;
	public GameObject OnScreenText;
	public GameObject ControllerObject;
	public GameObject TextController;

	private bool showGUI = false;
	private GameObject currentTarget;
	private Controller controller;
	private TextController textController;

	public delegate void UIOnScreenHandler(bool onscreen);
	public event UIOnScreenHandler OnScreenHandler;

	private GameObject playerObject = null;
	private PlayerController playerController;

	void Awake () {
		NPCMenu.SetActive(showGUI);

		controller = ControllerObject.GetComponent<Controller>();

		textController = TextController.GetComponent<TextController>();
	}

	public void ShowNPCMenu(GameObject npc) {
		showGUI = true;
		currentTarget = npc;

		//NPCMenu.transform.position = currentTarget.transform.position;
		RectTransform rect = NPCMenu.GetComponent<RectTransform>();

		NPCMenu.transform.position = new Vector3(currentTarget.transform.position.x, 
		                                         currentTarget.transform.position.y + 0.64f,
		                                         currentTarget.transform.position.z);
		                                         
		NPCMenu.SetActive(showGUI);

		OnScreenHandler(showGUI);
	}

	public void HideNPCMenu() {
		showGUI = false;
		currentTarget = null;

		NPCMenu.SetActive(showGUI);

		OnScreenHandler(showGUI);
	}

	public void HideAllUI() {
		HideNPCMenu();
	}

	private void CheckPlayerSetup() {
		if (playerObject != null) {
			return;
		}

		playerObject = (GameObject)GameObject.Find("Player");
		playerController = playerObject.GetComponent<PlayerController>();
	}

	void ExecuteActionsForTrigger(GameObject localTarget, ActionTrigger.TriggerType type)
	{
		CheckPlayerSetup ();
		NPC npcController = localTarget.GetComponent<NPC> ();
		ConversationAction[] actions = npcController.actionsForTriggerType (type);
		if (actions == null) {
			// Can't do this action
			return;
		}
		controller.RunActions (actions, localTarget);
	}

	public void Talk () {
		GameObject localTarget = currentTarget;
		HideNPCMenu();

		ExecuteActionsForTrigger (localTarget, ActionTrigger.TriggerType.Talk);
	}

	public void Look () {
		GameObject localTarget = currentTarget;
		HideNPCMenu();

		ExecuteActionsForTrigger(localTarget, ActionTrigger.TriggerType.Look);

		/*
		ILookable lookable = localTarget.GetComponent(typeof(ILookable)) as ILookable;
		if (lookable == null) {
			return;
		}

		controller.MovePlayerTo(lookable.LookMapPoint, delegate(bool completed) {
			if (completed == false) {
				return;
			}
			SetText(lookable.LookAt(), localTarget);
	    });
	    */
	}

	public void PickUp() {
		GameObject localTarget = currentTarget;
		HideNPCMenu();

		//ExecuteActionsForTrigger(localTarget, ActionTrigger.TriggerType.Collect);

		ICollectable collectable = localTarget.GetComponent(typeof(ICollectable)) as ICollectable;
		if (collectable == null) { 
			return;
		}

		ItemController itc = localTarget.GetComponent<ItemController>();
		if (itc == null) {
			Debug.LogError("Item without controller");
			return;
		}
		/*
		controller.MovePlayerTo(collectable.CollectMapPoint, delegate (bool completed){
			if (completed == false) {
				return;
			}
*/
			Item item = itc.Item;
			//controller.AddItemToPlayerInventory(item);
			Inventory.Instance.AddItem(item);

			// Remove item from the map
			Destroy(localTarget);
		//});
	}

	public void SetText(string text, GameObject forObject) {
		textController.SetText(text, forObject);
	}
}
