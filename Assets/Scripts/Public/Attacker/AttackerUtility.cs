
/// <summary>
/// 攻击者工具类，里面有一些对攻击者脚本很有用的方法
/// </summary>
public class AttackerUtility
{
    /// <summary>
    /// 判断攻击者是否应该伤害受害者
    /// </summary>
    /// <param name="attacker">攻击者</param>
    /// <param name="victimState">受害者状态，可以为null</param>
    /// <param name="playerHurtEachOther">玩家之间是否可以互相伤害</param>
    /// <returns>如果攻击者应该伤害受害者，则返回true，否则返回false</returns>
    public static bool ShouldDamage(IAttacker attacker, IState victimState)
    {
        if (victimState == null) return false; // 受害者无状态，不伤害。

        if (attacker.owner == null) return true; // 攻击者无主人，伤害。

        Group attackerGroup = attacker.owner.GetComponent<IState>().group; // 得到攻击者所属组别。
        if (attackerGroup != victimState.group) return true; // 攻击者与目标组别不同，伤害。
        else if (attackerGroup == Group.Player && Online.GameManager.singleton != null && Online.GameManager.singleton.hurtEachOther) return true; // 在允许互相伤害的联机模式下，攻击者与目标都是玩家，则伤害。
        else return false; // 否则不伤害。
    }
}
