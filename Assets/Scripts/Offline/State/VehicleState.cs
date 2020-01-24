
namespace Offline
{
    public class VehicleState : State
    {
        // 等一会就销毁
        protected override void OnEmptyHealth()
        {
            Invoke("DestroyLater", 1f);
        }

        // 一秒后销毁
        private void DestroyLater()
        {
            GetComponent<Vehicle>().LeaveAll(); // 为了使玩家武器显示正常回归
            Destroy(gameObject);
        }
    }
}