using System.Collections.Generic;
using UnityEngine;

public class SingletonObject : MonoBehaviour {
    public static Dictionary<string, GameObject> singletonDict = new Dictionary<string, GameObject>();

    public string singletonKey = "";

    private void Awake() {
        if (singletonDict.ContainsKey(singletonKey)) {
            if (singletonDict[singletonKey] != gameObject)
                Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
        singletonDict[singletonKey] = gameObject;
    }
}
