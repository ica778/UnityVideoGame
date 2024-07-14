using UnityEngine;

public class CharacterHealth : MonoBehaviour {
    [SerializeField] private BaseDeathHandler deathHandler;
    [SerializeField] private int health;

    private int Health {
        get { return health; }
        set {
            health = value;
            CheckIfCharacterDead();
        }
    }

    private void OnValidate() {
        if (health <= 0) {
            Debug.LogError("health is not assigned a correct value!", this);
        }
        if (!deathHandler) {
            Debug.LogError("deathHandler is not assigned a correct value!", this);
        }
    }

    private void CheckIfCharacterDead() {
        if (health <= 0) {
            deathHandler.OnCharacterDeath();
            this.gameObject.SetActive(false);
        }
    }

    public void ApplyDamage(int damage) {
        Health -= damage;
    }
}