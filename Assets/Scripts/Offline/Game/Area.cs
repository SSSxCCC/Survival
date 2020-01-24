using UnityEngine;

namespace Offline
{
    public class Area : MonoBehaviour
    {
        public event EnterEventHandler Enter;
        public delegate void EnterEventHandler(Area area, Collider2D collision);

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (Enter != null) Enter(this, collision);
        }
    }
}