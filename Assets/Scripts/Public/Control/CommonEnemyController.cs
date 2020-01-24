using UnityEngine;

public class CommonEnemyController<T> where T : MonoBehaviour, IEnemyController
{
    private T enemyController;

    public CommonEnemyController(T enemyController)
    {
        this.enemyController = enemyController;
    }

    public void GoToward(Vector2 targetPosition)
    {
        enemyController.LookAt(targetPosition);
        enemyController.Move(new Vector2(targetPosition.x - enemyController.transform.position.x, targetPosition.y - enemyController.transform.position.y).normalized);
    }
}
