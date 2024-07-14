using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDropsHandler : NetworkBehaviour {
    [SerializeField] private ItemSO[] lootPool;


    [ServerRpc(RequireOwnership = false)]
    public void SpawnDropsServerRpc(Vector3 position) {
        StartCoroutine(SpawnDropsAsync(position));
    }

    private IEnumerator SpawnDropsAsync(Vector3 position) {
        foreach (ItemSO item in lootPool) {
            AsyncInstantiateOperation<GameObject> asyncInstantiateOperation = InstantiateAsync(item.GroundLootObject, position, Quaternion.identity);

            while (!asyncInstantiateOperation.isDone) {
                yield return null;
            }

            GameObject newLootObject = asyncInstantiateOperation.Result[0];
            base.ServerManager.Spawn(newLootObject);
        }
    }

}
