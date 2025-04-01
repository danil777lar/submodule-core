using System;
using System.Collections.Generic;
using System.Linq;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using UnityEngine;

public class CoreCharacterCrouch3D : CharacterAbility
{
	[MMReadOnly] public bool ForcedCrouch = false;
	[Header("Input")]
	public InputModes InputMode = InputModes.Pressed;
	[Header("Crawl")]
	public bool CrawlAuthorized = true;
	public float CrawlSpeed = 4f;
	[Space(10)]
	[Header("Crouching")]
	public bool ResizeColliderWhenCrouched = false;
	[MMCondition("ResizeColliderWhenCrouched", true)] public bool TranslateColliderOnCrouch = false;
	public float CrouchedColliderHeight = 1.25f;
	[Space(10)]
	[Header("Offset")]
	public List<GameObject> ObjectsToOffset;
	public Vector3 OffsetCrouch;
	public Vector3 OffsetCrawl;
	public float OffsetSpeed = 5f;
	[MMReadOnly] public bool InATunnel;

	protected List<Vector3> _objectsToOffsetOriginalPositions;
	protected const string _crouchingAnimationParameterName = "Crouching";
	protected const string _crawlingAnimationParameterName = "Crawling";
	protected int _crouchingAnimationParameter;
	protected int _crawlingAnimationParameter;
	protected bool _crouching = false;

	private Dictionary<Func<bool>, Func<int>> _inputCrouch;

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

	protected override void Initialization()
	{
		base.Initialization();
		InATunnel = false;

		// we store our objects to offset's initial positions
		if (ObjectsToOffset.Count > 0)
		{
			_objectsToOffsetOriginalPositions = new List<Vector3>();
			foreach (GameObject go in ObjectsToOffset)
			{
				if (go != null)
				{
					_objectsToOffsetOriginalPositions.Add(go.transform.localPosition);
				}
			}
		}
	}

	public override void ProcessAbility()
	{
		base.ProcessAbility();

		HandleInput();
		HandleForcedCrouch();
		DetermineState();

		if (InputMode != InputModes.Toggle)
		{
			CheckExitCrouch();
		}

		OffsetObjects();
	}

	protected virtual void HandleForcedCrouch()
	{
		if (ForcedCrouch && (_movement.CurrentState != CharacterStates.MovementStates.Crouching) &&
		    (_movement.CurrentState != CharacterStates.MovementStates.Crawling))
		{
			Crouch();
		}
	}

