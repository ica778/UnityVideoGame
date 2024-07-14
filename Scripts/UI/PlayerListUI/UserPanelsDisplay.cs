using FishNet;
using FishNet.Connection;
using GameKit.Dependencies.Utilities;
using HeathenEngineering.SteamworksIntegration;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserPanelsDisplay : MonoBehaviour {
    [SerializeField] private GameObject userPanelPrefab;

    private Dictionary<UserData, UserPanel> userPanels = new();

    private LobbyManager lobbyManager;

    private void Awake() {
        lobbyManager = LobbyHandler.Instance.GetLobbyManager();

        foreach (LobbyMemberData lmd in lobbyManager.Members) {
            AddUserPanel(lmd.user);
        }

        lobbyManager.evtUserJoined.AddListener(AddUserPanel);
        lobbyManager.evtUserLeft.AddListener(RemoveUserPanel);
    }

    private void OnDestroy() {
        lobbyManager.evtUserJoined.RemoveAllListeners();
        lobbyManager.evtUserLeft.RemoveAllListeners();
    }

    private void RemoveUserPanel(UserLobbyLeaveData arg0) {
        if (userPanels.ContainsKey(arg0.user)) {
            Destroy(userPanels[arg0.user].gameObject);
            userPanels.Remove(arg0.user);
        }
    }

    private void AddUserPanel(UserData userData) {
        GameObject newUserPanel = Instantiate(userPanelPrefab, this.transform);
        UserPanel userPanel = newUserPanel.GetComponent<UserPanel>();
        userPanel.Initialize(userData);
        userPanels[userData] = userPanel;
    }
}
