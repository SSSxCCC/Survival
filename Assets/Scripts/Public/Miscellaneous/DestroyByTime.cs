using UnityEngine;

public class DestroyByTime : MonoBehaviour {
    public float lifeTime = 1f;

    public float remainingTime { get { return startTime + lifeTime - Time.time; } }

    private float startTime;
    
	private void Start () {
        startTime = Time.time;
	}

    private void Update() {
        // 到时间销毁自己
        if (remainingTime <= 0)
            Destroy(gameObject);
    }
}
