using UnityEngine;

namespace Online
{
    public class EnemyStartPosition : MonoBehaviour
    {
        private void Awake()
        {
            GameManager.RegisterEnemyStartPosition(transform);
        }

        private void OnDestroy()
        {
            GameManager.UnRegisterEnemyStartPosition(transform);
        }
    }
}