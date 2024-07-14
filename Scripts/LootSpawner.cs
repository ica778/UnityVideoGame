using FishNet.Object;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootSpawner : NetworkBehaviour {
    public static LootSpawner Instance { get; private set; }

    [Header("Loot Pool")]
    [SerializeField] private GroundLoot[] commonLoot;

    [Header("Loot Spawning Targets in World (Not Dungeon)")]
    [SerializeField] private Transform[] regularLootSpawnPointsWorld;
    [SerializeField] private Transform[] guaranteedLootSpawnPointsWorld;
    [SerializeField] private Transform[] requiredLootSpawnPointsWorld;

    public bool FinishedLootSpawning { get; private set; } = false;
    public event EventHandler OnFinishedLootSpawning;

    public override void OnStartNetwork() {
        if (!base.IsServerInitialized) { 
            this.gameObject.SetActive(false);
            return; 
        }

        Instance = this;

        DungeonGenerator.Instance.OnFinishedDungeonGeneration += DungeonGenerator_OnFinishedDungeonGeneration;
    }

    private void DungeonGenerator_OnFinishedDungeonGeneration(object sender, System.EventArgs e) {
        StartCoroutine(SpawnLoot());
    }

    private IEnumerator SpawnLoot() {
        yield return StartCoroutine(SpawnDungeonLoot());
        Debug.Log("FINISHED SPAWNING LOTTT ---------------");

        FinishedLootSpawning = true;
        OnFinishedLootSpawning?.Invoke(this, EventArgs.Empty);
    }

    private IEnumerator SpawnDungeonLoot() {
        List<RoomHandler> dungeonRooms = DungeonGenerator.Instance.GetSpawnedRooms();

        foreach (RoomHandler room in dungeonRooms) {
            Transform[] regularDungeonLootSpawnPoints = room.GetRegularLootSpawnPoints();

            foreach (Transform spawnPoint in regularDungeonLootSpawnPoints) {
                if (UnityEngine.Random.Range(0, 10) < 5) {
                    continue;
                }

                GameObject lootToSpawn = commonLoot[UnityEngine.Random.Range(0, commonLoot.Length)].gameObject;

                AsyncInstantiateOperation<GameObject> asyncInstantiateOperation = InstantiateAsync<GameObject>(lootToSpawn, spawnPoint.position, spawnPoint.rotation);

                while (!asyncInstantiateOperation.isDone) {
                    yield return null;
                }

                GameObject spawnedLoot = asyncInstantiateOperation.Result[0];
                base.ServerManager.Spawn(spawnedLoot);
            }
        }
    }

    public override void OnStopNetwork() {
        DungeonGenerator.Instance.OnFinishedDungeonGeneration -= DungeonGenerator_OnFinishedDungeonGeneration;
    }
}