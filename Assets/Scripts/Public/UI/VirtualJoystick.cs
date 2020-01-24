using UnityEngine;

public class VirtualJoystick : MonoBehaviour
{
    public Transform joystick;
    public Transform background;

    private float radius; // 虚拟摇杆半径
    private int currentFingerId = int.MinValue; // 当前触屏摇杆的碰触id，值为整数最小值时代表没有

    private void Start()
    {
        Canvas canvas = transform.root.GetComponent<Canvas>();
        radius = ((RectTransform)background).rect.width * canvas.scaleFactor / 2f;
    }

    private void Update()
    {
        Touch[] touches = Input.touches;
        for (int i = 0; i < Input.touchCount; i++)
        {
            if (touches[i].phase == TouchPhase.Began) // 触屏开始
            {
                if (touches[i].position.x <= Screen.width / 2 && touches[i].position.y <= Screen.height / 2 // 左下屏幕用来操作虚拟摇杆
                    && currentFingerId == int.MinValue) // 没有使用摇杆时才认为开始触碰摇杆，为了防误触
                {
                    background.gameObject.SetActive(true);
                    joystick.gameObject.SetActive(true);
                    background.position = touches[i].position;
                    joystick.position = touches[i].position;
                    currentFingerId = touches[i].fingerId;
                }
            }
            else if (touches[i].phase == TouchPhase.Canceled || touches[i].phase == TouchPhase.Ended) // 触屏结束
            {
                if (touches[i].fingerId == currentFingerId)
                {
                    background.gameObject.SetActive(false);
                    joystick.gameObject.SetActive(false);
                    currentFingerId = int.MinValue;
                }
            }
            else // 正在持续触屏
            {
                if (touches[i].fingerId == currentFingerId)
                {
                    Vector3 touchPosition = touches[i].position;
                    if (Vector2.Distance(background.position, touchPosition) > radius) touchPosition = (touchPosition - background.position).normalized * radius + background.position;
                    joystick.position = touchPosition;
                }
            }
        }
    }

    /// <summary>
    /// 得到这个虚拟摇杆的轴偏移
    /// </summary>
    /// <returns>一个代表轴偏移的2维矢量，长度最大为1</returns>
    public Vector2 GetAxis()
    {
        if (currentFingerId == int.MinValue)
            return Vector2.zero;
        else
            return ((Vector2)joystick.position - (Vector2)background.position) / radius;
    }
}