using UnityEngine;

/// <summary>
/// 镜头控制脚本，使镜头跟随玩家单位，并处理镜头缩放。
/// </summary>
public class CameraController : MonoBehaviour {

    public float minSize = 5f; // 镜头最小尺寸
    public float maxSize = 10f; // 镜头最大尺寸
    public float mouseSensitivity = 10f; // 鼠标滚轮缩放速度
    public float touchSensitivity = 0.1f; // 触屏缩放速度

    [HideInInspector] public GameObject localPlayer; // 玩家对象，在PlayerController里面赋值

    private void Update() {
        Zoom(); // 缩放
        FollowPlayer(); // 跟随玩家

        // 限制范围
        if (MapBorder.singleton == null) return;
        float ylimit = Mathf.Max(MapBorder.singleton.mapHalfHeight - Camera.main.orthographicSize, 0);
        float xlimit = Mathf.Max(MapBorder.singleton.mapHalfWidth - Camera.main.orthographicSize * Camera.main.pixelWidth / Camera.main.pixelHeight, 0);
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, -xlimit, xlimit), Mathf.Clamp(transform.position.y, -ylimit, ylimit), transform.position.z);
    }

    // 镜头缩放
    private void Zoom() {
        // 鼠标滚轮缩放
        float size = Camera.main.orthographicSize;
        size -= Input.GetAxis("Mouse ScrollWheel") * mouseSensitivity;
        size = Mathf.Clamp(size, minSize, maxSize);
        Camera.main.orthographicSize = size;

        // 触屏缩放
        if (ShouldTouchZoom()) {
            // 得到这两点上一帧的位置
            Vector2 touchZeroPrevPos = Input.GetTouch(0).position - Input.GetTouch(0).deltaPosition;
            Vector2 touchOnePrevPos = Input.GetTouch(1).position - Input.GetTouch(1).deltaPosition;

            // 分别得到这两点在这2帧的距离，并计算距离差
            float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float touchDeltaMag = (Input.GetTouch(0).position - Input.GetTouch(1).position).magnitude;
            float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

            // 根据距离差产生缩放
            Camera.main.orthographicSize += deltaMagnitudeDiff * touchSensitivity;
            Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize, minSize, maxSize);
        }
    }

    // 判断当前触屏情况是否可以缩放
    private bool ShouldTouchZoom() {
        // 有2个触点
        if (Input.touchCount == 2) {
            // 分别得到2个触点的位置
            Vector2 touchZero = Input.GetTouch(0).position;
            Vector2 touchOne = Input.GetTouch(1).position;

            // 一个在左上角一个在右上角
            if (touchZero.y > Screen.height / 2 && touchOne.y > Screen.height / 2 &&
                ((touchZero.x < Screen.width / 2 && touchOne.x > Screen.width / 2) ||
                (touchOne.x < Screen.width / 2 && touchZero.x > Screen.width / 2))) {
                return true;
            }
        }

        return false;
    }

    // 镜头跟着玩家
    private void FollowPlayer() {
        if (localPlayer == null) {
            return;
        }

        Vector2 position;
        GameObject vehicle;
        if ((vehicle = localPlayer.GetComponent<PlayerController>().vehicle) == null) {
            position = localPlayer.transform.position;
        } else {
            position = vehicle.transform.position;
        }
        transform.position = new Vector3(position.x, position.y, -10);
    }
}