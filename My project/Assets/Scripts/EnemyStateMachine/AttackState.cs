using UnityEngine;

//State for shoot at the player. Enemy stops moving, aims, and shoots. Accuracy increases over time.
public class AttackState : BaseState
{
    private bool _shooting = true; //Are we in the middle of taking a shot (multi-frame dependency)
    private float _attackTimer = 0; //Time between shots/taking shots
    private float _attackAccuracy; //Chance to hit on each shot. Goes up every time we shoot. Resets on entering state.
    public AttackState(EnemyStateMachineController controller, EnemyStateFactory factory) : base(controller, factory)
    {
    }

    public override void CheckSwitchState()
    {
        //If the player is too far away and we are not in the middle of shooting, go back to pursuit
        if(!_shooting &&
            Vector3.Distance(_controller.Trans.position, _controller.Player.position) < _controller.AttackRange)
        {
            //Switch to pursuit. Let the pursuit state decide when to go back to default
            this.SwitchState(_factory.PursuitState());
        }
    }

    public override void EnterState()
    {
        _attackAccuracy = _controller.BaseAccuracy; //Reset to base accuracy every time we enter state.
        _controller.Agent.isStopped = true; //Stop moving
        _controller.Renderer.material = _controller.SkinMaterial[4];
    }

    public override void ExitState()
    {
        _controller.Agent.isStopped = false; //Allow the agent to move again when they leave attacking state
    }

    public override void InitializeSubState()
    {
        throw new System.NotImplementedException();
    }

    public override void UpdateState()
    {
        //If we are in the middle of shooting, handle shooting
        if (_shooting)
        {
            if( Random.value <= _attackAccuracy)
            {
                    _controller.PlayerController.Damage(_controller.DamageValue); //Do damage
            }
            
            _attackTimer = 0; //Reset Attack timer
            _attackAccuracy += 0.25f; //Increment accuracy so shots are more likely to hit next time
            if(_attackAccuracy > 1.0f)
            {
                _attackAccuracy = 1.0f; //Accuracy cannot be greater than 100%
            }
        } else {
            _attackTimer += Time.deltaTime;
            if (_attackTimer > _controller.AttackDelay) { _shooting = true; } //Time to shoot!
        }

        CheckSwitchState();
    }
}
