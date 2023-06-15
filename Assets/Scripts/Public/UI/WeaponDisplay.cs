using UnityEngine;
using UnityEngine.UI;

public class WeaponDisplay : MonoBehaviour {
    public static WeaponDisplay singleton; // 单例

    public Image weaponImage; // 武器的图片显示
    public Text ammoText; // 武器剩余弹药显示

    private void Awake() {
        singleton = this;
    }

    /// <summary>
    /// 显示武器及其弹药数量
    /// </summary>
    /// <param name="sprite">武器图片（正方形）</param>
    /// <param name="color">武器颜色</param>
    /// <param name="numAmmo">弹药数量</param>
    public void ShowWeapon(Sprite sprite, Color color, int numAmmo) {
        // 显示图片
        weaponImage.sprite = sprite;
        weaponImage.color = color;

        // 显示弹药数量
        ammoText.text = numAmmo == int.MaxValue ? "∞" : numAmmo.ToString();
    }
}