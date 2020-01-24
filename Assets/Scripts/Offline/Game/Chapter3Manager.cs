using UnityEngine;

namespace Offline
{
    public class Chapter3Manager : GameManager
    {
        public Mission3c1 mission1;

        private void Start()
        {
            if (initialization)
                mission1.Start();
        }
    }
}