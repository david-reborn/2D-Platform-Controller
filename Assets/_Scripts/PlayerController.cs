using System;
using UnityEditor;
using UnityEngine;


namespace Myd.Platform.Demo
{
    public struct Asix
    {
        public float moveX;
        public float MoveY;
    }
    public class PlayerController
    {
        private const int MaxDashes = 1;    // 最大Dash次数
        private readonly int GroundMask;

        private readonly Rect normalHitbox = new Rect(0, -0.25f, 0.8f, 1.1f);
        private readonly Rect duckHitbox = new Rect(0, -0.5f, 0.8f, 0.6f);
        private readonly Rect normalHurtbox = new Rect(0f, -0.15f, 0.8f, 0.9f);
        private readonly Rect duckHurtbox = new Rect(8f, 4f, 0.8f, 0.4f);
        private Rect hitbox;
        private Rect hurtbox;
        Vector2 scale;
        Vector2 size;
        Vector2 speed;   //移动向量
        float jumpGraceTimer;
        float varJumpTimer;
        float varJumpSpeed; //
        int moveX;
        private float maxFall;
        private float fastMaxFall;

        private float dashCooldownTimer;                //冲刺冷却时间计数器，为0时，可以再次冲刺
        private float dashRefillCooldownTimer;          //
        private int dashes;
        private float wallSpeedRetentionTimer; // If you hit a wall, start this timer. If coast is clear within this timer, retain h-speed
        private float wallSpeedRetained;
        private bool onGround;

        private FiniteStateMachine<BaseActionState> stateMachine;

        public PlayerController()
        {
            this.stateMachine = new FiniteStateMachine<BaseActionState>((int)EActionState.Size);
            this.stateMachine.AddState(new NormalState(this));
            this.stateMachine.AddState(new DashState(this));
            this.GroundMask = LayerMask.GetMask("Ground");

            this.LastAim = Vector2.right;
            this.Facing = Facings.Right;
        }

        public void Init(Vector2 position)
        {
            //根据进入的方式,决定初始状态
            this.stateMachine.State = (int)EActionState.Normal;
            this.dashes = 1;
            this.Position = position;
        }

        public void Update(float deltaTime)
        {
            //更新变量状态
            {
                if (speed.y <= 0)
                {
                    //碰撞检测地面
                    this.onGround = CollideY();
                }
                else
                {
                    this.onGround = false;
                }

                //Var Jump
                if (varJumpTimer > 0)
                {
                    varJumpTimer -= deltaTime;
                }

                //Force Move X
                //if (forceMoveXTimer > 0)
                //{
                //    forceMoveXTimer -= Engine.DeltaTime;
                //    moveX = forceMoveX;
                //}
                //else
                {
                    //输入
                    this.moveX = Math.Sign(UnityEngine.Input.GetAxisRaw("Horizontal"));
                }

                //撞墙以后的速度保持，Wall Speed Retention
                if (wallSpeedRetentionTimer > 0)
                {
                    if (Math.Sign(speed.x) == -Math.Sign(wallSpeedRetained))
                        wallSpeedRetentionTimer = 0;
                    else if (!CollideCheck(Position + Vector2.right * Math.Sign(wallSpeedRetained) * 0.00001f))
                    {
                        Debug.Log($"====UseWallSpeed:{wallSpeedRetained}");
                        speed.x = wallSpeedRetained;
                        wallSpeedRetentionTimer = 0;
                    }
                    else
                        wallSpeedRetentionTimer -= deltaTime;
                }

                //更新冲刺冷却时间
                {
                    if (dashCooldownTimer > 0)
                        dashCooldownTimer -= deltaTime;
                    if (dashRefillCooldownTimer > 0)
                    {
                        dashRefillCooldownTimer -= deltaTime;
                    }
                    else if (onGround)
                    {
                        RefillDash();
                    }
                }

                //Facing
                //if (moveX != 0 && InControl && StateMachine.State != StClimb && StateMachine.State != StPickup && StateMachine.State != StRedDash && StateMachine.State != StHitSquash)
                if (moveX != 0)
                {
                    Facing = (Facings)moveX;
                }
                LastAim = Input.GetAimVector(Facing);
            }

            //落地设置土狼时间
            if (OnGround)
            {
                //dreamJump = false;
                jumpGraceTimer = Constants.JumpGraceTime;
            }
            else
            {
                if (jumpGraceTimer > 0)
                {
                    jumpGraceTimer -= deltaTime;
                }
            }

            //处理逻辑
            stateMachine.Update(deltaTime);

            //更新位置
            UpdatePositionX(speed.x * deltaTime);
            UpdatePositionY(speed.y * deltaTime);
            //Physics
            //if (StateMachine.State != StDreamDash && StateMachine.State != StAttract)
            //    MoveH(Speed.X * Engine.DeltaTime, onCollideH);
            //if (StateMachine.State != StDreamDash && StateMachine.State != StAttract)
            //    MoveV(Speed.Y * Engine.DeltaTime, onCollideV);

            //更新
            UpdateSprite(deltaTime);
        }

        public void Render()
        {
        }

        private void UpdateSprite(float deltaTime)
        {
            Vector2 tempScale = Scale;
            tempScale.x = Mathf.MoveTowards(tempScale.x, 1f, 1.75f * deltaTime);
            tempScale.y = Mathf.MoveTowards(tempScale.y, 1f, 1.75f * deltaTime);
            Scale = tempScale;
        }

