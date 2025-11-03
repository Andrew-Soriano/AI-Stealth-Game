using UnityEngine;
using static NoiseHandler;
using static Unity.VisualScripting.Member;

public class AlertManager : MonoBehaviour
{
    public static AlertManager instance { get; private set; } //This object is a singletone, this is the instance
    [SerializeField] private LayerMask enemyLayer = 1<<6; //Set to enemies in inspector, causes unity functions to only interact with guards
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        //Set the singleton
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    //Subscribe to noise events
    private void OnEnable()
    {
        NoiseHandler.OnNoise += NoiseAlert;
    }

    private void OnDisable()
    {
        NoiseHandler.OnNoise -= NoiseAlert;
    }

    //Find enemy or enemies based on distance from noise source. Use id to decide how many to alert.
    private void NoiseAlert(NoiseID id, Transform pos, double radius)
    {
        //Cast in a sphere around noise position and gather all enemies.
        Collider[] hits = Physics.OverlapSphere(pos.position, (float)radius, enemyLayer);

        EnemyStateMachineController closestEnemy = null;
        float closestDist = float.MaxValue;

        //Find closest enemy (TODO: Execute different behavior based on ID instead of always closest enemy)
        foreach (var hit in hits)
        {
            EnemyStateMachineController enemy = hit.GetComponent<EnemyStateMachineController>();
            if (enemy == null)
                continue;

            float dist = Vector3.Distance(pos.position, enemy.transform.position);
            if (dist < closestDist)
            {
                closestDist = dist;
                closestEnemy = enemy;
            }
        }

        //Finding closest enemy, alert the enemy
        if (closestEnemy != null)
            closestEnemy.HearNoise(id, pos, radius);
    }
}
