using UnityEngine;
using UnityEngine.SceneManagement;

namespace Offline
{
    public class SinglePlayerPanel : MonoBehaviour
    {
        public void Return()
        {
            Destroy(SinglePlayerManager.singleton.gameObject);

            SceneManager.LoadScene("Main Menu");
        }
    }
}