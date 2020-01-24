using UnityEngine;

public class CommonAmmoAttacker<T> where T : MonoBehaviour, IAmmoAttacker
{
    private T ammoAttacker;

    private ContactPoint2D[] contacts = new ContactPoint2D[1];

    public CommonAmmoAttacker(T ammoAttacker)
    {
        this.ammoAttacker = ammoAttacker;
    }

    public void Start()
    {
        IPlayerController playerController = ammoAttacker.owner.GetComponent<IPlayerController>();
        if (playerController != null && playerController.vehicle != null)
        {
            Physics2D.IgnoreCollision(playerController.vehicle.GetComponent<Collider2D>(), ammoAttacker.GetComponent<Collider2D>());
        }
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        IState state = collision.gameObject.GetComponent<IState>();
        if (AttackerUtility.ShouldDamage(ammoAttacker, state))
        {
            ammoAttacker.OnAttack(state, collision);
        }
        ammoAttacker.OnDeath(collision);
    }

    public void OnDirectAttack(IState state, Collision2D collision)
    {
        state.TakeAttack(ammoAttacker.attack); // 造成伤害

        if (collision.GetContacts(contacts) > 0) state.DamagedEffect(contacts[0].point); // 产生受伤特效
    }
}
