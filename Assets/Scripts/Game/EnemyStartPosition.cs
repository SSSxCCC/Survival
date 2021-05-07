using UnityEngine;

public class EnemyStartPosition : MonoBehaviour {
    private void Awake() {
        GameManager.RegisterEnemyStartPosition(transform);
    }

    private void OnDestroy() {
        GameManager.UnRegisterEnemyStartPosition(transform);
    }
}
