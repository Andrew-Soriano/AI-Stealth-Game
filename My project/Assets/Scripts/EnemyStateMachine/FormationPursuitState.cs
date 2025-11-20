using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

//State for chasing the player
public class FormationPursuitState : BaseState
{

    private Transform _leader;        // The enemy we are following in formation
    private NavMeshAgent _leaderNav;  //Cache for the leader's navmesh agent
    private EnemyStateMachineController _leaderController; //cache of enemy's state controller
    private Vector3 _formationOffset; // Offset relative to leader

    public FormationPursuitState(EnemyStateMachineController controller, EnemyStateFactory factory,
                                 Transform leader, Vector3 formationOffset) : base(controller, factory)
    {
        this._leader = leader;
        this._formationOffset = formationOffset;
    }

    public override void CheckSwitchState()
    {
        //If we are close enough, go on the offensive
        if (_controller.playerVision() &&
           Vector3.Distance(_controller.Trans.position, _controller.Player.position) < _controller.AttackRange)
        {
            this.SwitchState(_factory.FormationAttackState(_leader, _formationOffset));
        }
        //If our leader stops without us switching to an attack state, the player is not there anymore
        else if (_leaderController.CurrentState is not SoloPursuitState &&
            _leaderController.CurrentState is not SoloAttackState &&
            !_controller.playerVision())
        {
            this.SwitchState(_controller.DefaultState); //Switch back to the default state, end pursuit
        }
    }

    public override void EnterState()
    {
        //For debug, go to new material
        _controller.Renderer.material = _controller.SkinMaterial[3];
        _leaderNav = _leader.GetComponent<NavMeshAgent>();
        _leaderController = _leader.GetComponent<EnemyStateMachineController>();

        //Update position to follow leader
        UpdateState();
    }

    public override void ExitState()
    {
        //Player lost, return to default goal on exit
        _controller.Goal = _controller.Trans.position;
    }

    public override void InitializeSubState()
    {
        throw new System.NotImplementedException();
    }

    public override void UpdateState()
    {
        //Update goal according to follow leader
        Vector3 offset = _leader.position + _formationOffset;

        // Make sure the target is on the NavMesh
        NavMeshHit hit;
        if (NavMesh.SamplePosition(offset, out hit, 2.0f, NavMesh.AllAreas))
        {
            _controller.Goal = hit.position;
            _controller.Agent.SetDestination(_controller.Goal);
        }

        //Check if we should switch (such as if we are close enough to attack)
        CheckSwitchState();
        Debug.DrawLine(_controller.Trans.position, _controller.Goal, Color.green);
    }
}
