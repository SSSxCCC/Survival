using UnityEngine;
using UnityEngine.UI;

public class WeaponData : MonoBehaviour {
    public GameObject weaponPrefab;

    public Image image;
    public Text attackValueText;
    public Text intervalValueText;
    public Text speedValueText;

    private void Start() {
        var weapon = weaponPrefab.GetComponent<Weapon>();
        image.sprite = weapon.weaponSprite;
        attackValueText.text = weapon.attack.ToString();
        intervalValueText.text = weapon.fireInterval.ToString();
        speedValueText.text = weapon.ammoSpeed.ToString();
    }
}
