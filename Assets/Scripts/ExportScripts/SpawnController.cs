using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;

public class SpawnController : MonoBehaviour, ISpawnEnumerator {

    [SerializeField] private bool spawnOnStart = false;
    [SerializeField] private SpawnItemList m_itemList = null;
    private AssetReferenceGameObject m_assetLoadedAsset;
    private GameObject m_instanceObject = null;
    private int m_currIndex = -1;

    private void Start() {
        if (m_itemList == null || m_itemList.AssetReferenceCount == 0) {
            Debug.LogError("Spawn list not setup correctly");
        }
        if (spawnOnStart) { SpawnNext(); }
    }

    private void LoadGameObjectAtIndex(SpawnItemList itemList, int index, Action<GameObject> onLoaded) {
        m_assetLoadedAsset = itemList.GetAssetReferenceAtIndex(index);
        if (m_assetLoadedAsset.OperationHandle.IsValid()) {
            onLoaded(m_assetLoadedAsset.OperationHandle.Result as GameObject);
        } else {
            var loadRoutine = m_assetLoadedAsset.LoadAssetAsync();
            loadRoutine.Completed += (obj) => { onLoaded(obj.Result as GameObject); };
        }
    }

    public void SpawnNext(Action<GameObject> onSpawned = null, Action onEndOfList = null) {

        ClearSpawned();

        if (m_currIndex++ < m_itemList.AssetReferenceCount - 1) {
            LoadGameObjectAtIndex(m_itemList, m_currIndex, (go) => {

                var spawnPosition = new Vector3();
                var spawnRotation = Quaternion.identity;
                var parentTransform = this.transform;
                m_instanceObject = Instantiate(go, spawnPosition, spawnRotation, parentTransform);
                m_instanceObject.name = go.name;
                if (onSpawned != null) { onSpawned(m_instanceObject); }
            });
        } else {
            if (onEndOfList != null) onEndOfList();
        }
    }

    public void ClearSpawned() {
        if (m_instanceObject != null) {
            Destroy(m_instanceObject);
        }
    }

    public void Reset() {
        ClearSpawned();
        m_currIndex = -1;
    }


}



