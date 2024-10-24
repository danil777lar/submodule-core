#if MOREMOUNTAINS_TOPDOWNENGINE

using System.Collections;
using System.Collections.Generic;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using UnityEngine;

namespace Larje.Core.Tools.TopDownEngine
{
	public class CharacterCapsulePush3D : CharacterAbility
	{
		protected const string _pushingAnimationParameterName = "Pushing";
		
		[Header("Physics interaction")]
		public bool AllowPhysicsInteractions = true;
		public float PhysicsInteractionsRaycastLength = 0.05f;
		public float PushPower = 1850f;
		public LayerMask InteractionMask;
		public ForceMode ForceMode = ForceMode.VelocityChange;
		
		protected bool _pushing = false;
		protected int _pushingAnimationParameter;
		protected CharacterController _characterController;
		protected CoreCharacterMovement _coreMovement;

		protected override void Initialization()
		{
			base.Initialization();
			_characterController = _controller.GetComponent<CharacterController>();
			_controller3D = _controller.GetComponent<TopDownController3D>();
			_coreMovement = _character.FindAbility<CoreCharacterMovement>();
		}

		public override void ProcessAbility()
		{
			base.ProcessAbility();

			if (!AbilityAuthorized
			    || ((_condition.CurrentState != CharacterStates.CharacterConditions.Normal) &&
			        (_condition.CurrentState != CharacterStates.CharacterConditions.ControlledMovement)))
			{
				return;
			}

			HandlePhysicsInteractions();
		}

		protected virtual void HandlePhysicsInteractions()
		{
			if (!AllowPhysicsInteractions)
			{
				return;
			}

			Vector3 point1 = _controller3D.ColliderBottom;
			Vector3 point2 = _controller3D.ColliderTop;
			Vector3 direction = _coreMovement.RawDirection;
			Debug.DrawRay(transform.position, direction * 5f, Color.red);
			float radius = _characterController.radius;
			float maxDistance = radius + _characterController.skinWidth + PhysicsInteractionsRaycastLength;
			
			RaycastHit[] hits = Physics.CapsuleCastAll(point1, point2, radius, direction, maxDistance, InteractionMask);
			_pushing = (hits.Length > 0);
			if (_pushing)
			{
				foreach (RaycastHit hit in hits)
				{
					HandlePush(hit);	
				}
			}
		}
		
		protected virtual void HandlePush(RaycastHit hit)
		{
			Rigidbody pushedRigidbody = hit.collider.attachedRigidbody;
			if ((pushedRigidbody == null) || (pushedRigidbody.isKinematic))
			{
				return;
			}

			Vector3 direction = -hit.normal;
			Debug.DrawRay(hit.point, direction * 5f, Color.green);
			Vector3 force = direction * (PushPower * Time.deltaTime);
			pushedRigidbody.AddForceAtPosition(force, hit.point, ForceMode);
		}
		
		protected override void InitializeAnimatorParameters()
		{
			RegisterAnimatorParameter(_pushingAnimationParameterName, AnimatorControllerParameterType.Float,
				out _pushingAnimationParameter);
		}

		public override void UpdateAnimator()
		{
			MMAnimatorExtensions.UpdateAnimatorBool(_animator, _pushingAnimationParameter, _pushing,
				_character._animatorParameters, _character.RunAnimatorSanityChecks);
		}
	}
}

#endif