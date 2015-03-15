using UnityEngine;
using System;
using System.Collections;

public struct MapPoint : IEquatable<MapPoint> {
	public int row;
	public int column;

	public MapPoint(int r, int c) {
		row = r;
		column = c;
	}

	public override string ToString ()
	{
		return string.Format ("({0},{1})", row, column);
	}

	public bool Equals(MapPoint other) {
		return (other.row == row) && (other.column == column);
	}

	public override bool Equals(object obj) {
		if (obj == null || GetType() != obj.GetType()) {
			return false;
		}

		var other = (MapPoint)obj;
		return (other.row == row) && (other.column == column);
	}

	public override int GetHashCode()
	{
		var calculation = row + column;
		return calculation.GetHashCode();
	}

	public static bool operator ==(MapPoint p1, MapPoint p2) 
	{
		return p1.Equals(p2);
	}
	
	public static bool operator !=(MapPoint p1, MapPoint p2) 
	{
		return !p1.Equals(p2);
	}
}
