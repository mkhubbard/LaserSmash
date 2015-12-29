﻿using System;

namespace Game
{
    public class GameControllerState_INTRO_ANIM : StateMachineMB.State
    {
        public override Enum Name { get { return GameController.GameState.INTRO_ANIM; } }

        public override void OnStateEnter( StateMachineMB.State from_state )
        {
            GameController.instance.ResetGameEnvironment();
        }

        public override void OnStateExit( StateMachineMB.State to_state )
        {
            GameController.instance.GameEnv.PlayerComponent.ChangeState( Player.PlayerState.RESET );
            GameController.instance.GameEnv.WaveCon.Paused = false;
        }

        public override void OnUpdate()
        {
            ;
        }

        public override void OnStateMessageReceived( object o )
        {
            Owner.ChangeState( GameController.GameState.RUNNING );
        }
    }
}