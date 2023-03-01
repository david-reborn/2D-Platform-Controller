//using System;
//using System.Collections.Generic;
//using UnityEngine;

//namespace Myd.Platform.Demo
//{
//    public interface IPlayerContext
//    {
//        int MoveX { get; }
//        int MoveY { get; }
//        Vector2 Speed { get; set; }
//        System.Object Holding { get; }
//        bool OnGround { get; }
//        float JumpGraceTimer { get; }

//        float VarJumpSpeed { get; }     
//        float VarJumpTimer { get; set; }
//        void Jump();
//        void Dash();
//        void SetState(int state);

//        Vector2 LastAim { get; set; }
//        float MaxFall { get; set; }
//        float WallSpeedRetentionTimer { get; set; }

//        bool CanDash { get; }
//        float DashCooldownTimer { get; set; }

//        Facings Facing { get; set; }

//        void WallJump(int dir);

//        void ClimbJump();
//    }
//}
