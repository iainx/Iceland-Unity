using UnityEngine;
using System;
using System.Collections;

public class MovementStep : IEquatable<MovementStep> {
	private MapPoint mapPoint;
	public int gScore = 0;
	public int hScore = 0;
	
	public MovementStep parent;

	public MapPoint MapPoint {
		get { return mapPoint; }
	}

	public MovementStep(MapPoint basePoint) {
		mapPoint = basePoint;
	}

	public int fScore() {
		return gScore + hScore;
	}

	// A step is equal is the two map points are equal
	public bool Equals (MovementStep otherStep) {
		return this.mapPoint == otherStep.mapPoint;
	}
}
