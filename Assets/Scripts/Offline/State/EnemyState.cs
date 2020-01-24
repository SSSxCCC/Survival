using UnityEngine;

namespace Offline
{
    public class EnemyState : State
    {
        private bool isAlive = true;

        // 空血了1秒后销毁
        protected override void OnEmptyHealth()
        {
            if (isAlive) // 保证只调用一次OnEnemyDeath
            {
                isAlive = false;
                GameManager.singleton.OnEnemyDeath(gameObject);
                Invoke("DestroyLater", 1f);
            }
        }

        private void DestroyLater()
        {
            Destroy(gameObject);
        }
    }
}