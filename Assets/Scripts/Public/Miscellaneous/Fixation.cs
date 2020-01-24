using UnityEngine;

public class Fixation : MonoBehaviour {

    public Transform owner;
    public Vector2 offset;

    // 每一帧固定其位置与旋转
    private void LateUpdate()
    {
        transform.position = owner.position + (Vector3)offset;
        transform.rotation = Quaternion.identity;
    }
}
