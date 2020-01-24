
/// <summary>
/// 霰弹枪子弹接口。
/// </summary>
public interface IShotgunBulletAttacker : IAmmoAttacker
{
    int extraNumBullets { get; set; }
    bool isHost { get; set; }
    int shotgunBulletId { get; set; }
}
