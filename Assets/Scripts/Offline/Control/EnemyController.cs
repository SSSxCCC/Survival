using UnityEngine;

namespace Offline
{
    public class EnemyController : Controller, IEnemyController
    {
        GameObject m_TargetPlayer; // 这个敌人盯上的玩家
        public GameObject targetPlayer { get { return m_TargetPlayer; } set { m_TargetPlayer = value; } }

        private IState state; // 状态

        private float lastWanderTime; // 上一次游荡时间
        private Vector2 wanderDestination; // 游荡目的地

        private CommonEnemyController<EnemyController> commonEnemyController; // 公共实现

        protected override void Awake()
        {
            base.Awake();
            commonEnemyController = new CommonEnemyController<EnemyController>(this);
        }

        protected override void Start()
        {
            base.Start();

            state = GetComponent<IState>();
            wanderDestination = transform.position;
        }

        // AI
        private void Update()
        {
            if (state.health <= 0)
            {
                return;
            }

            // 更新进攻目标
            UpdateTarget();

            if (targetPlayer == null) // 没有进攻目标
            {
                Wander();
            }
            else // 有进攻目标
            {
                GoToward(targetPlayer.transform.position);
            }
        }

        // 找一个目标
        private void UpdateTarget()
        {
            if (targetPlayer == null || HasObstacleTo(targetPlayer.transform.position) || targetPlayer.GetComponent<IState>().health <= 0) // 没有当前目标或与当前目标之间有障碍或当前目标死亡
            {
                targetPlayer = FindNearestPlayer();
            }
        }

        // 找到一个中间无障碍物的最近玩家
        private GameObject FindNearestPlayer()
        {
            float minSqrDistance = float.MaxValue;
            GameObject nearestPlayer = null;
            GameObject[] allPlayers = GameObject.FindGameObjectsWithTag("Player");
            foreach (GameObject player in allPlayers)
            {
                if (!HasObstacleTo(player.transform.position) && player.GetComponent<IState>().health > 0)
                {
                    float sqrDistance = Vector2.SqrMagnitude(transform.position - player.transform.position);
                    if (sqrDistance < minSqrDistance)
                    {
                        minSqrDistance = sqrDistance;
                        nearestPlayer = player;
                    }
                }
            }
            return nearestPlayer;
        }

        // 随机游荡
        private void Wander()
        {
            if (Vector2.Distance(wanderDestination, transform.position) < 0.1f || Time.time > lastWanderTime + 3f)
            {
                float x = transform.position.x;
                float y = transform.position.y;
                wanderDestination = new Vector2(Random.Range(x - 10f, x + 10f), Random.Range(y - 10f, y + 10f));
                lastWanderTime = Time.time;
            }

            GoToward(wanderDestination);
        }

        // 向目标前进
        public void GoToward(Vector2 targetPosition)
        {
            commonEnemyController.GoToward(targetPosition);
        }
    }
}