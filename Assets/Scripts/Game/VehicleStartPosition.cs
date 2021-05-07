using UnityEngine;

public class VehicleStartPosition : MonoBehaviour {
    private void Awake() {
        GameManager.RegisterVehicleStartPosition(transform);
    }

    private void OnDestroy() {
        GameManager.UnRegisterVehicleStartPosition(transform);
    }
}
