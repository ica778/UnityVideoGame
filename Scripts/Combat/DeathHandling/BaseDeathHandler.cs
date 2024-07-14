using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseDeathHandler : NetworkBehaviour {
    public abstract void OnCharacterDeath();
}