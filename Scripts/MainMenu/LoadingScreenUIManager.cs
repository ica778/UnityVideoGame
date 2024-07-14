using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingScreenUIManager : MonoBehaviour {

    private void Awake() {
        gameObject.SetActive(false);

        LobbyHandler.Instance.OnStartConnecting += LobbyHandler_OnStartConnecting;
        ConnectionManager.Instance.OnEndConnecting += ConnectionManager_OnEndConnecting;
    }

    private void ConnectionManager_OnEndConnecting(object sender, System.EventArgs e) {
        gameObject.SetActive(false);
    }

    private void LobbyHandler_OnStartConnecting(object sender, System.EventArgs e) {
        gameObject.SetActive(true);
    }

    private void OnDestroy() {
        LobbyHandler.Instance.OnStartConnecting -= LobbyHandler_OnStartConnecting;
        ConnectionManager.Instance.OnEndConnecting -= ConnectionManager_OnEndConnecting;
    }
}