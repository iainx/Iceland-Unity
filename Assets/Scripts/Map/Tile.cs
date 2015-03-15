using UnityEngine;
using System;
using System.Collections;
using System.IO;
using System.Xml.Linq;

public class Tile {
	public float offsetX = 0;
	public float offsetY = 0;

	public float centreX = 50.0f;
	public float centreY = 35.0f;
	public float northX = -1.0f;
	public float northY = -1.0f;
	public float eastX = -1.0f;
	public float eastY = -1.0f;
	public float southX = -1.0f;
	public float southY = -1.0f;
	public float westX = -1.0f;
	public float westY = -1.0f;

	public string spriteName;
	public Texture2D texture;
	public Sprite sprite;

	[Flags]
	public enum ValidDirection {
		None = 0,
		North = 1,
		South = 2,
		East = 4,
		West = 8
	}
	
	public ValidDirection validDirections;

	public Tile (string sourceFilePath, XElement properties) {
		string resourceFile = Path.GetDirectoryName(sourceFilePath) + "/" + Path.GetFileNameWithoutExtension(sourceFilePath);
		texture = Resources.Load<Texture2D>(resourceFile);

		sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0), 100);

		//validDirections = parseValidDirections(allowedDirections);
		if (properties != null) {
			parseProperties(properties);
		}
	}

	private void parseProperties(XElement properties) {
		foreach (XElement property in properties.Elements("property")) {
			if (property.Attribute("name") == null) {
				continue;
			}
			string name = property.Attribute("name").Value;
			string value = property.Attribute("value").Value;
			switch (name) {
			case "exits":
				validDirections = parseValidDirections(value);
				break;

			case "centre-x":
				centreX = Convert.ToSingle(value);
				break;

			case "centre-y":
				centreY = Convert.ToSingle(value);
				break;

			case "east-x":
				eastX = Convert.ToSingle(value);
				break;

			case "east-y":
				eastY = Convert.ToSingle(value);
				break;

			case "west-x":
				westX = Convert.ToSingle(value);
				break;

			case "west-y":
				westY = Convert.ToSingle(value);
				break;

			case "north-x":
				northX = Convert.ToSingle(value);
				break;

			case "north-y":
				northY = Convert.ToSingle(value);
				break;

			case "south-x":
				southX = Convert.ToSingle(value);
				break;

			case "south-y":
				southY = Convert.ToSingle(value);
				break;

			default:
				Debug.Log ("Unknown property " + name);
				break;
			}
		}
	}

	private ValidDirection parseValidDirections (string directions) {
		if (directions == null) {
			return ValidDirection.None;
		}

		ValidDirection valid = ValidDirection.None;
		string[] parts = directions.Split(',');
		foreach (var part in parts) {
			switch (part) {
			case "north":
				valid |= ValidDirection.North;
				break;

			case "south":
				valid |= ValidDirection.South;
				break;

			case "east":
				valid |= ValidDirection.East;
				break;

			case "west":
				valid |= ValidDirection.West;
				break;
			}
		}

		return valid;
	}

	public bool Walkable(Tile fromTile, Map.Direction directionOfTravel) {
		if (validDirections == ValidDirection.None || fromTile.validDirections == ValidDirection.None) {
			return false;
		}

		ValidDirection toDirectionMask = ValidDirection.None;
		ValidDirection fromDirectionMask = ValidDirection.None;
		switch (directionOfTravel) {
		case Map.Direction.North:
			toDirectionMask = ValidDirection.South;
			fromDirectionMask = ValidDirection.North;
			break;

		case Map.Direction.South:
			toDirectionMask = ValidDirection.North;
			fromDirectionMask = ValidDirection.South;
			break;

		case Map.Direction.East:
			toDirectionMask = ValidDirection.West;
			fromDirectionMask = ValidDirection.East;
			break;

		case Map.Direction.West:
			toDirectionMask = ValidDirection.East;
			fromDirectionMask = ValidDirection.West;
			break;
		}

		return ((validDirections & toDirectionMask) != ValidDirection.None) && ((fromTile.validDirections & fromDirectionMask) != ValidDirection.None);
	}

	public Vector2 edgeForDirection(Map.Direction direction) {
		float x = -1.0f, y = -1.0f;

		switch (direction) {
		case Map.Direction.North:
			x = northX;
			y = northY;
			break;

		case Map.Direction.South:
			x = southX;
			y = southY;
			break;

		case Map.Direction.East:
			x = eastX;
			y = eastY;
			break;

		case Map.Direction.West:
			x = westX;
			y = westY;
			break;
		}

		return new Vector2(x, y);
	}
}
