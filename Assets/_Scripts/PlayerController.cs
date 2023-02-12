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
    public partial class PlayerController
    {
        private const int MaxDashes = 1;    // 最大Dash次数
        private readonly int GroundMask;


        Vector2 scale;
        Vector2 size;
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

        public float WallSlideTimer { get; set; } = Constants.WallSlideTime;
        public int WallSlideDir { get; set; }

        private bool onGround;

        public int ForceMoveX;
        public float ForceMoveXTimer;

        public int HopWaitX;   // If you climb hop onto a moving solid, snap to beside it until you get above it
        public float HopWaitXSpeed;

        private FiniteStateMachine<BaseActionState> stateMachine;

        public PlayerController()
        {
            this.stateMachine = new FiniteStateMachine<BaseActionState>((int)EActionState.Size);
            this.stateMachine.AddState(new NormalState(this));
            this.stateMachine.AddState(new DashState(this));
            this.stateMachine.AddState(new ClimbState(this));
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
                //Get ground
                if (Speed.y <= 0)
                {
                    this.onGround = CheckGround();//碰撞检测地面
                }
                else
                {
                    this.onGround = false;
                }
                //TODO Highest Air Y
                //TODO Flashing
                //Wall Slide
                if (this.WallSlideDir != 0)
                {
                    this.WallSlideTimer = Math.Max(this.WallSlideTimer - deltaTime, 0);
                    this.WallSlideDir = 0;
                }

                //TODO Wall Boost
                //After Dash
                if (this.onGround && this.stateMachine.State != (int)EActionState.Climb)
                {
                    //AutoJump = false;
                    //Stamina = ClimbMaxStamina;
                    this.WallSlideTimer = Constants.WallSlideTime;
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
                    if (Math.Sign(Speed.x) == -Math.Sign(wallSpeedRetained))
                        wallSpeedRetentionTimer = 0;
                    else if (!CollideCheck(Position, Vector2.right * Math.Sign(wallSpeedRetained)))
                    {
                        Debug.Log($"====UseWallSpeed:{wallSpeedRetained}");
                        Speed.x = wallSpeedRetained;
                        wallSpeedRetentionTimer = 0;
                    }
                    else
                        wallSpeedRetentionTimer -= deltaTime;
                }

                //Hop Wait X
                if (this.HopWaitX != 0)
                {
                    if (Math.Sign(Speed.x) == -HopWaitX || Speed.y > 0)
                        this.HopWaitX = 0;
                    else if (!CollideCheck(Position, Vector2.right * this.HopWaitX))
                    {
                        Speed.x = this.HopWaitXSpeed;
                        this.HopWaitX = 0;
                    }
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
                if (moveX != 0 && this.stateMachine.State != (int)EActionState.Climb)
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
            UpdateCollideX(Speed.x * deltaTime);
            UpdateCollideY(Speed.y * deltaTime);
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

        //处理跳跃
        public void Jump()
        {
            this.jumpGraceTimer = 0;
            this.WallSlideTimer = Constants.WallSlideTime;

            this.varJumpTimer = Constants.VarJumpTime;
            this.Speed.y = Constants.JumpSpeed;
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

        //表示进入爬墙状态有0.1秒时间,不发生下落
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
            Ducking = false;
            jumpGraceTimer = 0;
            varJumpTimer = Constants.VarJumpTime;
            //dashAttackTimer = 0;
            WallSlideTimer = Constants.WallSlideTime;
            //WallBoostTimer = 0;
            if (moveX != 0)
            {
                this.ForceMoveX = dir;
                this.ForceMoveXTimer = Constants.WallJumpForceTime;
            }

            //TODO 考虑电梯对速度的加成
            Speed.x = Constants.WallJumpHSpeed * dir;
            Speed.y = Constants.JumpSpeed;
            //Speed += LiftBoost;
            varJumpSpeed = Speed.y;
        }

        public void ClimbJump()
        {
            if (!onGround)
            {
                //Stamina -= ClimbJumpCost;

                //sweatSprite.Play("jump", true);
                //Input.Rumble(RumbleStrength.Light, RumbleLength.Medium);
            }
            Jump();
            if (moveX == 0)
            {
                //this.WallBoostDir = -(int)Facing;
                //this.wallBoostTimer = ClimbJumpBoostTime;
            }
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
                    return;
                }
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

        public bool CanUnDuck() => true;
    }
}