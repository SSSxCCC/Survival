using UnityEngine;

public interface IVehicleWeapon : IShooter
{
    bool Fire(GameObject owner);
    void CreateAmmo(GameObject owner);
}
