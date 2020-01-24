using System;
using System.Collections.Generic;
using UnityEngine;

namespace Offline
{
    public class PlayerSaver : ObjectSaver
    {
        public override void LoadStringData(string stringData)
        {
            PlayerData playerData = JsonUtility.FromJson<PlayerData>(stringData);

            IState state = GetComponent<IState>();
            state.layer = playerData.layer;

            vehicleIndex = playerData.vehicleIndex;
        }

        private int vehicleIndex;

        public override void LoadMoreData(List<GameObject> loadedObjectList)
        {
            if (vehicleIndex >= 0)
            {
                GetComponent<IPlayerController>().vehicle = loadedObjectList[vehicleIndex];
                Physics2D.IgnoreCollision(GetComponent<Collider2D>(), loadedObjectList[vehicleIndex].GetComponent<Collider2D>());
            }
        }

        protected override string GetStringData()
        {
            GameObject vehicle = GetComponent<IPlayerController>().vehicle;
            int vehicleIndex = vehicle != null ? vehicle.GetComponent<ObjectSaver>().tempIndex : -1;
            PlayerData playerData = new PlayerData(gameObject.layer, vehicleIndex);
            return JsonUtility.ToJson(playerData);
        }

        [Serializable]
        private class PlayerData
        {
            public int layer;
            public int vehicleIndex;

            public PlayerData(int layer, int vehicleIndex)
            {
                this.layer = layer;
                this.vehicleIndex = vehicleIndex;
            }
        }
    }
}