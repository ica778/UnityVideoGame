using FishNet;
using FishNet.Connection;
using HeathenEngineering.SteamworksIntegration;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UserPanel : MonoBehaviour {
    [SerializeField] private RawImage avatarImage;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private Button kickButton;

    private UserData userData;

    private void Awake() {
        kickButton.onClick.AddListener(Kick);

        if (!InstanceFinder.IsServerStarted) {
            kickButton.gameObject.SetActive(false);
        }
    }


    public void Initialize(UserData userData) {
        userData.LoadAvatar(SetAvatar);
        nameText.text = userData.Name;
        this.userData = userData;

        if (userData.IsMe) {
            kickButton.gameObject.SetActive(false);
        }
    }

    private void SetAvatar(Texture2D userAvatar) {
        avatarImage.texture = userAvatar;
    }

    public UserData GetUserData() {
        return userData;
    }

    private void Kick() {
        LobbyHandler.Instance.Kick(userData);
    }
}