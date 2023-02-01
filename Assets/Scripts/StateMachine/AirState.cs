using UnityEngine;

namespace CharacterController {
	public class AirState : BaseState {

		private const float AntiFloatForce = 25.0f;

		private const string State = "AirState";
		public override void Enter() {
			
			StateChange.stateUpdate?.Invoke(State);
		}

		public override void Run() {

			Character.AirControl();

			AddGravityForce();

			Character.ApplyAirFriction();

			if (Character.Grounded)
				stateMachine.TransitionTo<MoveState>();

			if (Character.Jumped) // coyote time jump
				stateMachine.TransitionTo<JumpState>();

			if (WallRunState.Requirement(Character))
				stateMachine.TransitionTo<WallRunState>();
		}

		private void AddGravityForce() {

			float gravityMovement =
				-Character._defaultGravity * Character._fallGravityMultiplier * Time.deltaTime;
			
			CounteractFloat();
			
			Character._velocity.y += gravityMovement;

			void CounteractFloat() {
				if (Character._velocity.y > 0)
					gravityMovement -= AntiFloatForce * Time.deltaTime;
			}
		}

		public override void Exit() { }
	}
}