using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static NoiseHandler;

//Used to help track enemy states in the inspector.
public enum EnemyStateType
{
    Idle,
    Patrol,
    MoveToNoise,
    Knifed,
    Dead
}

//Controls he enemy, including holding variables for use by enemy states, and handling functions from outside scripts that need to be intelligently routed to states.
public class EnemyStateMachineController : MonoBehaviour
{
    //Navmesh variables for pathing to goals
    private NavMeshAgent _agent; //Navmesh agent that controls movement in scene.
    [SerializeField] private Vector3 _goal; //A goal that can be set to pass to the navmesh agent in certain states (like moving to a noise)
    [SerializeField] private List<Vector3> _patrolPoints; //A list of coordinates to move to when patroling.
    [SerializeField] private double _patrolWait = 2.0; //How long the enemy waits before moving to the next patrol point.

    //State Machine variables for controlling behaviour
    [SerializeField] private BaseState _currentState; //Used to see what state we are in when viewed in the inspector
    private EnemyStateFactory _states; //Used by states to switch to new states.
    [SerializeField] private EnemyStateType _defaultState = EnemyStateType.Idle; //Set in the inspector to change what state an enemy starts in when the scene starts.
    [SerializeField] private EnemyStateType _myState; //Used to see current state in the inspector.

    //OTHER
    private Transform _trans; //Our transform. Keeping a reference is slightly more CPU efficient than using this.transform
    [SerializeField] private List<Material> _skinMaterial; //A list of materials for states to switch to. Mostly used to debug in-progress behavior
    private Renderer _renderer; //Our randerer. Ued to switch materials.

    //Getters and Setters
    public BaseState CurrentState { get => _currentState; set => _currentState = value; }
    public NavMeshAgent Agent { get => _agent; }
    public Vector3 Goal { get => _goal; set => _goal = value; }
    public Transform Trans { get => _trans; }
    public List<Vector3> patrolPoints { get => _patrolPoints; set => _patrolPoints = value; }
    public double patrolWait { get => _patrolWait; set => _patrolWait = value; }
    public List<Material> SkinMaterial { get => _skinMaterial; set => _skinMaterial = value; }
    public Renderer Renderer { get => _renderer; }
    public EnemyStateType MyState { get => _myState; set => _myState = value; }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _states = new EnemyStateFactory(this);
        _trans = this.transform;
        _goal = _trans.position; //Default goal to where we are to stop movement in relevant states
        _renderer = this.GetComponent<Renderer>();
        _renderer.material = SkinMaterial[0]; //Start our renderer on the default material.

        //Switch immediately to our default state.
        switch (_defaultState)
        {
            case EnemyStateType.Idle:
                _currentState = _states.Idle();
                break;
            case EnemyStateType.Patrol:
                _currentState = _states.Patrol();
                break;
            case EnemyStateType.MoveToNoise:
                _currentState = _states.MoveToNoise();
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        _currentState.UpdateState(); //Run update logic based on what state we are in.
    }

    //Public function to allow other scripts to cause an enemy to hear a noise. Respects the enemy's state's ability to hear that noise.
    public void HearNoise(NoiseID id, Transform origin, double range)
    {
        if (_currentState is ICanHear listener)
        {
            listener.HearNoise(id, origin, range);
        }
        else
        {
            return;
        }
        
    }

    //Public function to backstab the enemy. Respects whether the enemy can currently be stabbed (if they are dying for example, they cannot be stabbed)
    public void getBackstabbed()
    {
        if(_currentState is ICanBeDamaged damaged)
        {
            damaged.getBackStabbed();
        } else
        {
            return;
        }
    }
}
