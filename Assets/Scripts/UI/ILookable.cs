using UnityEngine;
using System.Collections;

public interface ILookable {
	string LookAt();
	MapPoint LookMapPoint {get;}
}
