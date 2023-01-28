using System;
using System.Collections.Generic;
using UnityEngine;

namespace Myd.Platform.Demo
{
    public interface IPlayerContext
    {
        int MoveX { get; }
        int MoveY { get; }
        Vector2 Speed { get; set; }
        System.Object Holding { get; }
        bool OnGround { get; }
        bool JumpPressed { get; } //Jump键刚刚按下
        bool JumpChecked { get; } //Jump键处于按下状态
        float JumpGraceTimer { get; }

        float VarJumpSpeed { get; }     
        float VarJumpTimer { get; set; }
        void Jump();


        float MaxFall { get; set; }
        float WallSpeedRetentionTimer { get; set; }
    }
}
