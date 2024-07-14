using UnityEngine;

public class RoomHandler : MonoBehaviour {
    [Header("Dungeon Generation")]
    [SerializeField] private RoomConnectorHandler[] roomEntrances;
    [SerializeField] private RoomConnectorHandler[] roomSpawnConnectors;
    [SerializeField] private DungeonValidator dungeonValidator;

    [Header("Loot Spawning Targets")]
    [SerializeField] private Transform[] regularLootSpawnPoints;
    [SerializeField] private Transform[] guaranteedLootSpawnPoints;
    [SerializeField] private Transform[] requiredLootSpawnPoints;

    public RoomConnectorHandler[] GetRoomConnectors() {
        return roomEntrances;
    }

    public RoomConnectorHandler GetRoomConnector(int index) {
        return roomEntrances[index];
    }

    public DungeonValidator GetDungeonValidator() {
        return dungeonValidator;
    }

    public RoomConnectorHandler[] GetRoomSpawnConnectors() {
        return roomSpawnConnectors;
    }

    public void DisableConnectorColliders() {
        foreach (RoomConnectorHandler roomEntranceConnectorHandler in roomEntrances) {
            roomEntranceConnectorHandler.GetDoorwayCollider().gameObject.SetActive(false);
        }
    }

    public void EnableConnectorColliders() {
        foreach (RoomConnectorHandler roomEntranceConnectorHandler in roomEntrances) {
            roomEntranceConnectorHandler.GetDoorwayCollider().gameObject.SetActive(true);
        }
    }

    public Vector3 GetRoomSpawnVector(RoomConnectorHandler parentRoomConnectorHandler, RoomConnectorHandler newRoomConnectHandler) {
        bool containsRoomConnectorHandler = false;
        foreach (RoomConnectorHandler x in roomSpawnConnectors) {
            if (newRoomConnectHandler.Equals(x)) {
                containsRoomConnectorHandler = true;
                break;
            }
        }

        if (!containsRoomConnectorHandler) {
            Debug.LogException(new System.Exception("ROOM DOES NOT CONTAIN THIS OPENING"), this);
        }

        return parentRoomConnectorHandler.GetDoorwayCollider().transform.position - (GetSpawnNewRoomObjectQuaternion(parentRoomConnectorHandler, newRoomConnectHandler) * (newRoomConnectHandler.GetDoorwayCollider().transform.position));

    }

    // TODO: fix this duplicate code for getting quaternion rotation
    private Quaternion GetSpawnNewRoomObjectQuaternion(RoomConnectorHandler parentRoomConnectorHandler, RoomConnectorHandler newRoomConnectorHandler) {
        Quaternion rotation = newRoomConnectorHandler.transform.rotation * Quaternion.Euler(0, 180, 0);

        return rotation * parentRoomConnectorHandler.transform.rotation;
    }

    public Transform[] GetRegularLootSpawnPoints() {
        return regularLootSpawnPoints;
    }

    public Transform[] GetGuaranteedLootSpawnPoints() {
        return guaranteedLootSpawnPoints;
    }

    public Transform[] GetRequiredLootSpawnPoints() {
        return requiredLootSpawnPoints;
    }
}