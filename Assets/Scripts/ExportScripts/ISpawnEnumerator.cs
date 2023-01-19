using System;
using UnityEngine;

public interface ISpawnEnumerator
{
	void Reset();
	void SpawnNext(Action<GameObject> onSpawned = null, Action onEndOfList = null);
	void ClearSpawned();
}



