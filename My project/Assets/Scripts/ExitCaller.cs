using UnityEngine;
using static KeyController;

public class ExitCaller : MonoBehaviour
{
    public delegate void PlayerExit();
    public static event PlayerExit OnPlayerExit;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //Player has gone through the exit
            OnPlayerExit?.Invoke();
        }
    }
}
