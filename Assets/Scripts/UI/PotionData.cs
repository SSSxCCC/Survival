using UnityEngine;
using UnityEngine.UI;

public class PotionData : MonoBehaviour {
    public GameObject potionPrefab;

    public Image image;
    public Text maxHealthValueText;
    public Text healthValueText;
    public Text defenseValueText;

    private void Start() {
        var potion = potionPrefab.GetComponent<Potion>();
        maxHealthValueText.text = potion.maxHealth.ToString();
        healthValueText.text = potion.health.ToString();
        defenseValueText.text = potion.defense.ToString();

        var renderer = potionPrefab.GetComponentInChildren<SpriteRenderer>();
        image.sprite = renderer.sprite;
        image.color = renderer.color;
    }
}
