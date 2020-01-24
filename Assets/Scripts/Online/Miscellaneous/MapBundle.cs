using System.Collections.Generic;
using UnityEngine;

namespace Online
{
    [CreateAssetMenu(menuName = "Map Bundle")]
    public class MapBundle : ScriptableObject
    {
        public GameObject[] mapPrefabs;

        Dictionary<string, GameObject> m_NameMapDict;
        public Dictionary<string, GameObject> nameMapDict
        {
            get
            {
                if (m_NameMapDict == null)
                {
                    m_NameMapDict = new Dictionary<string, GameObject>();
                    foreach (GameObject mapPrefab in mapPrefabs)
                    {
                        m_NameMapDict[mapPrefab.GetComponent<MapInfo>().name] = mapPrefab;
                    }
                }
                return m_NameMapDict;
            }
        }
    }
}