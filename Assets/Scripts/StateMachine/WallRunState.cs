using UnityEngine;

namespace CharacterController {
	public class WallRunState : BaseState {
		private const float AntiFloatForce = 25.0f;
		private static float wallRunMagnitudeThreshold = 4.0f;
		private Vector3 _wallNormal;
		private const string State = "WallRunState";

		public static bool Requirement(CharacterController character) {

			if (character.Grounded || Vector3.Dot(character._velocity, character.transform.forward) < 0.0f)
				return false;

			Vector3 verticalVelocity = Vector3.ProjectOnPlane(character._velocity, Vector3.up);

			RaycastHit rightHit = RayCast(character, character.transform.right);
			if (rightHit.collider) {
				Vector3 wallProjectionVector = Vector3.ProjectOnPlane(verticalVelocity, rightHit.normal);
				return wallProjectionVector.magnitude > wallRunMagnitudeThreshold;
			}

			RaycastHit leftHit = RayCast(character, -character.transform.right);
			if (leftHit.collider) {
				Vector3 wallProjectionVector = Vector3.ProjectOnPlane(verticalVelocity, leftHit.normal);
				return wallProjectionVector.magnitude > wallRunMagnitudeThreshold;
			}

			return false;
		}

		public override void Enter() {
			StateChange.stateUpdate?.Invoke(State);
			RaycastHit rightHit = RayCast(Character, Character.transform.right);
			RaycastHit leftHit = RayCast(Character, -Character.transform.right);

			if (rightHit.collider)
				_wallNormal = rightHit.normal;
			else if (leftHit.collider)
				_wallNormal = leftHit.normal;
		}

		public override void Run() {
			// Move character along vector of wall surface normal
			// project characters _velocity on wall normal, multiply with initial wall running speed
			// redirect characters _velocity into run? add upwards velocity?
			// decrease speed with time

			// Player.AirControl();

			AddGravityForce();

			// Player.ApplyAirFriction();

			// if (Player.pressedJump && Player._velocity.y > threshold) 
			// 	stateMachine.TransitionTo<JumpState>();

			if (Character.PressedJump)
				stateMachine.TransitionTo<WallJumpState>();

			if (!RayCast(Character, Character.transform.right).collider && !RayCast(Character, -Character.transform.right).collider)
				stateMachine.TransitionTo<AirState>();

			if (Character.Grounded)
				stateMachine.TransitionTo<MoveState>();
		}

		private void AddGravityForce() {

			Vector3 gravityMovement = Character._defaultGravity * Time.deltaTime * Vector3.down;
			if (Character._velocity.y > 0.0f)
				gravityMovement *= .75f;
			else {
				Character._velocity += Character._velocity.y * -_wallNormal * Time.deltaTime;
			}

			//CounteractFloat();
			Character._velocity += gravityMovement;
		}

		private static RaycastHit RayCast(CharacterController character, Vector3 direction) {
			Ray ray = new Ray(character._point2Transform.position/*Player.transform.position + Player._point2*/, direction);
			Physics.Raycast(ray, out var hit, character._colliderRadius + .5f, character._collisionMask);
			return hit;
		}

		public override void Exit() { }
	}
}