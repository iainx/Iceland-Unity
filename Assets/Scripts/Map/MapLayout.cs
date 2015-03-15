using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class MapLayout : MonoBehaviour {

	private Map map;
	const float halfTileHeight = 0.25f;
	const float halfTileWidth = 0.5f;

	// Use this for initialization
	void Start () {
	
	}

	public static Vector2 ScreenPointFromMapPoint(MapPoint mapPoint) {
		float row = mapPoint.row; 
		float column = mapPoint.column;
		
		float y = (column + row) * halfTileHeight;;
		float x = (column - row) * halfTileWidth;

		return new Vector2 (x, -y);
	}

	public static MapPoint MapPointFromWorldPoint(Vector2 worldPoint){
		float row, column;

		worldPoint.y -= 0.65f;
		worldPoint.y = Mathf.Abs(worldPoint.y);

		row = ((worldPoint.y / halfTileHeight) - (worldPoint.x / halfTileWidth)) / 2.0f;
		column = ((worldPoint.x / halfTileWidth) + (worldPoint.y / halfTileHeight)) / 2.0f;

		return new MapPoint((int)row, (int)column);
	}

	public void SetMap(Map map) {
		int row = 0;
		int column = 0;
		int i = 0;

		this.map = map;

		BoxCollider2D collider = GetComponent<BoxCollider2D>();
		if (collider == null) {
			Debug.LogError("No collider");
			return;
		}

		float mapWidth = (map.width + map.height) * halfTileWidth;
		float mapHeight = ((map.width + map.height) * halfTileHeight) + 0.15f;

		collider.size = new Vector2(mapWidth, mapHeight);
		collider.offset = new Vector2(0, -((mapHeight / 2.0f) - 0.65f));

		foreach (UInt32 tileID in map.tiles) {
			if (tileID != 0) {
				Tile tile = map.GIDToSprite[tileID];
				GameObject tileObject = CreateTile(tile, i);
				
				tileObject.transform.parent = transform;
				tileObject.transform.localPosition = ScreenPointFromMapPoint(new MapPoint(row, column));
			}

			i++;
			column++;
			if (column >= map.width) {
				column = 0;
				row++;
			}
		}
	}

	GameObject CreateTile(Tile tile, int idx) {
		GameObject tileObject = new GameObject("Tile");
		SpriteRenderer renderer = tileObject.AddComponent<SpriteRenderer>();
		renderer.sprite = tile.sprite;
		renderer.sortingOrder = idx;

		return tileObject;
	}
}
