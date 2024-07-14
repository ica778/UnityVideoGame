using FishNet.Object;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLoadingScreenUIManager : MonoBehaviour {
    //public override void OnStartNetwork() {
    private void Start() {
        gameObject.SetActive(false);
        SceneLoading.Instance.OnBeginLoadingLevelScenes += SceneLoading_OnBeginLoadingLevelScenes;
        SceneLoading.Instance.OnFinishedLoadingLevelScenes += SceneLoading_OnFinishedLoadingLevelScenes;
    }

    private void SceneLoading_OnBeginLoadingLevelScenes(object sender, EventArgs e) {
        gameObject.SetActive(true);
    }

    private void SceneLoading_OnFinishedLoadingLevelScenes(object sender, EventArgs e) {
        gameObject.SetActive(false);
    }

    //public override void OnStopNetwork() {
    private void OnDestroy() {
        SceneLoading.Instance.OnBeginLoadingLevelScenes -= SceneLoading_OnBeginLoadingLevelScenes;
        SceneLoading.Instance.OnFinishedLoadingLevelScenes -= SceneLoading_OnFinishedLoadingLevelScenes;
    }
}