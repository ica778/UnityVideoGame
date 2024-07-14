using Cinemachine;
using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.GridLayoutGroup;

public class Player : NetworkBehaviour {
    [SerializeField] private List<Behaviour> clientSideScripts = new List<Behaviour>();
    [SerializeField] private List<GameObject> clientSideObjects = new List<GameObject>();
    [SerializeField] private GameObject playerCharacter;

    public GameObject PlayerCharacter => playerCharacter;

    public override void OnStartNetwork() {
        PlayerCharacterManager.Instance.AddPlayer(Owner, GetComponent<Player>());
        Debug.Log("CLIENT CONNECTED WITH Id: " + Owner.ClientId);

        if (!Owner.IsLocalClient) {
            foreach (Behaviour obj in clientSideScripts) {
                obj.enabled = false;
            }

            foreach (GameObject obj in clientSideObjects) {
                obj.SetActive(false);
            }

            //this.enabled = false;
            return;
        }
    }

    public override void OnStartServer() {
        base.ServerManager.Objects.OnPreDestroyClientObjects += ServerManager_OnPreDestroyClientObjects;
    }

    private void ServerManager_OnPreDestroyClientObjects(FishNet.Connection.NetworkConnection conn) {
        if (base.Owner == conn) {
            base.RemoveOwnership();
        }
    }

    public override void OnStopServer() {
        base.ServerManager.Objects.OnPreDestroyClientObjects -= ServerManager_OnPreDestroyClientObjects;
    }
}
