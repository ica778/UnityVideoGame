using FishNet.Connection;
using FishNet.Object;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneLoading : NetworkBehaviour {
    public static SceneLoading Instance { get; private set; }

    public event EventHandler OnHostFinishedLoadingStartScenes;

    public event EventHandler OnFinishedLoadingStartScenes;

    public event EventHandler OnBeginLoadingLevelScenes;
    public event EventHandler OnFinishedLoadingLevelScenes;

    private Dictionary<NetworkConnection, bool> clientsLoaded;

    private int timesToCheckIfLoadingComplete = 20;
    private float delayBetweenChecks = 0.5f;

    private void Awake() {
        Instance = this;
    }

    public override void OnStartNetwork() {
        if (base.IsServerInitialized) {
            base.SceneManager.OnClientLoadedStartScenes += SceneManager_OnClientFinishedLoadingStartScenes;
        }
    }

    public override void OnStopNetwork() {
        if (base.IsServerInitialized) {
            base.SceneManager.OnClientLoadedStartScenes -= SceneManager_OnClientFinishedLoadingStartScenes;
        }
    }

    /// <summary>
    /// Wait for host to finish loading start scenes.
    /// </summary>
    /// <param name="sceneNames">Integer array of SceneNames of start scenes</param>
    public void WaitForHostToLoadStartScenes(SceneName[] sceneNames) {
        StartCoroutine(WaitForHostToLoadStartScenesAsync(sceneNames));
    }

    private IEnumerator WaitForHostToLoadStartScenesAsync(SceneName[] sceneNames) {
        int timesToCheck = timesToCheckIfLoadingComplete;
        while (!CheckIfScenesAreLoaded(sceneNames) && timesToCheck > 0) {
            timesToCheck--;
            yield return new WaitForSeconds(delayBetweenChecks);
        }

        if (timesToCheck <= 0) {
            Debug.LogError("ERROR: TIMEOUT SCENE NOT LOADING");
        }

        OnHostFinishedLoadingStartScenes?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Notify server that NetworkConnection conn is done loading start scenes.
    /// </summary>
    private void SceneManager_OnClientFinishedLoadingStartScenes(NetworkConnection arg1, bool arg2) {
        if (base.IsServerOnlyInitialized) {
            return;
        }
        
        if (arg2) {
            ClientFinishedLoadingStartScenesTargetRpc(arg1);
        }
    }

    /// <summary>
    /// Notify client of NetworkConnection conn that they are done loading start scenes.
    /// </summary>
    [TargetRpc]
    private void ClientFinishedLoadingStartScenesTargetRpc(NetworkConnection conn) {
        OnFinishedLoadingStartScenes?.Invoke(this, EventArgs.Empty);
    }

    public void ResetClientsLoaded() {
        clientsLoaded = new();
        foreach (NetworkConnection conn in base.ClientManager.Clients.Values) {
            clientsLoaded[conn] = false;
        }
    }

    /// <summary>
    /// ObserversRpc that tells clients to wait for themselves to load scenes then notifies server when done loading.
    /// </summary>
    /// <param name="sceneNameHashCodes">Integer array of SceneName HashCodes</param>
    [ObserversRpc]
    public void WaitForClientsToLoadLevelScenes(int[] sceneNameHashCodes) {
        OnBeginLoadingLevelScenes?.Invoke(this, EventArgs.Empty);
        SceneName[] sceneNames = new SceneName[sceneNameHashCodes.Length];

        for (int i = 0; i < sceneNameHashCodes.Length; i++) {
            sceneNames[i] = (SceneName)sceneNameHashCodes[i];
        }

        StartCoroutine(WaitForClientToLoadLevelScenes(sceneNames));

    }

    private IEnumerator WaitForClientToLoadLevelScenes(SceneName[] sceneNames) {
        int timesToCheck = timesToCheckIfLoadingComplete;
        while (!CheckIfScenesAreLoaded(sceneNames) && timesToCheck > 0) {
            timesToCheck--;
            yield return new WaitForSeconds(delayBetweenChecks);
        }

        while (DungeonGenerator.Instance && !DungeonGenerator.Instance.FinishedDungeonGeneration && timesToCheck > 0) {
            timesToCheck--;
            yield return new WaitForSeconds(delayBetweenChecks);
        }

        while (LootSpawner.Instance && !LootSpawner.Instance.FinishedLootSpawning && timesToCheck > 0) {
            timesToCheck--;
            yield return new WaitForSeconds(delayBetweenChecks);
        }

        if (timesToCheck <= 0) {
            Debug.LogError("ERROR: TIMEOUT SCENE NOT LOADING");
        }

        NotifyServerThatClientFinishedLoadingLevelScenesServerRpc(base.LocalConnection);
    }

    private bool CheckIfScenesAreLoaded(SceneName[] sceneNames) {
        foreach (SceneName sceneName in sceneNames) {
            if (!SceneHelper.GetScene(sceneName).isLoaded) {
                return false;
            }
        }
        return true;
    }

    [ServerRpc(RequireOwnership = false)]
    private void NotifyServerThatClientFinishedLoadingLevelScenesServerRpc(NetworkConnection conn) {
        clientsLoaded[conn] = true;
        foreach (bool current in clientsLoaded.Values) {
            if (!current) {
                return;
            }
        }

        // server notify all clients everyone is done loading level scenes
        NotifyClientsFinishedLoadingLevelScenes();
    }

    [ObserversRpc]
    private void NotifyClientsFinishedLoadingLevelScenes() {
        OnFinishedLoadingLevelScenes?.Invoke(this, EventArgs.Empty);
    }
}