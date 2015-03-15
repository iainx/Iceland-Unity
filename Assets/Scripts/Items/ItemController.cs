using UnityEngine;
using System.IO;
using System.Collections;

public class ItemController : MonoBehaviour, ILookable, ICollectable {
	private SpriteRenderer spriteRenderer;
	private MapPoint location = new MapPoint {row = 0, column = 6};

	public MapPoint LookMapPoint {
		get {
			return location;
		}
	}

	public MapPoint CollectMapPoint {
		get {
			return location;
		}
	}

	private Item item;
	public Item Item {
		get {
			return item;
		} 
		set {
			item = value;

			Vector2 worldPos = MapLayout.ScreenPointFromMapPoint(item.location);
			//this.transform.position = new Vector3 (worldPos.x, worldPos.y, 0);

			string resourceFile = "images/Items/" + Path.GetFileNameWithoutExtension(item.assetFile);
			Texture2D texture = Resources.Load<Texture2D>(resourceFile);

			Debug.Log ("Resource file: " + resourceFile);
			
			Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0), 100);
			spriteRenderer.sprite = sprite;
		}
	}

	void Awake () {
		spriteRenderer = GetComponent<SpriteRenderer>();
	}

	public string LookAt() {
		return item.description;
	}

	public void PickUp() {

	}
}
