using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Online
{
    public class IpManager : MonoBehaviour
    {
        public static IpManager singleton; // 单例

        public GameObject ipButtonPrefab; // 显示搜索到的主机的ip的按钮
        public InputField hostInputField; // 主机ip输入框

        private Dictionary<string, IpButton> ipButtons = new Dictionary<string, IpButton>(); // 保存所有写着ip的按钮的映射
        private List<string> outdatedIps = new List<string>(); // 过时的ip地址

        private void Awake()
        {
            singleton = this; // 单例
        }

        // 持续检查每一个ip地址看是否过时了
        private void Update()
        {
            outdatedIps.Clear();

            // 对于每一个已发现的主机ip地址，如果超过2次广播间隔还没有收到其广播，则认为此ip过时了，删除此按钮
            foreach (KeyValuePair<string, IpButton> item in ipButtons)
            {
                if (Time.time - item.Value.lastDiscoverTime > (float)MyNetworkDiscovery.singleton.broadcastInterval * 2 / 1000)
                {
                    Destroy(item.Value.gameObject);
                    outdatedIps.Add(item.Key);
                }
            }

            // 删除每一个过时的显示其ip的按钮的映射
            foreach (string outdatedIp in outdatedIps)
            {
                ipButtons.Remove(outdatedIp);
            }
        }

        // 增加一个显示主机ip按钮
        public void AddIpButton(string ip)
        {
            IpButton ipButton;
            if (!ipButtons.TryGetValue(ip, out ipButton)) // 如果原来没有就新建一个
            {
                ipButton = Instantiate(ipButtonPrefab, transform).GetComponent<IpButton>();
                ipButton.ipText.text = ip;
                ipButtons.Add(ip, ipButton);
            }
            ipButton.lastDiscoverTime = Time.time; // 更新上次发现时间
        }
    }
}