using UnityEngine;

public class CommonRocketAttacker<T> where T : MonoBehaviour, IRocketAttacker
{
    private T rocketAttacker;

    private Rigidbody2D rb2D;

    public CommonRocketAttacker(T rocketAttacker)
    {
        this.rocketAttacker = rocketAttacker;
    }

    public void Start()
    {
        rb2D = rocketAttacker.GetComponent<Rigidbody2D>();
    }

    public void FixedUpdate()
    {
        float radianAngle = rocketAttacker.transform.eulerAngles.z * Mathf.Deg2Rad;
        rb2D.AddForce(new Vector2(Mathf.Cos(radianAngle), Mathf.Sin(radianAngle)) * rocketAttacker.propulsion);
    }

    public GameObject CreateExplosion()
    {
        GameObject explosion = Object.Instantiate(rocketAttacker.explosionPrefab, rocketAttacker.transform.position, Quaternion.identity);
        IExplosion exp = explosion.GetComponent<IExplosion>();
        exp.attack = rocketAttacker.attack;
        exp.owner = rocketAttacker.owner;
        return explosion;
    }
}