	protected override void HandleInput()
	{
		Func<bool> input = _inputCrouch
			.OrderByDescending(x => x.Value.Invoke()).First().Key;

		if (input != null)
		{
			switch (InputMode)
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

	public virtual void StartForcedCrouch()
	{
		ForcedCrouch = true;
		_crouching = true;
	}

	public virtual void StopForcedCrouch()
	{
		ForcedCrouch = false;
		_crouching = false;
	}

	protected virtual void Crouch()
	{
		if (!AbilityAuthorized // if the ability is not permitted
		    || (_condition.CurrentState !=
		        CharacterStates.CharacterConditions.Normal) // or if we're not in our normal stance
		    || (!_controller.Grounded)) // or if we're grounded
			// we do nothing and exit
		{
			return;
		}

		// if this is the first time we're here, we trigger our sounds
		if ((_movement.CurrentState != CharacterStates.MovementStates.Crouching) &&
		    (_movement.CurrentState != CharacterStates.MovementStates.Crawling))
		{
			// we play the crouch start sound 
			PlayAbilityStartFeedbacks();
			PlayAbilityStartSfx();
			PlayAbilityUsedSfx();
		}

		_crouching = true;

		// we set the character's state to Crouching and if it's also moving we set it to Crawling
		_movement.ChangeState(CharacterStates.MovementStates.Crouching);
		if ((Mathf.Abs(_horizontalInput) > 0) && (CrawlAuthorized))
		{
			_movement.ChangeState(CharacterStates.MovementStates.Crawling);
		}

		// we resize our collider to match the new shape of our character (it's usually smaller when crouched)
		if (ResizeColliderWhenCrouched)
		{
			_controller.ResizeColliderHeight(CrouchedColliderHeight, TranslateColliderOnCrouch);
		}

		// we change our character's speed
		if (_characterMovement != null)
		{
			_characterMovement.MovementSpeed = CrawlSpeed;
		}

		// we prevent movement if we can't crawl
		if (!CrawlAuthorized)
		{
			_characterMovement.MovementSpeed = 0f;
		}
	}

	protected virtual void OffsetObjects()
	{
		// we move all the objects we want to move
		if (ObjectsToOffset.Count > 0)
		{
			for (int i = 0; i < ObjectsToOffset.Count; i++)
			{
				Vector3 newOffset = Vector3.zero;
				if (_movement.CurrentState == CharacterStates.MovementStates.Crouching)
				{
					newOffset = OffsetCrouch;
				}

				if (_movement.CurrentState == CharacterStates.MovementStates.Crawling)
				{
					newOffset = OffsetCrawl;
				}

				if (ObjectsToOffset[i] != null)
				{
					ObjectsToOffset[i].transform.localPosition = Vector3.Lerp(
						ObjectsToOffset[i].transform.localPosition, _objectsToOffsetOriginalPositions[i] + newOffset,
						Time.deltaTime * OffsetSpeed);
				}
			}
		}
	}

	protected virtual void DetermineState()
	{
		if ((_movement.CurrentState == CharacterStates.MovementStates.Crouching) ||
		    (_movement.CurrentState == CharacterStates.MovementStates.Crawling))
		{
			if ((_controller.CurrentMovement.magnitude > 0) && (CrawlAuthorized))
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
			if (_inputManager == null)
			{
				if (!ForcedCrouch)
				{
					ExitCrouch();
				}

				return;
			}

			// but we're not pressing down anymore, or we're not grounded anymore
			if ((!_controller.Grounded)
			    || ((_movement.CurrentState != CharacterStates.MovementStates.Crouching)
			        && (_movement.CurrentState != CharacterStates.MovementStates.Crawling)
			        && (_inputManager.CrouchButton.IsOff || _inputManager.CrouchButton.IsUp) && (!ForcedCrouch))
			    || ((_inputManager.CrouchButton.IsOff || _inputManager.CrouchButton.IsUp) && (!ForcedCrouch)))
			{
				CheckForTunnel();
			}
		}
	}

	protected virtual void CheckForTunnel()
	{
		// we cast a raycast above to see if we have room enough to go back to normal size
		InATunnel = !_controller.CanGoBackToOriginalSize();

		// if the character is not in a tunnel, we can go back to normal size
		if (!InATunnel)
		{
			ExitCrouch();
		}
	}

	protected virtual void ExitCrouch()
	{
		_crouching = false;

		// we return to normal walking speed
		if (_characterMovement != null)
		{
			_characterMovement.ResetSpeed();
		}

		// we play our exit sound
		StopAbilityUsedSfx();
		PlayAbilityStopSfx();
		StopStartFeedbacks();
		PlayAbilityStopFeedbacks();

		// we go back to Idle state and reset our collider's size
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

	public override void UpdateAnimator()
	{
		MMAnimatorExtensions.UpdateAnimatorBool(_animator, _crouchingAnimationParameter,
			(_movement.CurrentState == CharacterStates.MovementStates.Crouching), _character._animatorParameters,
			_character.RunAnimatorSanityChecks);
		MMAnimatorExtensions.UpdateAnimatorBool(_animator, _crawlingAnimationParameter,
			(_movement.CurrentState == CharacterStates.MovementStates.Crawling), _character._animatorParameters,
			_character.RunAnimatorSanityChecks);
	}
	
	public enum InputModes
	{
		Pressed,
		Toggle
	}
}
