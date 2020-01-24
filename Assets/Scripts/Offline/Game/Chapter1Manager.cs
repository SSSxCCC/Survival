using UnityEngine;

namespace Offline
{
    public class Chapter1Manager : GameManager
    {
        public Mission1c1 mission1;

        private void Start()
        {
            if (initialization)
                mission1.Start();
        }

        public override void OnMissionComplete(Mission mission)
        {
            base.OnMissionComplete(mission);
            Invoke("FinishChapter", 3f);
        }
    }
}