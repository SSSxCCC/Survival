using System;
using System.Collections.Generic;
using UnityEngine;

namespace Offline
{
    public class LaserSaver : ObjectSaver
    {
        public override void LoadStringData(string stringData)
        {
            LaserData laserData = JsonUtility.FromJson<LaserData>(stringData);

            IAmmoAttacker laserAttacker = GetComponent<IAmmoAttacker>();
            laserAttacker.attack = laserData.attack;

            ownerIndex = laserData.ownerIndex;

            DestroyByTime destroyByTime = GetComponent<DestroyByTime>();
            destroyByTime.lifeTime = laserData.remainingTime;
        }

        private int ownerIndex;

        public override void LoadMoreData(List<GameObject> loadedObjectList)
        {
            if (ownerIndex >= 0) GetComponent<IAmmoAttacker>().owner = loadedObjectList[ownerIndex];
        }

        protected override string GetStringData()
        {
            IAmmoAttacker laserAttacker = GetComponent<IAmmoAttacker>();
            int ownerIndex = laserAttacker.owner != null ? laserAttacker.owner.GetComponent<ObjectSaver>().tempIndex : -1;
            DestroyByTime destroyByTime = GetComponent<DestroyByTime>();
            LaserData laserData = new LaserData(laserAttacker.attack, ownerIndex, destroyByTime.remainingTime);
            return JsonUtility.ToJson(laserData);
        }

        [Serializable]
        private class LaserData
        {
            public int attack;
            public int ownerIndex;
            public float remainingTime;

            public LaserData(int attack, int ownerIndex, float remainingTime)
            {
                this.attack = attack;
                this.ownerIndex = ownerIndex;
                this.remainingTime = remainingTime;
            }
        }
    }
}