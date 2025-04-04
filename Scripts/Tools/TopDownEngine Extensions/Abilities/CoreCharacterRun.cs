#if MOREMOUNTAINS_TOPDOWNENGINE
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Larje.Core;
using Larje.Core.Tools.TopDownEngine;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using UnityEditor.Timeline.Actions;
using UnityEngine;
using UnityEngine.InputSystem;
using PlayerActions = InputSystem_Actions.PlayerActions;

public class CoreCharacterRun : CharacterAbility
{
	[Header("Speed")]
	[SerializeField] private float speedMultiplier = 2f;
	[SerializeField] private float lerpSpeedEnter = 10f;
	[SerializeField] private float lerpSpeedExit = 10f;

	private bool _runningStarted = false;
	private bool _lastFrameInput;
	private float _speedMultiplierCurrent = 1f;
	private CoreCharacterMovement _characterMove;
	private Dictionary<Func<bool>, Func<int>> _inputRunning;
	
	protected const string _runningAnimationParameterName = "Running";
	protected int _runningAnimationParameter;
	
	public event Action EventRunStart; 
	
	public void AddInput(Func<bool> input, Func<int> priority)
	{
		_inputRunning ??= new Dictionary<Func<bool>, Func<int>>();

		if (!_inputRunning.ContainsKey(input))
		{
			_inputRunning.Add(input, priority);
		}
	}

	public void RemoveInput(Func<bool> input)
	{
		if (_inputRunning.ContainsKey(input))
		{
			_inputRunning.Remove(input);
		}
	}
	
	public override void UpdateAnimator()
	{
		MMAnimatorExtensions.UpdateAnimatorBool(_animator, _runningAnimationParameter,
			(_movement.CurrentState == CharacterStates.MovementStates.Running), _character._animatorParameters,
			_character.RunAnimatorSanityChecks);
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
			if ((_characterMove != null))
			{
				_characterMove.ResetSpeed();
				_movement.ChangeState(CharacterStates.MovementStates.Idle);
			}

			StopFeedbacks();
			StopSfx();
			_runningStarted = false;
		}
	}
	
	public override void ProcessAbility()
	{
		base.ProcessAbility();
		InterpolateMultiplier();
		HandleInput();
		HandleRunningExit();
	}

	public virtual void ResetMultiplier()
	{
		_speedMultiplierCurrent = 1f;
	}

	protected override void Initialization()
	{
		base.Initialization();
		
		_characterMove = _character?.FindAbility<CoreCharacterMovement>();
		_characterMove.TryAddSpeedMultiplier(() => _speedMultiplierCurrent);
	}

	protected void InterpolateMultiplier()
	{
		float targetMultiplier = _runningStarted ? speedMultiplier : 1f;
		float lerpSpeed = _runningStarted ? lerpSpeedEnter : lerpSpeedExit;

		_speedMultiplierCurrent = Mathf.Lerp(_speedMultiplierCurrent, targetMultiplier,
			Time.deltaTime * lerpSpeed);
	}

	protected override void HandleInput()
	{
		if (_inputRunning == null)
		{
			return;
		}
		
		Func<bool> input = _inputRunning
			.OrderByDescending(x => x.Value.Invoke()).First().Key;

		if (input != null)
		{
			bool inputValue = input();
			
			if (inputValue)
			{
				if (!_lastFrameInput)
				{
					EventRunStart?.Invoke();
				}
				RunStart();
			}

			if (_runningStarted && !inputValue)
			{
				RunStop();
			}
			
			_lastFrameInput = input();
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

		/*if ((Mathf.Abs(_controller.CurrentMovement.magnitude) < runSpeed / 10) &&
		    (_movement.CurrentState == CharacterStates.MovementStates.Running))
		{
			_movement.ChangeState(CharacterStates.MovementStates.Idle);
			StopFeedbacks();
			StopSfx();
		}*/

		if (!_controller.Grounded && _abilityInProgressSfx != null)
		{
			StopFeedbacks();
			StopSfx();
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

	protected override void InitializeAnimatorParameters()
	{
		RegisterAnimatorParameter(_runningAnimationParameterName, AnimatorControllerParameterType.Bool,
			out _runningAnimationParameter);
	}
}
#endif