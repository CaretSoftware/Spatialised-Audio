
    public abstract class BaseState : State {
        
        public CharController.CharacterController owner;
        private CharController.CharacterController _character;

        protected CharController.CharacterController Character => owner;
    }
