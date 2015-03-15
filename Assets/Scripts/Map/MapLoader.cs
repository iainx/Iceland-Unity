using UnityEngine;
using System.Collections;
using System.Xml.Linq;
using System.IO;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.IO.Compression;

public class MapLoader {
	private enum MapOrientation {
		Orthogonal,
		Isometric,
		Staggered,
		Hexagonal,
		Unsupported
	}
	
	private enum MapStaggerIndex {
		Odd,
		Even,
		Unsupported
	}
	
	private enum MapRenderOrder {
		RightDown,
		RightUp,
		LeftDown,
		LeftUp,
		Unsupported
	}
	
	private enum MapStaggerAxis {
		X,
		Y,
		Unsupported
	}

	public static Map ImportMap(string mapFilename) {
		XDocument input = XDocument.Load(mapFilename);

		MapOrientation mapOrientation = MapOrientation.Unsupported;
		switch (input.Document.Root.Attribute("orientation").Value) {
		case "orthogonal":
			mapOrientation = MapOrientation.Orthogonal;
			break;
			
		case "isometric":
			mapOrientation = MapOrientation.Isometric;
			break;
			
		case "staggered":
			mapOrientation = MapOrientation.Staggered;
			break;
			
		case "hexagonal":
			mapOrientation = MapOrientation.Hexagonal;
			break;
			
		default:
			mapOrientation = MapOrientation.Unsupported;
			break;
		}
		
		if (mapOrientation == MapOrientation.Unsupported)
			throw new NotSupportedException("Map - Unsupported map type: " + input.Document.Root.Attribute("orientation").Value);
		
		String mapName = Path.GetFileNameWithoutExtension(mapFilename);
		float mapWidth = Convert.ToSingle(input.Document.Root.Attribute("width").Value);
		float mapHeight = Convert.ToSingle(input.Document.Root.Attribute("height").Value);
		float mapTileWidth = Convert.ToSingle(input.Document.Root.Attribute("tilewidth").Value);
		float mapTileHeight = Convert.ToSingle(input.Document.Root.Attribute("tileheight").Value);

		MapRenderOrder mapRenderOrder = MapRenderOrder.LeftDown;
		if (input.Document.Root.Attribute("renderorder") != null) {
			switch (input.Document.Root.Attribute("renderorder").Value) {
			case "right-down":
				mapRenderOrder = MapRenderOrder.RightDown;
				break;
				
			case "right-up":
				mapRenderOrder = MapRenderOrder.RightUp;
				break;
				
			case "left-down":
				mapRenderOrder = MapRenderOrder.LeftDown;
				break;
				
			case "left-up":
				mapRenderOrder = MapRenderOrder.LeftUp;
				break;
				
			default:
				mapRenderOrder = MapRenderOrder.Unsupported;
				break;
			}
			
			if (mapRenderOrder == MapRenderOrder.Unsupported)
				throw new NotSupportedException("UTiled - Unsupported map render order: " + input.Document.Root.Attribute("renderorder").Value);
		}

		Debug.Log("Map Name: " + mapName);
		Debug.Log ("   " + mapWidth + "x" + mapHeight + " (" + mapTileWidth + "x" + mapTileHeight + ")");
		Debug.Log ("   RenderOrder: " + mapRenderOrder.ToString());

		Dictionary<UInt32, Tile> gidToSprite = new Dictionary<UInt32, Tile>();

		Int32 tsNum = 1;
		foreach (var elem in input.Document.Root.Elements("tileset")) {
			UInt32 FirstGID = Convert.ToUInt32(elem.Attribute("firstgid").Value);
			XElement tsElem = elem;
			
			String tsImageBaseDir = Path.GetDirectoryName(mapFilename);
			if (elem.Attribute("source") != null) {
				XDocument tsx = XDocument.Load(Path.Combine(tsImageBaseDir, elem.Attribute("source").Value));
				tsElem = tsx.Root;
				tsImageBaseDir = Path.GetDirectoryName(Path.Combine(tsImageBaseDir, elem.Attribute("source").Value));
			}

			/*
			List<UTiledProperty> tilesetProps = new List<UTiledProperty>();
			if (tsElem.Element("properties") != null)
				foreach (var pElem in tsElem.Element("properties").Elements("property"))
					tilesetProps.Add(new UTiledProperty() { Name = pElem.Attribute("name").Value, Value = pElem.Attribute("value").Value });
			*/
			Int32 tsTileWidth = tsElem.Attribute("tilewidth") == null ? 0 : Convert.ToInt32(tsElem.Attribute("tilewidth").Value);
			Int32 tsTileHeight = tsElem.Attribute("tileheight") == null ? 0 : Convert.ToInt32(tsElem.Attribute("tileheight").Value);
			Int32 tsSpacing = tsElem.Attribute("spacing") == null ? 0 : Convert.ToInt32(tsElem.Attribute("spacing").Value);
			Int32 tsMargin = tsElem.Attribute("margin") == null ? 0 : Convert.ToInt32(tsElem.Attribute("margin").Value);
			
			Int32 tsTileOffsetX = 0;
			Int32 tsTileOffsetY = 0;
			if (tsElem.Element("tileoffset") != null) {
				tsTileOffsetX = Convert.ToInt32(tsElem.Element("tileoffset").Attribute("x").Value);
				tsTileOffsetY = Convert.ToInt32(tsElem.Element("tileoffset").Attribute("y").Value);
			}

			foreach (var tile in elem.Elements("tile")) {
				UInt32 gid = Convert.ToUInt32 (tile.Attribute("id").Value);
				var image = tile.Element ("image");
				if (image == null) {
					continue;
				}

				string exits = null;
				/*
				Debug.LogError("Properties: " + tile.Elements("properties"));
				foreach (var property in tile.Elements("properties")) {
				}
*/
				XElement properties = tile.Element ("properties");
				/*
				if (properties != null) {
	 
					foreach (var property in properties.Elements ("property")) {
						Debug.LogError("Attribute: " + property.Name);
						if (property.Attribute ("name") != null && property.Attribute("name").Value == "exits") {
							exits = property.Attribute("value").Value;
							break;
						}
					}
				}
				*/

				float imageWidth = Convert.ToSingle (image.Attribute("width").Value);
				float imageHeight = Convert.ToSingle (image.Attribute("height").Value);
				string sourceFile = image.Attribute("source").Value;
				Tile tileObject = new Tile(sourceFile, properties);

				gidToSprite[gid + FirstGID] = tileObject;
			}
		}

		List<UInt32> gids = null;

		foreach (var lElem in input.Document.Root.Elements()) {
			if (lElem.Name.LocalName.Equals("layer")) {
				gids = parseLayer(lElem);
			}
		}

		if (gids == null) {
			return null;
		}

		return new Map((int)mapWidth, (int)mapHeight, gidToSprite, gids);
	}

	private static List<UInt32> parseLayer(XElement layerElement) {
		List<UInt32> gids = new List<UInt32>();

		if (layerElement.Element("data") != null) {
			if (layerElement.Element("data").Attribute("encoding") != null || layerElement.Element("data").Attribute("compression") != null) {
				
				// parse csv formatted data
				if (layerElement.Element("data").Attribute("encoding") != null && layerElement.Element("data").Attribute("encoding").Value.Equals("csv")) {
					foreach (var gid in layerElement.Element("data").Value.Split(",\n\r".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)) {
						gids.Add(Convert.ToUInt32(gid));
					}
				}
			}
		}

		return gids;
	}
}
