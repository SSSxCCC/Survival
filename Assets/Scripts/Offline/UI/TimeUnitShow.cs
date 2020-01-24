using UnityEngine;

namespace Offline
{
    public class TimeUnitShow : MonoBehaviour
    {
        public GameObject hourObject;
        public GameObject minuteObject;
        public GameObject secondObject;

        public void Hour()
        {
            hourObject.SetActive(true);
            minuteObject.SetActive(false);
            secondObject.SetActive(false);
        }

        public void Minute()
        {
            hourObject.SetActive(false);
            minuteObject.SetActive(true);
            secondObject.SetActive(false);
        }

        public void Second()
        {
            hourObject.SetActive(false);
            minuteObject.SetActive(false);
            secondObject.SetActive(true);
        }
    }
}