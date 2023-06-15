using UnityEngine;
using UnityEngine.EventSystems;

public class VirtualButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler {
    private bool m_Pressed = false; // 按钮是否被按住
    private int m_PressedFrame = -5; // 上次在游戏第几帧按下
    private int m_ReleasedFrame = -5; // 最后一次在游戏第几帧释放

    public void OnPointerDown(PointerEventData eventData) {
        Pressed();
    }

    public void OnPointerUp(PointerEventData eventData) {
        Released();
    }

    public void OnPointerExit(PointerEventData eventData) {
        Released();
    }

    // 按下时调用
    private void Pressed() {
        if (m_Pressed) return;
        
        m_Pressed = true;
        m_PressedFrame = Time.frameCount;
    }


    // 释放时调用
    private void Released() {
        if (!m_Pressed) return;

        m_Pressed = false;
        m_ReleasedFrame = Time.frameCount;
    }

    /// <summary>
    /// 此按钮是否被按住
    /// </summary>
    public bool GetButton() {
        return m_Pressed;
    }

    /// <summary>
    /// 此按钮是否在此帧被按下
    /// </summary>
    public bool GetButtonDown() {
        return m_PressedFrame == Time.frameCount -1;
    }

    /// <summary>
    /// 此按钮是否在此帧被释放
    /// </summary>
    public bool GetButtonUp() {
        return m_ReleasedFrame == Time.frameCount - 1;
    }
}