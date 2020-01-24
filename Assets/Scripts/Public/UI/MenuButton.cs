using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 此脚本放在游戏内菜单按钮上，使得esc按键可以打开游戏内菜单
/// </summary>
[RequireComponent(typeof(Button))]
public class MenuButton : MonoBehaviour
{
    private Button button;
    
	void Start()
    {
        button = GetComponent<Button>();
	}
	
	void Update()
    {
		if (Input.GetButtonDown("Cancel"))
        {
            button.onClick.Invoke();
        }
	}
}
