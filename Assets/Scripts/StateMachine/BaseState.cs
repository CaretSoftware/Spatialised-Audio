
namespace CharacterController {
    public abstract class BaseState : State {
        
        public CharacterController owner;
        private CharacterController _character;

        protected CharacterController Character => owner;
    }
}