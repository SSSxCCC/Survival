using UnityEngine.Networking;

namespace Online
{
    public class EnemyState : State
    {
        // 等一会就销毁
        [Server]
        protected override void OnEmptyHealth()
        {
            Invoke("DestroyLater", 1f);
            GameManager.singleton.OnKilledEnemy();
        }

        // 一秒后销毁
        [Server]
        private void DestroyLater()
        {
            NetworkServer.Destroy(gameObject);
        }
    }
}