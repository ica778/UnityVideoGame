using FishNet;
using FishNet.Connection;
using FishNet.Object;
using HeathenEngineering.SteamworksIntegration;
using Steamworks;
using System;
using System.Collections;
using UnityEngine;

public class ConnectionManager : MonoBehaviour {
    public static ConnectionManager Instance { get; private set; }

    [SerializeField] private FishySteamworks.FishySteamworks fishySteamworks;

    public event EventHandler OnNetworkTimeout;
    public event EventHandler OnEndConnecting;

    private bool isHost = false;
    private bool isConnected = false;

    private float connectTimer = 5f;

    private Coroutine connectingToServer;

    private void Awake() {
        Instance = this;
    }

    private IEnumerator StartGameAsHostSteamAsync() {
        float timer = connectTimer;

        fishySteamworks.StartConnection(true);
        fishySteamworks.StartConnection(false);

        while (!isConnected && timer > 0) {
            timer -= Time.deltaTime;
            yield return null;
        }
        if (isConnected) {
            isHost = true;
            ClientSideGameSceneManager.Instance.LoadIntoGame(true);
        }
        else {
            OnEndConnecting?.Invoke(this, EventArgs.Empty);
            Debug.LogError("ERROR: DID NOT CONNECT TO SERVER IN TIME====================================");
        }
    }

    // start server and join as host
    public void StartGameAsHostSteam() {
        connectingToServer = StartCoroutine(StartGameAsHostSteamAsync());
    }

    private IEnumerator StartGameAsClientSteamAsync(CSteamID hostCSteamID) {
        float timer = connectTimer;
        UserData hostUser = UserData.Get(hostCSteamID);

        if (!hostUser.IsValid) {
            Debug.LogError("TESTING HOST USER IS NOT VALID");
        }

        fishySteamworks.SetClientAddress(hostCSteamID.ToString());
        fishySteamworks.StartConnection(false);

        while (!isConnected && timer > 0) {
            timer -= Time.deltaTime;
            yield return null;
        }

        if (isConnected) {
            ClientSideGameSceneManager.Instance.LoadIntoGame(false);
        }
        else {
            OnEndConnecting?.Invoke(this, EventArgs.Empty);
            Debug.LogError("ERROR: DID NOT CONNECT TO SERVER IN TIME====================================");
        }
    }

    // join server as guest using host CSteamID
    public void StartGameAsClientSteam(CSteamID hostCSteamID) {
        connectingToServer = StartCoroutine(StartGameAsClientSteamAsync(hostCSteamID));
    }

    public void Disconnect() {
        isConnected = false;
        if (isHost) {
            fishySteamworks.Shutdown();
        }
        else {
            fishySteamworks.StopConnection(false);
        }
        isHost = false;
    }

    public void Quit() {
        LobbyHandler.Instance.Leave();
        Disconnect();
        ClientSideGameSceneManager.Instance.QuitGameBackToMainMenu();

        // if Quit() called while this coroutine is still running, stop trying to connect
        if (connectingToServer != null) {
            OnEndConnecting?.Invoke(this, EventArgs.Empty);
            StopCoroutine(connectingToServer);
        }
    }

    // NOTE: this is for testing multiplayer without having to use steam
    public void StartGameAsHostOffline() {
        connectingToServer = StartCoroutine(StartGameAsHostOfflineAsync());
    }

    private IEnumerator StartGameAsHostOfflineAsync() {
        float timer = connectTimer;

        InstanceFinder.NetworkManager.ServerManager.StartConnection();
        InstanceFinder.NetworkManager.ClientManager.StartConnection();

        while (!isConnected && timer > 0) {
            timer -= Time.deltaTime;
            yield return null;
        }
        if (isConnected) {
            isHost = true;
            ClientSideGameSceneManager.Instance.LoadIntoGame(true);
        }
        else {
            OnEndConnecting?.Invoke(this, EventArgs.Empty);
            Debug.LogError("ERROR: DID NOT CONNECT TO SERVER IN TIME====================================");
        }
    }

    public void StartGameAsClientOffline() {
        connectingToServer = StartCoroutine(StartGameAsClientOfflineAsync());
    }

    private IEnumerator StartGameAsClientOfflineAsync() {
        float timer = connectTimer;

        InstanceFinder.NetworkManager.ClientManager.StartConnection();

        while (!isConnected && timer > 0) {
            timer -= Time.deltaTime;
            yield return null;
        }
        if (isConnected) {
            ClientSideGameSceneManager.Instance.LoadIntoGame(false);
        }
        else {
            OnEndConnecting?.Invoke(this, EventArgs.Empty);
            Debug.LogError("ERROR: DID NOT CONNECT TO SERVER IN TIME====================================");
        }
    }

    public void Kick(NetworkConnection conn) {
        if (!InstanceFinder.IsServerStarted) {
            Debug.LogWarning("================ ONLY SERVER CAN KICK ================");
        }

        conn.Kick(FishNet.Managing.Server.KickReason.Unset);
    }

    private void ClientManager_OnAuthenticated() {
        isConnected = true;
    }

    // NOTE: FOR SOME REASON CANT USE OnClientTimeOut SO I HAVE TO USE THIS
    private void ClientManager_OnClientConnectionState(FishNet.Transporting.ClientConnectionStateArgs obj) {
        if (isConnected && obj.ConnectionState == FishNet.Transporting.LocalConnectionState.Stopped) {
            Quit();
            OnNetworkTimeout?.Invoke(this, EventArgs.Empty);
        }
    }

    private void OnEnable() {
        InstanceFinder.ClientManager.OnAuthenticated += ClientManager_OnAuthenticated;
        InstanceFinder.ClientManager.OnClientConnectionState += ClientManager_OnClientConnectionState;
    }
}