using UnityEngine;

namespace Offline
{
    public class Chapter2Manager : GameManager
    {
        public Area portalArea;
        public Mission2c1 mission1;

        private void Start()
        {
            if (initialization)
                mission1.Start();
        }

        public override void OnMissionComplete(Mission mission)
        {
            base.OnMissionComplete(mission);
            Invoke("FinishChapter", 1f);
        }
    }
}