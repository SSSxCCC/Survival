using UnityEngine;

namespace Offline
{
	public class ObjectSpawn : MonoBehaviour
	{
        public GameObject prefab;
        
        private void Start()
        {
            if (GameManager.singleton.initialization)
            {
                Instantiate(prefab, transform.position, transform.rotation);
            }

            Destroy(gameObject);
        }
	}
}