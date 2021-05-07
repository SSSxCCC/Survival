using UnityEngine;
using UnityEngine.Networking;

public class Potion : NetworkBehaviour {
    [SerializeField] int m_MaxHealth; // 此药水可以增加的最大生命值
    public int maxHealth { get { return m_MaxHealth; } set { m_MaxHealth = value; } }

    [SerializeField] int m_Health; // 此药水可以增加的生命值
    public int health { get { return m_Health; } set { m_Health = value; } }

    [SerializeField] int m_Defense; // 次药水可以增加的防御力
    public int defense { get { return m_Defense; } set { m_Defense = value; } }

    // 被玩家捡到增加相应状态属性
    [ServerCallback]
    private void OnTriggerEnter2D(Collider2D collider) {
        if (FindOwner(collider)) NetworkServer.Destroy(gameObject);
    }

    [Server]
    private bool FindOwner(Collider2D collider) {
        if (!collider.CompareTag("Player") || collider.GetComponent<PlayerController>().vehicle != null) return false;

        var state = collider.GetComponent<State>();
        if (state.health <= 0) return false;

        state.maxHealth += maxHealth;
        state.health += health;
        state.defense += defense;

        return true;
    }
}
