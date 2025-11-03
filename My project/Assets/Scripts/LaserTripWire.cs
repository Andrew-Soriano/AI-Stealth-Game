using Unity.VisualScripting;
using UnityEngine;

public class LaserTripWire : MonoBehaviour {
    [SerializeField]
    private double _noiseRange = 50; //The distance at which enemies can hear the alert
    private void OnTriggerEnter(Collider other)
    {
        NoiseHandler.InvokeNoise(NoiseHandler.NoiseID.Laser, this.transform, _noiseRange);
    }
}
