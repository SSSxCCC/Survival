using UnityEngine;
using UnityEngine.Networking;

public class Explosion : Attacker {
    // 对碰到爆炸的单位造成伤害
    [ServerCallback]
    private void OnTriggerEnter2D(Collider2D collider) {
        var state = collider.GetComponent<State>();
        if (AttackerUtility.ShouldDamage(owner, state))
            state.TakeAttack(attack);

        if (state != null)
            state.stun = Mathf.Max(state.stun, 0.5f);
    }
}
