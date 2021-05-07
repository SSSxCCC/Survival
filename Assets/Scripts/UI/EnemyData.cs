using UnityEngine;
using UnityEngine.UI;

public class EnemyData : MonoBehaviour {
    public GameObject enemyPrefab;

    public Image image;
    public Text attackValueText;
    public Text defenseValueText;
    public Text healthValueText;

    private void Start() {
        var attacker = enemyPrefab.GetComponent<TouchAttacker>();
        attackValueText.text = attacker.attack.ToString();

        var state = enemyPrefab.GetComponent<State>();
        defenseValueText.text = state.defense.ToString();
        healthValueText.text = state.maxHealth.ToString();

        image.sprite = state.spriteRenderer.sprite;
    }
}
