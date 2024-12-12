#if MOREMOUNTAINS_TOPDOWNENGINE
using System.Collections;
using System.Collections.Generic;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using UnityEngine;

public class CoreCharacterRun : CharacterAbility
{
	[Header("Speed")]
	[SerializeField] private float runSpeed = 16f;

	protected PlayerInputActions _inputActions;
	protected const string _runningAnimationParameterName = "Running";
	protected int _runningAnimationParameter;
	protected bool _runningStarted = false;

	protected override void Start()
	{
		base.Start();
		_inputActions = new PlayerInputActions();
	}
	
	public override void ProcessAbility()
	{
		base.ProcessAbility();
		HandleInput();
		HandleRunningExit();
	}
	
	protected override void HandleInput()
	{
		_inputActions.Player.Run.Enable();
		if (_inputActions.Player.Run.IsPressed())
		{
			RunStart();
		}

		if (_runningStarted)
		{
			if (!_inputActions.Player.Run.IsPressed())
			{
				RunStop();
			}
		}
	}
	
	protected virtual void HandleRunningExit()
	{
		if (_condition.CurrentState != CharacterStates.CharacterConditions.Normal)
		{
			StopAbilityUsedSfx();
		}

		if (_movement.CurrentState == CharacterStates.MovementStates.Running && AbilityInProgressSfx != null &&
		    _abilityInProgressSfx == null)
		{
			PlayAbilityUsedSfx();
		}

		if (!_controller.Grounded
		    && (_condition.CurrentState == CharacterStates.CharacterConditions.Normal)
		    && (_movement.CurrentState == CharacterStates.MovementStates.Running))
		{
			_movement.ChangeState(CharacterStates.MovementStates.Falling);
			StopFeedbacks();
			StopSfx();
		}

		if ((Mathf.Abs(_controller.CurrentMovement.magnitude) < runSpeed / 10) &&
		    (_movement.CurrentState == CharacterStates.MovementStates.Running))
		{
			_movement.ChangeState(CharacterStates.MovementStates.Idle);
			StopFeedbacks();
			StopSfx();
		}

		if (!_controller.Grounded && _abilityInProgressSfx != null)
		{
			StopFeedbacks();
			StopSfx();
		}
	}

	public virtual void RunStart()
	{
		if (!AbilityAuthorized
		    || (!_controller.Grounded)
		    || (_condition.CurrentState !=
		        CharacterStates.CharacterConditions.Normal)
		    || (_movement.CurrentState != CharacterStates.MovementStates.Walking))
		{
			return;
		}
		
		if (_characterMovement != null)
		{
			_characterMovement.MovementSpeed = runSpeed;
		}

		if (_movement.CurrentState != CharacterStates.MovementStates.Running)
		{
			PlayAbilityStartSfx();
			PlayAbilityUsedSfx();
			PlayAbilityStartFeedbacks();
			_runningStarted = true;
		}

		_movement.ChangeState(CharacterStates.MovementStates.Running);
	}
	
	public virtual void RunStop()
	{
		if (_runningStarted)
		{
			if ((_characterMovement != null))
			{
				_characterMovement.ResetSpeed();
				_movement.ChangeState(CharacterStates.MovementStates.Idle);
			}

			StopFeedbacks();
			StopSfx();
			_runningStarted = false;
		}
	}
	
	protected virtual void StopFeedbacks()
	{
		if (_startFeedbackIsPlaying)
		{
			StopStartFeedbacks();
			PlayAbilityStopFeedbacks();
		}
	}
	
	protected virtual void StopSfx()
	{
		StopAbilityUsedSfx();
		PlayAbilityStopSfx();
	}

	protected override void OnDisable()
	{
		base.OnDisable();
	}

	protected override void InitializeAnimatorParameters()
	{
		RegisterAnimatorParameter(_runningAnimationParameterName, AnimatorControllerParameterType.Bool,
			out _runningAnimationParameter);
	}
	
	public override void UpdateAnimator()
	{
		MMAnimatorExtensions.UpdateAnimatorBool(_animator, _runningAnimationParameter,
			(_movement.CurrentState == CharacterStates.MovementStates.Running), _character._animatorParameters,
			_character.RunAnimatorSanityChecks);
	}
}
#endif