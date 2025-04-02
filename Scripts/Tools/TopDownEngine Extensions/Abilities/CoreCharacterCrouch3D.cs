using System;
using System.Collections.Generic;
using System.Linq;
using Larje.Core.Tools.TopDownEngine;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using UnityEngine;
using UnityEngine.Serialization;

public class CoreCharacterCrouch3D : CharacterAbility
{
	[SerializeField, MMReadOnly] private bool inATunnel;
	
	[Header("Input")]
	[SerializeField] private InputModes inputMode = InputModes.Pressed;
	
	[Header("Crawl")]
	[SerializeField] private bool crawlAuthorized = true;
	[SerializeField] private float speedMultiplier = 0.25f;
	[SerializeField] private float speedMultiplierInterpolationSpeed = 10f;
	
	[Header("Crouching")]
	[SerializeField] private bool resizeColliderWhenCrouched = false;
	[SerializeField] private bool translateColliderOnCrouch = false;
	[SerializeField] private float crouchedColliderHeight = 1.25f;
	
	[Header("Offset")]
	[SerializeField] private List<GameObject> objectsToOffset;
	[SerializeField] private Vector3 offsetCrouch;
	[SerializeField] private Vector3 offsetCrawl;
	[SerializeField] private float offsetSpeed = 5f;
	
	private bool _crouching = false;
	private float _speedMultiplierCurrent = 1f;
	private CoreCharacterMovement _characterMove; 
	private CoreCharacterRun _characterRun;
	private List<Vector3> _objectsToOffsetOriginalPositions;
	private Dictionary<Func<bool>, Func<int>> _inputCrouch;
	
	protected const string _crouchingAnimationParameterName = "Crouching";
	protected const string _crawlingAnimationParameterName = "Crawling";
	protected int _crouchingAnimationParameter;
	protected int _crawlingAnimationParameter;

	public void AddInput(Func<bool> input, Func<int> priority)
	{
		_inputCrouch ??= new Dictionary<Func<bool>, Func<int>>();
		if (!_inputCrouch.ContainsKey(input))
		{
			_inputCrouch.Add(input, priority);
		}
	}

	public void RemoveInput(Func<bool> input)
	{
		_inputCrouch ??= new Dictionary<Func<bool>, Func<int>>();
		if (_inputCrouch.ContainsKey(input))
		{
			_inputCrouch.Remove(input);
		}
	}
	
	public override void ProcessAbility()
	{
		base.ProcessAbility();

		InterpolateMultiplier();
		HandleInput();
		DetermineState();

		if (inputMode != InputModes.Toggle)
		{
			CheckExitCrouch();
		}

		OffsetObjects();
	}
	
	public override void UpdateAnimator()
	{
		MMAnimatorExtensions.UpdateAnimatorBool(_animator, _crouchingAnimationParameter,
			(_movement.CurrentState == CharacterStates.MovementStates.Crouching), _character._animatorParameters,
			_character.RunAnimatorSanityChecks);
		MMAnimatorExtensions.UpdateAnimatorBool(_animator, _crawlingAnimationParameter,
			(_movement.CurrentState == CharacterStates.MovementStates.Crawling), _character._animatorParameters,
			_character.RunAnimatorSanityChecks);
	}

	protected override void Initialization()
	{
		base.Initialization();
		inATunnel = false;

		if (objectsToOffset.Count > 0)
		{
			_objectsToOffsetOriginalPositions = new List<Vector3>();
			foreach (GameObject go in objectsToOffset)
			{
				if (go != null)
				{
					_objectsToOffsetOriginalPositions.Add(go.transform.localPosition);
				}
			}
		}
		
		_characterMove = _character?.FindAbility<CoreCharacterMovement>();
		_characterMove.TryAddSpeedMultiplier(() => _speedMultiplierCurrent);
		
		_characterRun = _character?.FindAbility<CoreCharacterRun>();
	}

	protected virtual void InterpolateMultiplier()
	{
		float targetMultiplier = _crouching ? speedMultiplier : 1f;
		_speedMultiplierCurrent = Mathf.Lerp(_speedMultiplierCurrent, targetMultiplier,
			Time.deltaTime * speedMultiplierInterpolationSpeed);
	}

	protected override void HandleInput()
	{
		Func<bool> input = _inputCrouch
			.OrderByDescending(x => x.Value.Invoke()).First().Key;

		if (input != null)
		{
			switch (inputMode)
			{
				case InputModes.Pressed:
					if (input())
					{
						Crouch();
					}

					break;
				case InputModes.Toggle:
					if (input())
					{
						if (_crouching)
						{
							CheckForTunnel();
						}
						else
						{
							Crouch();
						}
					}

					break;
			}
		}
	}

