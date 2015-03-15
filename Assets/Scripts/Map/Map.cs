using UnityEngine;
using System;
using System.Collections.Generic;

public class Map {
	public enum Direction {
		North = 0,
		South,
		East,
		West
	};

	public int width;
	public int height;

	public Dictionary<UInt32, Tile> GIDToSprite;
	public List<UInt32> tiles;

	public Map (int width, int height, Dictionary<UInt32, Tile>GIDToSprite, List<UInt32> tiles) {
		this.width = width;
		this.height = height;
		this.GIDToSprite = GIDToSprite;
		this.tiles = tiles;
	}

	public bool IsValidPosition(MapPoint mapPoint) {
		return (mapPoint.row >= 0 && mapPoint.row < height && mapPoint.column >= 0 && mapPoint.column < width);
	}

	public Tile TileAtMapPoint(MapPoint mapPoint) {
		UInt32 tileID = tiles[(int)(mapPoint.column + (mapPoint.row * width))];
		return GIDToSprite[tileID];
	}	

	public static Map.Direction CalculateDirectionTravelling(MapPoint fromPoint, MapPoint toPoint) {
		int dRow = fromPoint.row - toPoint.row;
		int dCol = fromPoint.column - toPoint.column;
		
		if (dRow < 0) {
			return Map.Direction.West;
		} else if (dRow > 0) {
			return Map.Direction.East;
		} else if (dCol < 0) {
			return Map.Direction.South;
		} else {
			return Map.Direction.North;
		}
	}

	public Vector2 ObjectPositionAtMapPoint(MapPoint mapPoint) {
		Vector2 worldPosition = MapLayout.ScreenPointFromMapPoint(mapPoint);
		Tile tile = TileAtMapPoint(mapPoint);

		Debug.Log("Tile centre :" + tile.centreX +"," + tile.centreY);

		worldPosition.x += (tile.centreX / 100.0f) - 0.5f;
		worldPosition.y += (tile.centreY / 100.0f);

		return worldPosition;
	}

	private void InsertStepIntoList(MovementStep step, List<MovementStep> list) {
		int stepFScore = step.fScore();
		int count = list.Count;
		int i;
		
		// Keep the list sorted by fscore
		for (i = 0; i < count; i++) {
			MovementStep currentStep = list[i];
			if (stepFScore < currentStep.fScore()) {
				break;
			}
		}
		
		list.Insert(i, step);
	}	
	
	private List<MapPoint>WalkableAdjacentTiles(MapPoint mapPoint) {
		List<MapPoint> walkableTiles = new List<MapPoint>();
		Tile tile, currentTile;

		currentTile = TileAtMapPoint(mapPoint);
		
		Map.Direction direction;
		
		MapPoint p = new MapPoint (mapPoint.row, mapPoint.column + 1);
		direction =  Map.CalculateDirectionTravelling(mapPoint, p);
		
		if (IsValidPosition(p)) {
			tile = TileAtMapPoint(p);
			if (tile.Walkable(currentTile, direction)) {
				walkableTiles.Add(p);
			}
		}
		
		p = new MapPoint (mapPoint.row, mapPoint.column - 1);
		direction = Map.CalculateDirectionTravelling(mapPoint, p);
		if (IsValidPosition(p)) {
			tile = TileAtMapPoint(p);
			if (tile.Walkable(currentTile, direction)) {
				walkableTiles.Add(p);
			}
		}
		
		p = new MapPoint (mapPoint.row + 1, mapPoint.column);
		direction = Map.CalculateDirectionTravelling(mapPoint, p);
		if (IsValidPosition(p)) {
			tile = TileAtMapPoint(p);
			
			if (tile.Walkable(currentTile, direction)) {
				walkableTiles.Add(p);
			}
		}
		
		p = new MapPoint (mapPoint.row - 1, mapPoint.column);
		direction = Map.CalculateDirectionTravelling(mapPoint, p);
		if (IsValidPosition(p)) {
			tile = TileAtMapPoint(p);
			
			if (tile.Walkable(currentTile, direction)) {
				walkableTiles.Add(p);
			}
		}
		
		return walkableTiles;
	}
	
	private int CostToMove(MovementStep fromStep, MovementStep toStep) {
		// Because we can't move diagonally and because terrain is just walkable or unwalkable the cost is always the same.
		// But it can to be different if we can move diagonally and/or if there is swamps, hills, etc...
		return 1;
	}
	
	private int ComputeHScoreForMovement(MapPoint fromPoint, MapPoint toPoint) {
		return Convert.ToInt32(Math.Abs(toPoint.row - fromPoint.row) + Math.Abs(toPoint.column - fromPoint.column));
	}
	
	public List<MovementStep> GeneratePath(MapPoint fromPoint, MapPoint toPoint) {
		Debug.Log ("Generating path from " + fromPoint + " to " + toPoint);
		
		if (fromPoint == toPoint) {
			Debug.Log ("Already at point");
			// FIXME: is this the best way, or should I return an empty list?
			return null;
		}
		
		List<MovementStep> openSteps = new List<MovementStep>();
		List<MovementStep> closedSteps = new List<MovementStep>();
		
		// Start by adding the from position to the open list
		MovementStep firstStep = new MovementStep(fromPoint);
		InsertStepIntoList(firstStep, openSteps);
		
		do {
			MovementStep currentStep = openSteps[0];
			
			// Move the first step into the closedSteps list
			closedSteps.Add(currentStep);
			openSteps.RemoveAt(0);
			
			if (currentStep.MapPoint == toPoint) {
				List<MovementStep> shortestPath = new List<MovementStep>();
				do {
					if (currentStep.parent != null) {
						shortestPath.Insert(0, currentStep);
					}
					currentStep = currentStep.parent;
				} while (currentStep != null);
				
				return shortestPath;
			}
			
			List<MapPoint> walkableTiles = WalkableAdjacentTiles(currentStep.MapPoint);
			foreach (MapPoint mapPoint in walkableTiles) {
				MovementStep step = new MovementStep(mapPoint);
				if (closedSteps.Contains (step)) {
					continue;
				}
				
				// Compute the cost from the current step to that step
				int moveCost = CostToMove(currentStep, step);
				
				int idx = openSteps.IndexOf(step);
				if (idx == -1) {
					step.parent = currentStep;
					step.gScore = currentStep.gScore + moveCost;
					step.hScore = ComputeHScoreForMovement(step.MapPoint, toPoint);
					
					InsertStepIntoList(step, openSteps);
				} else {
					step = openSteps[idx];
					if (currentStep.gScore + moveCost < step.gScore) {
						step.gScore = currentStep.gScore + moveCost;
						
						// As gscore has changed, fscore has changed too, so reinsert it into the list
						openSteps.RemoveAt(idx);
						InsertStepIntoList(step, openSteps);
					}
				}
			}
		} while (openSteps.Count > 0);
		
		Debug.Log ("Cannot reach destination " + toPoint + " from " + fromPoint);
		return null;
	}
}
