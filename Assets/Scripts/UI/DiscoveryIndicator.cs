using UnityEngine;
using UnityEngine.UI;

public class DiscoveryIndicator : MonoBehaviour {
    public IndicateMode indicateMode; // 指示模式

    private Image indicatorImage; // 指示器图片

    private void Start() {
        indicatorImage = GetComponent<Image>();
    }
    
    // 根据网络发现器的状态改变指示器图片颜色
    void Update() {
        switch (indicateMode) {
            case IndicateMode.Clinet:
                if (MyNetworkDiscovery.singleton.isClient)
                    indicatorImage.color = Color.blue; // 正在收听广播
                else
                    indicatorImage.color = Color.gray;
                break;
            case IndicateMode.Server:
                if (MyNetworkDiscovery.singleton.isServer)
                    indicatorImage.color = Color.blue; // 正在广播
                else
                    indicatorImage.color = Color.gray;
                break;
        }
    }
}

public enum IndicateMode { Clinet, Server }