	protected virtual void Crouch()
	{
		if (!AbilityAuthorized
		    || (_condition.CurrentState !=
		        CharacterStates.CharacterConditions.Normal)
		    || (!_controller.Grounded))
		{
			return;
		}
		
		if ((_movement.CurrentState != CharacterStates.MovementStates.Crouching) &&
		    (_movement.CurrentState != CharacterStates.MovementStates.Crawling))
		{
			if (_characterRun != null)
			{
				_characterRun.RunStop();
			}
			
			PlayAbilityStartFeedbacks();
			PlayAbilityStartSfx();
			PlayAbilityUsedSfx();
		}

		_crouching = true;
		
		_movement.ChangeState(CharacterStates.MovementStates.Crouching);
		if ((Mathf.Abs(_horizontalInput) > 0) && (crawlAuthorized))
		{
			_movement.ChangeState(CharacterStates.MovementStates.Crawling);
		}
		
		if (resizeColliderWhenCrouched)
		{
			_controller.ResizeColliderHeight(crouchedColliderHeight, translateColliderOnCrouch);
		}
	}

	protected virtual void OffsetObjects()
	{
		if (objectsToOffset.Count > 0)
		{
			for (int i = 0; i < objectsToOffset.Count; i++)
			{
				Vector3 newOffset = Vector3.zero;
				if (_movement.CurrentState == CharacterStates.MovementStates.Crouching)
				{
					newOffset = offsetCrouch;
				}

				if (_movement.CurrentState == CharacterStates.MovementStates.Crawling)
				{
					newOffset = offsetCrawl;
				}

				if (objectsToOffset[i] != null)
				{
					objectsToOffset[i].transform.localPosition = Vector3.Lerp(
						objectsToOffset[i].transform.localPosition, _objectsToOffsetOriginalPositions[i] + newOffset,
						Time.deltaTime * offsetSpeed);
				}
			}
		}
	}

	protected virtual void DetermineState()
	{
		if ((_movement.CurrentState == CharacterStates.MovementStates.Crouching) ||
		    (_movement.CurrentState == CharacterStates.MovementStates.Crawling))
		{
			if ((_controller.CurrentMovement.magnitude > 0) && (crawlAuthorized))
			{
				_movement.ChangeState(CharacterStates.MovementStates.Crawling);
			}
			else
			{
				_movement.ChangeState(CharacterStates.MovementStates.Crouching);
			}
		}
	}
	
	protected virtual void CheckExitCrouch()
	{
		// if we're currently grounded
		if ((_movement.CurrentState == CharacterStates.MovementStates.Crouching)
		    || (_movement.CurrentState == CharacterStates.MovementStates.Crawling)
		    || _crouching)
		{
			if ((!_controller.Grounded)
			    || ((_movement.CurrentState != CharacterStates.MovementStates.Crouching)
			        && (_movement.CurrentState != CharacterStates.MovementStates.Crawling)
			        && (_inputManager.CrouchButton.IsOff || _inputManager.CrouchButton.IsUp))
			    || ((_inputManager.CrouchButton.IsOff || _inputManager.CrouchButton.IsUp)))
			{
				CheckForTunnel();
			}
		}
	}

	protected virtual void CheckForTunnel()
	{
		inATunnel = !_controller.CanGoBackToOriginalSize();
		if (!inATunnel)
		{
			ExitCrouch();
		}
	}

	protected virtual void ExitCrouch()
	{
		_crouching = false;
		
		StopAbilityUsedSfx();
		PlayAbilityStopSfx();
		StopStartFeedbacks();
		PlayAbilityStopFeedbacks();
		
		if ((_movement.CurrentState == CharacterStates.MovementStates.Crawling) ||
		    (_movement.CurrentState == CharacterStates.MovementStates.Crouching))
		{
			_movement.ChangeState(CharacterStates.MovementStates.Idle);
		}

		_controller.ResetColliderSize();
	}

	protected override void InitializeAnimatorParameters()
	{
		RegisterAnimatorParameter(_crouchingAnimationParameterName, AnimatorControllerParameterType.Bool,
			out _crouchingAnimationParameter);
		RegisterAnimatorParameter(_crawlingAnimationParameterName, AnimatorControllerParameterType.Bool,
			out _crawlingAnimationParameter);
	}
	
	public enum InputModes
	{
		Pressed,
		Toggle
	}
}
