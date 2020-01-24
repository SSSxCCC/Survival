using UnityEngine;

public class CommonTouchAttacker<T> where T : MonoBehaviour, ITouchAttacker
{
    private T touchAttacker;

    private IState myState;
    private ContactPoint2D[] contacts = new ContactPoint2D[1];

    public CommonTouchAttacker(T touchAttacker)
    {
        this.touchAttacker = touchAttacker;
    }

    public void Start()
    {
        touchAttacker.owner = touchAttacker.gameObject;
        myState = touchAttacker.GetComponent<IState>();
    }

    public void Assault(Collision2D collision)
    {
        if (myState != null && myState.health <= 0) return; // 自己不能是死的
        if (Time.time - touchAttacker.lastAssaultTime < touchAttacker.assaultInterval) return; // 攻击间隔检查

        IState state = collision.gameObject.GetComponent<IState>();
        if (AttackerUtility.ShouldDamage(touchAttacker, state))
        {
            state.TakeAttack(touchAttacker.attack); // 造成伤害
            touchAttacker.lastAssaultTime = Time.time;
            if (collision.GetContacts(contacts) > 0)
            {
                touchAttacker.Push(collision.gameObject, contacts[0].point); // 推开受伤者
                state.DamagedEffect(contacts[0].point); // 产生受伤特效
            }
        }
    }

    public void Push(GameObject pushedObject, Vector2 contactPoint)
    {
        if (pushedObject == null) return;

        Vector2 pushForce = (contactPoint - (Vector2)touchAttacker.transform.position).normalized * touchAttacker.thrust;
        pushedObject.GetComponent<Rigidbody2D>().AddForceAtPosition(pushForce, contactPoint, ForceMode2D.Impulse);
    }
}
