using System;
using UnityEngine;

public class CommonVehicle<T> where T : MonoBehaviour, IVehicle
{
    private T vehicle;

    public CommonVehicle(T vehicle)
    {
        this.vehicle = vehicle;
    }

    public void OnEnter(GameObject player)
    {
        Physics2D.IgnoreCollision(vehicle.GetComponent<Collider2D>(), player.GetComponent<Collider2D>());
    }

    public void OnLeave(GameObject player)
    {
        Physics2D.IgnoreCollision(vehicle.GetComponent<Collider2D>(), player.GetComponent<Collider2D>(), false);
    }
}

[Serializable]
public class VehicleWeaponState
{
    public Sprite sprite;
    public Color color;
    public int numAmmo;

    public VehicleWeaponState(Sprite sprite, Color color, int numAmmo)
    {
        this.sprite = sprite;
        this.color = color;
        this.numAmmo = numAmmo;
    }
}