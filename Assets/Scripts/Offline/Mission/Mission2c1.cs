using UnityEngine;

namespace Offline
{
    [CreateAssetMenu(menuName = "Mission/mission 2-1")]
    public class Mission2c1 : Mission
    {
        protected override void OnStart()
        {
            
        }

        protected override void Initialize()
        {
            ((Chapter2Manager)GameManager.singleton).portalArea.Enter += OnEnter;
        }

        protected override void OnComplete()
        {
            ((Chapter2Manager)GameManager.singleton).portalArea.Enter -= OnEnter;
        }

        private void OnEnter(Area area, Collider2D collision)
        {
            if (collision.gameObject.CompareTag("Player"))
                Complete();
        }

        protected override void SetMissionData(string missionData)
        {

        }

        protected override string GetMissionData()
        {
            return null;
        }
    }
}