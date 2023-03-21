using System;
using System.Collections;
using UnityEngine;

namespace Myd.Platform
{
    /// <summary>
    /// 移动组件，主要用于维护基础的运动参数，该运动参数为所有组件共用
    /// </summary>
    public class MovementComponent
    {
        public int MoveX;
        public int MoveY;

        public Vector2 Speed;

        //private int groundMask;

        //private int moveX;
        //private Vector2 lastAim;    //目标
        //private Facings facing;     //朝向
        //private float maxFall;
        //private float fastMaxFall;

        //private bool onGround;
        //private bool wasOnGround;

        //public int MoveX => moveX;
        //public int MoveY => Math.Sign(UnityEngine.Input.GetAxisRaw("Vertical"));
        //public float MaxFall { get => maxFall; set => maxFall = value; }
        //public Vector2 LastAim { get => lastAim; set => lastAim = value; }
        //public Facings Facing { get => facing; set => facing = value; }  //当前朝向

        //private Vector2 speed;
        //public int ForceMoveX { get; set; }
        //public float ForceMoveXTimer { get; set; }
        //public MovementComponent()
        //{

        //}

        //public void Init()
        //{
        //    this.groundMask = LayerMask.GetMask("Ground");
        //    this.facing = Facings.Right;
        //    this.lastAim = Vector2.right;
        //}

        //public void Update()
        //{
        //    //Force Move X
        //    if (ForceMoveXTimer > 0)
        //    {
        //        ForceMoveXTimer -= deltaTime;
        //        this.moveX = ForceMoveX;
        //    }
        //    else
        //    {
        //        //输入
        //        this.moveX = Math.Sign(UnityEngine.Input.GetAxisRaw("Horizontal"));
        //    }

        //    //Facing
        //    if (moveX != 0 && this.stateMachine.State != (int)EActionState.Climb)
        //    {
        //        Facing = (Facings)moveX;
        //    }
        //}

        //public void UpdatePosition()
        //{
        //    //更新位置
        //    UpdateCollideX(Speed.x * deltaTime);
        //    UpdateCollideY(Speed.y * deltaTime);
        //}
    }
}