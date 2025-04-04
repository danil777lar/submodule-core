#if MOREMOUNTAINS_TOPDOWNENGINE

using System;
using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using UnityEngine;
using UnityEngine.Serialization;

namespace Larje.Core.Tools.TopDownEngine
{
	public class CoreCharacterJump3D : CharacterAbility
	{
		[Header("Jump Settings")] 
		[SerializeField] private bool jumpProportionalToPress = true;
		[SerializeField] private bool blockControlOnJump;
		[SerializeField] private float minimumPressTime = 0.4f;
		[SerializeField] private float jumpForce = 800f;
		[SerializeField] private float jumpHeight = 4f;
		[SerializeField] private AnimationCurve jumpForceCurve = AnimationCurve.Constant(0f, 1f, 1f);
		
		[Header("Slopes")] 
		[SerializeField] private bool canJumpOnTooSteepSlopes = true;
		[SerializeField] private bool resetJumpsOnTooSteepSlopes = false;
		
		[Header("Number of Jumps")] 
		[SerializeField] private int numberOfJumps = 1;
		[SerializeField, MMReadOnly] public int numberOfJumpsLeft = 0;
		
		[Header("Feedbacks")] 
		[SerializeField] private MMFeedbacks jumpStartFeedback;
		[SerializeField] private MMFeedbacks jumpStopFeedback;
		
		protected Vector3 _jumpForce;
		protected Vector3 _jumpOrigin;
		protected CharacterCrouch _characterCrouch;
		protected CoreCharacterMovement _characterMove;
		protected CharacterButtonActivation _characterButtonActivation;
		protected bool _doubleJumping;
		protected bool _isInJump;
		protected bool _jumpStopped = false;
		protected bool _buttonReleased = false;
		protected int _initialNumberOfJumps;
		protected float _jumpStartedAt = 0f;

		protected const string _jumpingAnimationParameterName = "Jumping";
		protected const string _doubleJumpingAnimationParameterName = "DoubleJumping";
		protected const string _hitTheGroundAnimationParameterName = "HitTheGround";
		protected int _jumpingAnimationParameter;
		protected int _doubleJumpingAnimationParameter;
		protected int _hitTheGroundAnimationParameter;
		
		public bool IsInJump => _isInJump;
		
		public event Action EventJump;
		public event Action EventLanding;

		public void InputJumpStart()
		{
			if (AbilityAuthorized)
			{
				JumpStart();
			}
		}
		
		public void InputJumpStop()
		{
			if (AbilityAuthorized)
			{
				_buttonReleased = true;
			}
		}
		
		public virtual void JumpStart()
		{
			if (!EvaluateJumpConditions())
			{
				return;
			}
			
			if (numberOfJumpsLeft != numberOfJumps)
			{
				_doubleJumping = true;
			}

			numberOfJumpsLeft = numberOfJumpsLeft - 1;

			_isInJump = true;
			_movement.ChangeState(CharacterStates.MovementStates.Jumping);
			MMCharacterEvent.Trigger(_character, MMCharacterEventTypes.Jump);
			jumpStartFeedback?.PlayFeedbacks(this.transform.position);
			_jumpOrigin = this.transform.position;
			_jumpStopped = false;
			_jumpStartedAt = Time.time;
			_controller.Grounded = false;
			_controller.GravityActive = false;
			_buttonReleased = false;

			PlayAbilityStartSfx();
			PlayAbilityUsedSfx();
			PlayAbilityStartFeedbacks();
			
			EventJump?.Invoke();
		}
		
		public virtual void JumpStop()
		{
			_controller.GravityActive = true;
			if (_controller.Velocity.y > 0)
			{
				_controller.Velocity.y = 0f;
			}

			_jumpStopped = true;
			_buttonReleased = false;
			PlayAbilityStopSfx();
			StopAbilityUsedSfx();
			StopStartFeedbacks();
			PlayAbilityStopFeedbacks();
			jumpStopFeedback?.PlayFeedbacks(this.transform.position);
		}
		
		public virtual void ResetNumberOfJumps()
		{
			_isInJump = false;
			bool shouldResetJumps = true;

			if (!resetJumpsOnTooSteepSlopes)
			{
				if (_controller3D.TooSteep())
				{
					shouldResetJumps = false;
				}
			}

			if (shouldResetJumps)
			{
				numberOfJumpsLeft = numberOfJumps;
			}

			_doubleJumping = false;
		}
		
		public override void ProcessAbility()
		{
			if (_controller.JustGotGrounded)
			{
				ResetNumberOfJumps();
			}

			if (!AbilityAuthorized
			    || (_condition.CurrentState != CharacterStates.CharacterConditions.Normal))
			{
				return;
			}

			CheckMovementStates();
			TryUpdateJump();
			TryResetNumberOfJumps();

			if (_controller.JustGotGrounded)
			{
				EventLanding?.Invoke();
			}
		}
		
