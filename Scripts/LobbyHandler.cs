using HeathenEngineering.SteamworksIntegration;
using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

public class LobbyHandler : MonoBehaviour {
    public static LobbyHandler Instance { get; private set; }

    [SerializeField] private LobbyManager lobbyManager;

    public event EventHandler OnStartConnecting;

    private void Awake() {
        Instance = this;
    }

    private void Start () {
        HeathenEngineering.SteamworksIntegration.API.Overlay.Client.EventGameLobbyJoinRequested.AddListener(OnJoinRequestAccepted);
        lobbyManager.evtAskedToLeave.AddListener(OnAskedToLeave);
        lobbyManager.evtEnterSuccess.AddListener(OnJoinLobbySuccess);
        lobbyManager.evtCreated.AddListener(OnLobbyCreateSuccess);
    }

    // TODO: add proper error handling for hosting and joining games
    private void OnJoinLobbySuccess(LobbyData lobbyData) {
        Debug.Log("LOBBY JOINED SUCCESS +++++++++++++++++++++++");
        ConnectionManager.Instance.StartGameAsClientSteam(lobbyData.Owner.user.id);
    }

    private void OnAskedToLeave() {
        Debug.Log("LOBBY asked TYO LEAVE +++++++++++++++++++++++");
        ConnectionManager.Instance.Quit();
    }

    private void OnJoinRequestAccepted(LobbyData lobbyData, UserData userData) {
        JoinLobbyAsGuest(lobbyData);
    }

    private void OnLobbyCreateSuccess(LobbyData lobbyData) {
        lobbyData.Name = lobbyData.Owner.user.Name + "'s Lobby";
        ConnectionManager.Instance.StartGameAsHostSteam();
    }

    // This is what you use to join game, dont need to go into ConnectionManager, just call this function
    public void JoinLobbyAsGuest(LobbyData lobbyData) {
        OnStartConnecting?.Invoke(this, EventArgs.Empty);
        lobbyManager.Join(lobbyData);
    }

    // This is what you use to create a game as host. Dont need to go into ConnectionManager, just call this function
    public void CreateLobby() {
        OnStartConnecting?.Invoke(this, EventArgs.Empty);
        lobbyManager.Create();
    }

    public void InvitePlayer(UserData userData) {
        lobbyManager.Invite(userData);
    }

    public LobbyManager GetLobbyManager() {
        return lobbyManager;
    }

    public void Kick(UserData userData) {
        lobbyManager.KickMember(userData);
    }

    public void Leave() {
        lobbyManager.Leave();
    }

}