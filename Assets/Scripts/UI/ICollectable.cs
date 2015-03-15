using UnityEngine;
using System.Collections;

public interface ICollectable {
	MapPoint CollectMapPoint {get;}
	void PickUp();
}
