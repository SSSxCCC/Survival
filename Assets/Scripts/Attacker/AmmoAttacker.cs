using UnityEngine;
using UnityEngine.Networking;

public abstract class AmmoAttacker : Attacker {
    private ContactPoint2D[] contacts = new ContactPoint2D[1];
    
    // 弹药初始化
    protected virtual void Start() {
        var playerController = owner.GetComponent<PlayerController>();
        if (playerController != null && playerController.vehicle != null) {
            Physics2D.IgnoreCollision(playerController.vehicle.GetComponent<Collider2D>(), GetComponent<Collider2D>());
        }
    }

    // 子弹撞击判定造成伤害
    [ServerCallback]
    private void OnCollisionEnter2D(Collision2D collision) {
        var state = collision.gameObject.GetComponent<State>();
        if (AttackerUtility.ShouldDamage(owner, state)) {
            OnAttack(state, collision);
        }
        OnDeath(collision);
    }

    protected void OnDirectAttack(State state, Collision2D collision) {
        state.TakeAttack(attack); // 造成伤害

        if (collision.GetContacts(contacts) > 0) {
            state.DamagedEffect(contacts[0].point); // 产生受伤特效
        }
    }

    // 判定攻击
    [Server]
    public abstract void OnAttack(State state, Collision2D collision);

    // 判定毁掉自己
    [Server]
    public abstract void OnDeath(Collision2D collision);
}
