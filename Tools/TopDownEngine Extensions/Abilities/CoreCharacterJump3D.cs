#if MOREMOUNTAINS_TOPDOWNENGINE


using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using UnityEngine;

namespace Larje.Core.Tools.TopDownEngine
{
	public class CoreCharacterJump3D : CharacterAbility
	{
		[Header("Jump Settings")] public bool JumpProportionalToPress = true;
		public float MinimumPressTime = 0.4f;
		public float JumpForce = 800f;
		public float JumpHeight = 4f;
		public AnimationCurve JumpForceCurve = AnimationCurve.Constant(0f, 1f, 1f);

		[Header("Slopes")] public bool CanJumpOnTooSteepSlopes = true;
		public bool ResetJumpsOnTooSteepSlopes = false;

		[Header("Number of Jumps")] public int NumberOfJumps = 1;
		[MMReadOnly] public int NumberOfJumpsLeft = 0;

		[Header("Feedbacks")] public MMFeedbacks JumpStartFeedback;
		public MMFeedbacks JumpStopFeedback;

		protected bool _doubleJumping;
		protected Vector3 _jumpForce;
		protected Vector3 _jumpOrigin;
		protected CharacterButtonActivation _characterButtonActivation;
		protected CharacterCrouch _characterCrouch;
		protected bool _jumpStopped = false;
		protected float _jumpStartedAt = 0f;
		protected bool _buttonReleased = false;
		protected int _initialNumberOfJumps;

		protected const string _jumpingAnimationParameterName = "Jumping";
		protected const string _doubleJumpingAnimationParameterName = "DoubleJumping";
		protected const string _hitTheGroundAnimationParameterName = "HitTheGround";
		protected int _jumpingAnimationParameter;
		protected int _doubleJumpingAnimationParameter;
		protected int _hitTheGroundAnimationParameter;

		protected override void Initialization()
		{
			base.Initialization();

			ResetNumberOfJumps();
			_jumpStopped = true;
			_characterButtonActivation = _character?.FindAbility<CharacterButtonActivation>();
			_characterCrouch = _character?.FindAbility<CharacterCrouch>();
			JumpStartFeedback?.Initialization(this.gameObject);
			JumpStopFeedback?.Initialization(this.gameObject);
			_initialNumberOfJumps = NumberOfJumps;
		}

		protected override void HandleInput()
		{
			base.HandleInput();
			if (!AbilityAuthorized
			    || (_condition.CurrentState != CharacterStates.CharacterConditions.Normal))
			{
				return;
			}

			if (_inputManager.JumpButton.State.CurrentState == MMInput.ButtonStates.ButtonDown)
			{
				JumpStart();
			}

			if (_inputManager.JumpButton.State.CurrentState == MMInput.ButtonStates.ButtonUp)
			{
				_buttonReleased = true;
			}
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
				if (_buttonReleased && !_jumpStopped && JumpProportionalToPress && (Time.time - _jumpStartedAt > MinimumPressTime))
				{
					JumpStop();
				}

				if (!_jumpStopped)
				{
					if ((this.transform.position.y - _jumpOrigin.y > JumpHeight) || CeilingTest())
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
						float heightPercent = (transform.position.y - _jumpOrigin.y) / JumpHeight;
						float force = JumpForceCurve.Evaluate(heightPercent) * JumpForce * Time.deltaTime;
						_jumpForce = Vector3.up * force;
						_controller.AddForce(_jumpForce);
					}
				}
			}
		}

		protected virtual void TryResetNumberOfJumps()
		{
			if (!ResetJumpsOnTooSteepSlopes && _controller3D.ExitedTooSteepSlopeThisFrame && _controller3D.Grounded)
			{
				ResetNumberOfJumps();
			}
		}

	protected virtual bool CeilingTest()
		{
			bool returnValue = _controller3D.CollidingAbove();
			return returnValue;
		}
		
		public virtual void JumpStart()
		{
			if (!EvaluateJumpConditions())
			{
				return;
			}

			if (NumberOfJumpsLeft != NumberOfJumps)
			{
				_doubleJumping = true;
			}

			NumberOfJumpsLeft = NumberOfJumpsLeft - 1;

			_movement.ChangeState(CharacterStates.MovementStates.Jumping);
			MMCharacterEvent.Trigger(_character, MMCharacterEventTypes.Jump);
			JumpStartFeedback?.PlayFeedbacks(this.transform.position);
			_jumpOrigin = this.transform.position;
			_jumpStopped = false;
			_jumpStartedAt = Time.time;
			_controller.Grounded = false;
			_controller.GravityActive = false;
			_buttonReleased = false;

			PlayAbilityStartSfx();
			PlayAbilityUsedSfx();
			PlayAbilityStartFeedbacks();
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
			JumpStopFeedback?.PlayFeedbacks(this.transform.position);
		}
		
		public virtual void ResetNumberOfJumps()
		{
			bool shouldResetJumps = true;

			if (!ResetJumpsOnTooSteepSlopes)
			{
				if (_controller3D.TooSteep())
				{
					shouldResetJumps = false;
				}
			}

			if (shouldResetJumps)
			{
				NumberOfJumpsLeft = NumberOfJumps;
			}

			_doubleJumping = false;
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

			if (!CanJumpOnTooSteepSlopes)
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

			if (NumberOfJumpsLeft <= 0)
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
	}
}

#endif