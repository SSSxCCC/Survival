using UnityEngine;
using UnityEngine.UI;

namespace Online
{
    public class IpButton : MonoBehaviour
    {
        public Text ipText; // 显示ip的文本界面

        [HideInInspector] public float lastDiscoverTime; // 上一次被发现时间

        public void FillIpInHostInputField()
        {
            IpManager.singleton.hostInputField.text = ipText.text;
        }
    }
}