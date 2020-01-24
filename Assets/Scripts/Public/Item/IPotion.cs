/// <summary>
/// 药品接口。
/// </summary>
public interface IPotion
{
    int maxHealth { get; set; }
    int health { get; set; }
    int defense { get; set; }
}