		public override void UpdateAnimator()
		{
			MMAnimatorExtensions.UpdateAnimatorBool(_animator, _jumpingAnimationParameter,
				(_movement.CurrentState == CharacterStates.MovementStates.Jumping), _character._animatorParameters,
				_character.RunAnimatorSanityChecks);
			MMAnimatorExtensions.UpdateAnimatorBool(_animator, _doubleJumpingAnimationParameter, _doubleJumping,
				_character._animatorParameters, _character.RunAnimatorSanityChecks);
			MMAnimatorExtensions.UpdateAnimatorBool(_animator, _hitTheGroundAnimationParameter,
				_controller.JustGotGrounded, _character._animatorParameters, _character.RunAnimatorSanityChecks);
		}
		
		protected override void Initialization()
		{
			base.Initialization();

			ResetNumberOfJumps();
			_jumpStopped = true;
			
			_characterButtonActivation = _character?.FindAbility<CharacterButtonActivation>();
			_characterCrouch = _character?.FindAbility<CharacterCrouch>();
			_characterMove = _character?.FindAbility<CoreCharacterMovement>();
			
			jumpStartFeedback?.Initialization(this.gameObject);
			jumpStopFeedback?.Initialization(this.gameObject);
			_initialNumberOfJumps = numberOfJumps;
			
			if (blockControlOnJump)
			{
				_characterMove.TryAddChangeDirectionCondition(() => !_isInJump);
			}
		}

		protected virtual void CheckMovementStates()
		{
			if (!_jumpStopped
			    &&
			    ((_movement.CurrentState == CharacterStates.MovementStates.Idle)
			     || (_movement.CurrentState == CharacterStates.MovementStates.Walking)
			     || (_movement.CurrentState == CharacterStates.MovementStates.Running)
			     || (_movement.CurrentState == CharacterStates.MovementStates.Crouching)
			     || (_movement.CurrentState == CharacterStates.MovementStates.Crawling)
			     || (_movement.CurrentState == CharacterStates.MovementStates.Pushing)
			     || (_movement.CurrentState == CharacterStates.MovementStates.Falling)
			    ))
			{
				JumpStop();
			}
		}

		protected virtual void TryUpdateJump()
		{
			if (_movement.CurrentState == CharacterStates.MovementStates.Jumping)
			{
				if (_buttonReleased && !_jumpStopped && jumpProportionalToPress && (Time.time - _jumpStartedAt > minimumPressTime))
				{
					JumpStop();
				}

				if (!_jumpStopped)
				{
					if ((this.transform.position.y - _jumpOrigin.y > jumpHeight) || CeilingTest())
					{
						JumpStop();
						_controller3D.Grounded = _controller3D.IsGroundedTest();
						if (_controller.Grounded)
						{
							ResetNumberOfJumps();
						}
					}
					else
					{
						float heightPercent = (transform.position.y - _jumpOrigin.y) / jumpHeight;
						float force = jumpForceCurve.Evaluate(heightPercent) * jumpForce * Time.deltaTime;
						_jumpForce = Vector3.up * force;
						_controller.AddForce(_jumpForce);
					}
				}
			}
		}

		protected virtual void TryResetNumberOfJumps()
		{
			if (!resetJumpsOnTooSteepSlopes && _controller3D.ExitedTooSteepSlopeThisFrame && _controller3D.Grounded)
			{
				ResetNumberOfJumps();
			}
		}

		protected virtual bool CeilingTest()
		{
			bool returnValue = _controller3D.CollidingAbove();
			return returnValue;
		}
		
		protected virtual bool EvaluateJumpConditions()
		{
			if (!AbilityAuthorized)
			{
				return false;
			}

			if (_characterButtonActivation != null)
			{
				if (_characterButtonActivation.AbilityAuthorized
				    && _characterButtonActivation.InButtonActivatedZone
				    && _characterButtonActivation.PreventJumpInButtonActivatedZone)
				{
					return false;
				}
			}

			if (!canJumpOnTooSteepSlopes)
			{
				if (_controller3D.TooSteep())
				{
					return false;
				}
			}

			if (_characterCrouch != null)
			{
				if (_characterCrouch.InATunnel)
				{
					return false;
				}
			}

			if (CeilingTest())
			{
				return false;
			}

			if (numberOfJumpsLeft <= 0)
			{
				return false;
			}

			if (_movement.CurrentState == CharacterStates.MovementStates.Dashing)
			{
				return false;
			}

			return true;
		}
		
		protected override void InitializeAnimatorParameters()
		{
			RegisterAnimatorParameter(_jumpingAnimationParameterName, AnimatorControllerParameterType.Bool,
				out _jumpingAnimationParameter);
			RegisterAnimatorParameter(_doubleJumpingAnimationParameterName, AnimatorControllerParameterType.Bool,
				out _doubleJumpingAnimationParameter);
			RegisterAnimatorParameter(_hitTheGroundAnimationParameterName, AnimatorControllerParameterType.Bool,
				out _hitTheGroundAnimationParameter);
		}
	}
}

#endif