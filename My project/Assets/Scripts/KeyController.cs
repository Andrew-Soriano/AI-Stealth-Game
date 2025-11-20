using UnityEngine;

public class KeyController : MonoBehaviour
{
    public delegate void KeyPickup(int id);
    public static event KeyPickup OnKeyPickup;

    [SerializeField] private int _keyID;

    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Player"))
        {
            Debug.Log($"Key {_keyID} triggered by {other.name}");

            //Open doors with matching ID
            OnKeyPickup?.Invoke(_keyID);

            //Remove Key from world
            Destroy(gameObject);
        }
    }
}
