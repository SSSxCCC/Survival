using UnityEngine;

public class CommonExplosion<T> where T : MonoBehaviour, IExplosion
{
    private T explosion;

	public CommonExplosion(T explosion)
    {
        this.explosion = explosion;
    }

    public void OnTriggerEnter2D(Collider2D collider)
    {
        IState state = collider.GetComponent<IState>();
        if (AttackerUtility.ShouldDamage(explosion, state))
            state.TakeAttack(explosion.attack);

        if (state != null)
            state.stun = Mathf.Max(state.stun, 0.5f);
    }
}