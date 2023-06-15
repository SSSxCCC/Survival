using UnityEngine;
using UnityEngine.EventSystems;

public class SelectOnInput : MonoBehaviour {
    public GameObject selectedObject; // 当用户按下移动按钮且没有UI对象被选中时，会被选中的UI对象

    void Update() {
        if (Input.GetAxisRaw("Vertical") != 0 && (EventSystem.current.currentSelectedGameObject == null || !EventSystem.current.currentSelectedGameObject.activeInHierarchy)) {
            EventSystem.current.SetSelectedGameObject(selectedObject);
        }
    }
}