        private void UpdatePositionX(float distX)
        {
            if (distX == 0)
                return;
            //目标位置
            Vector2 direct = Math.Sign(distX) > 0 ? Vector2.right : Vector2.left;
            Vector2 targetPosition = this.Position;

            Vector2 origion = this.Position + normalHitbox.position + Vector2.up * 0.01f;

            RaycastHit2D hit = Physics2D.BoxCast(origion, normalHitbox.size, 0, direct, Mathf.Abs(distX) + 0.01f, GroundMask);
            if (hit && hit.normal == -direct)
            {
                //如果发生碰撞,则移动距离
                targetPosition += direct * (hit.distance - 0.01f);
                //Speed retention
                //if (wallSpeedRetentionTimer <= 0)
                //{
                //    wallSpeedRetained = this.speed.x;
                //    wallSpeedRetentionTimer = Constants.WallSpeedRetentionTime;
                //}
                this.speed.x = 0;
            }
            else
            {
                targetPosition += Vector2.right * distX;
            }
            this.Position = targetPosition;
        }
        private void UpdatePositionY(float distY)
        {
            Vector2 targetPosition = this.Position;
            Vector2 direct = Math.Sign(distY) > 0 ? Vector2.up : Vector2.down;
            Vector2 origion = this.Position + normalHitbox.position;
            RaycastHit2D hit = Physics2D.BoxCast(origion, normalHitbox.size, 0, direct, Mathf.Abs(distY), GroundMask);
            if (hit && hit.normal == -direct)
            {
                //如果发生碰撞,则移动距离
                targetPosition += direct * (hit.distance);
            }
            else
            {
                targetPosition += Vector2.up * distY;
            }
            this.Position = targetPosition;
        }

        //针对横向,进行碰撞检测.如果发生碰撞,
        private bool CollideY()
        {
            Vector2 origion = this.Position + Vector2.up * normalHitbox.position.y;
            RaycastHit2D hit = Physics2D.BoxCast(origion, normalHitbox.size, 0, Vector2.down, 0.00001f, GroundMask);
            if (hit && hit.normal == Vector2.up)
            {
                return true;
            }
            return false;
        }

        //处理跳跃
        public void Jump()
        {
            this.jumpGraceTimer = 0;
            this.varJumpTimer = Constants.VarJumpTime;
            this.speed.y = Constants.JumpSpeed;
            this.varJumpSpeed = Constants.JumpSpeed;

            Scale = new Vector2(.6f, 1.4f);
        }

        public bool RefillDash()
        {
            if (this.dashes < MaxDashes)
            {
                this.dashes = MaxDashes;
                return true;
            }
            else
                return false;
        }

        #region 实现IPlayerContext接口
        public bool CanDash
        {
            get
            {
                return Input.Dash.Pressed() && dashCooldownTimer <= 0 && this.dashes > 0;
            }
        }

        public float WallSpeedRetentionTimer
        {
            get { return this.wallSpeedRetentionTimer; }
            set { this.wallSpeedRetentionTimer = value; }
        }
        public Vector2 Speed;

        public object Holding => null;

        public bool OnGround => this.onGround;

        public float JumpGraceTimer => jumpGraceTimer;

        public Vector2 Position { get; private set; }
        public Vector2 Scale { get; private set; }

        public float ClimbNoMoveTimer { get; set; }
        public float VarJumpSpeed => this.varJumpSpeed;

        public float VarJumpTimer
        {
            get
            {
                return this.varJumpTimer;
            }
            set
            {
                this.varJumpTimer = value;
            }
        }

        public int MoveX => moveX;
        public int MoveY => Math.Sign(UnityEngine.Input.GetAxisRaw("Vertical"));

        public float MaxFall { get => maxFall; set => maxFall = value; }
        public float DashCooldownTimer { get => dashCooldownTimer; set => dashCooldownTimer = value; }
        public Vector2 LastAim { get; set; }
        public Facings Facing { get; set; }  //当前朝向
        public void Dash()
        {
            //wasDashB = Dashes == 2;
            this.dashes = Math.Max(0, this.dashes - 1);
            //Input.Dash.ConsumeBuffer();
        }
        public void SetState(int state)
        {
            this.stateMachine.State = state;
        }
        #endregion


        public void WallJump(int dir)
        {

        }

        public void ClimbJump()
        {

        }

        public bool Ducking
        {
            get
            {
                return this.hitbox == this.duckHitbox || this.hitbox == this.duckHurtbox;
            }
            set
            {
                if (value)
                {
                    this.hitbox = this.duckHitbox;
                    this.hurtbox = this.duckHurtbox;
                    return;
                }
                this.hurtbox = this.normalHurtbox;
            }
        }

        public bool IsTired
        {
            get
            {
                return false;
            }
            set
            {

            }
        }

        #region Physics
        private bool CollideCheck(Vector2 position)
        {
            return Physics2D.OverlapBox(position, normalHitbox.size, 0, GroundMask);
        }

        public bool ClimbCheck(int dir, int yAdd = 0)
        {
            //检查在关卡范围内
            //if (!this.ClimbBoundsCheck(dir))
            //    return false;

            //且前面两个单元没有ClimbBlock
            //if (ClimbBlocker.Check(base.Scene, this, this.Position + Vector2.UnitY * (float)yAdd + Vector2.UnitX * 2f * (float)this.Facing))
            //    return false;
            
            //TODO 获取当前的碰撞体
            if(Physics2D.OverlapBox(this.Position, normalHitbox.size, 0, GroundMask))
            {

            }

            return true;
        }
        #endregion
    }
}