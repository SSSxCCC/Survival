
/// <summary>
/// 射击武器，在攻击范围内会自动开火。
/// </summary>
public interface IEnemyWeapon : IShooter
{
    float attackRange { get; set; }
}
