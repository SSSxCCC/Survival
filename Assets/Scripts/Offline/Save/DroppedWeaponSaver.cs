using System.Collections.Generic;
using UnityEngine;

namespace Offline
{
    public class DroppedWeaponSaver : ObjectSaver
    {
        public override void LoadStringData(string stringData)
        {
            GetComponent<IDroppedWeapon>().numAmmo = int.Parse(stringData);
        }

        public override void LoadMoreData(List<GameObject> loadedObjectList) { }

        protected override string GetStringData()
        {
            return GetComponent<IDroppedWeapon>().numAmmo.ToString();
        }
    }
}