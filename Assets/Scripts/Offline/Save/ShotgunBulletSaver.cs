using System;
using System.Collections.Generic;
using UnityEngine;

namespace Offline
{
    public class ShotgunBulletSaver : ObjectSaver
    {
        public override void LoadStringData(string stringData)
        {
            ShotgunBulletData shotgunBulletData = JsonUtility.FromJson<ShotgunBulletData>(stringData);

            IShotgunBulletAttacker shotgunBulletAttacker = GetComponent<IShotgunBulletAttacker>();
            shotgunBulletAttacker.attack = shotgunBulletData.attack;

            ownerIndex = shotgunBulletData.ownerIndex;

            shotgunBulletAttacker.isHost = false;
            shotgunBulletAttacker.shotgunBulletId = shotgunBulletData.shotgunBulletId;

            Rigidbody2D rb2D = GetComponent<Rigidbody2D>();
            rb2D.velocity = shotgunBulletData.linearVelocity;
            rb2D.angularVelocity = shotgunBulletData.angularVelocity;
        }

        private int ownerIndex;

        public override void LoadMoreData(List<GameObject> loadedObjectList)
        {
            if (ownerIndex >= 0) GetComponent<IAmmoAttacker>().owner = loadedObjectList[ownerIndex];
        }

        protected override string GetStringData()
        {
            IShotgunBulletAttacker shotgunBulletAttacker = GetComponent<IShotgunBulletAttacker>();
            int ownerIndex = shotgunBulletAttacker.owner != null ? shotgunBulletAttacker.owner.GetComponent<ObjectSaver>().tempIndex : -1;
            Rigidbody2D rb2D = GetComponent<Rigidbody2D>();
            ShotgunBulletData shotgunBulletData = new ShotgunBulletData(shotgunBulletAttacker.attack, ownerIndex, shotgunBulletAttacker.shotgunBulletId, rb2D.velocity, rb2D.angularVelocity);
            return JsonUtility.ToJson(shotgunBulletData);
        }

        [Serializable]
        private class ShotgunBulletData
        {
            public int attack;
            public int ownerIndex;
            public int shotgunBulletId;
            public Vector2 linearVelocity;
            public float angularVelocity;

            public ShotgunBulletData(int attack, int ownerIndex, int shotgunBulletId, Vector2 linearVelocity, float angularVelocity)
            {
                this.attack = attack;
                this.ownerIndex = ownerIndex;
                this.shotgunBulletId = shotgunBulletId;
                this.linearVelocity = linearVelocity;
                this.angularVelocity = angularVelocity;
            }
        }
    }
}