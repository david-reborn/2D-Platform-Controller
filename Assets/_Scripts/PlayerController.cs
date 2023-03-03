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

    /// <summary>
    /// 玩家操作控制器
    /// </summary>
    public partial class PlayerController
    {
        private readonly int GroundMask;

        float varJumpTimer;
        float varJumpSpeed; //
        int moveX;
        private float maxFall;
        private float fastMaxFall;

        private float dashCooldownTimer;                //冲刺冷却时间计数器，为0时，可以再次冲刺
        private float dashRefillCooldownTimer;          //
        public int dashes;
        public int lastDashes;
        private float wallSpeedRetentionTimer; // If you hit a wall, start this timer. If coast is clear within this timer, retain h-speed
        private float wallSpeedRetained;

        private bool onGround;
        private bool wasOnGround;
        
        public bool DashStartedOnGround { get; set; }

        public int ForceMoveX { get; set; }
        public float ForceMoveXTimer { get; set; }

        public int HopWaitX;   // If you climb hop onto a moving solid, snap to beside it until you get above it
        public float HopWaitXSpeed;

        public WallSlide WallSlide { get; set; }    //WallSlide
        public JumpCheck JumpCheck { get; set; }    //土狼时间
        public WallBoost WallBoost { get; set; }    //WallBoost
        private FiniteStateMachine<BaseActionState> stateMachine;

        public PlayerController(IPlayerContext context)
        {
            RefreshAbility(context);

            this.stateMachine = new FiniteStateMachine<BaseActionState>((int)EActionState.Size);
            this.stateMachine.AddState(new NormalState(this));
            this.stateMachine.AddState(new DashState(this));
            this.stateMachine.AddState(new ClimbState(this));
            this.GroundMask = LayerMask.GetMask("Ground");

            this.Facing  = Facings.Right;
            this.LastAim = Vector2.right;
        }

        public void RefreshAbility(IPlayerContext context)
        {
            //启用或者禁用功能组件或特性
            if (!Constants.EnableWallSlide)
            {
                this.WallSlide = null;
            }
            else
            {
                this.WallSlide = this.WallSlide == null ? new WallSlide(this) : this.WallSlide;
            }

            this.JumpCheck = new JumpCheck(this, Constants.EnableJumpGrace);

            if (!Constants.EnableWallBoost)
            {
                this.WallBoost = null;
            }
            else
            {
                this.WallBoost = this.WallBoost == null ? new WallBoost(this) : this.WallBoost;
            }
        }

        public void Init(Vector2 position)
        {
            //根据进入的方式,决定初始状态
            this.stateMachine.State = (int)EActionState.Normal;
            this.dashes = 1;
            this.Position = position;
            this.collider = normalHitbox;

            //TODO 初始化尾巴颜色
            //Color color = NormalHairColor;
            //Gradient gradient = new Gradient();
            //gradient.SetKeys(
            //    new GradientColorKey[] { new GradientColorKey(color, 0.0f), new GradientColorKey(color, 1.0f) },
            //    new GradientAlphaKey[] { new GradientAlphaKey(1, 0.0f), new GradientAlphaKey(1, 0.6f), new GradientAlphaKey(0, 1.0f) }
            //);

            //this.player.SetTrailColor(gradient);

        }

        public void Update(float deltaTime)
        {
            //更新各个组件中变量的状态
            {
                //Get ground
                wasOnGround = onGround;
                if (Speed.y <= 0)
                {
                    this.onGround = CheckGround();//碰撞检测地面
                }
                else
                {
                    this.onGround = false;
                }

                //Wall Slide
                if (this.WallSlide != null)
                {
                    this.WallSlide.Update(deltaTime);
                    this.WallSlide.Check(this.onGround, this.stateMachine.State != (int)EActionState.Climb);
                }

                //Wall Boost, 不消耗体力WallJump
                this.WallBoost?.Update(deltaTime);
                
                //跳跃检查
                this.JumpCheck?.Update(deltaTime);

                //Dash
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

                //Var Jump
                if (varJumpTimer > 0)
                {
                    varJumpTimer -= deltaTime;
                }

                //Force Move X
                if (ForceMoveXTimer > 0)
                {
                    ForceMoveXTimer -= deltaTime;
                    this.moveX = ForceMoveX;
                }
                else
                {
                    //输入
                    this.moveX = Math.Sign(UnityEngine.Input.GetAxisRaw("Horizontal"));
                }

                //Facing
                if (moveX != 0 && this.stateMachine.State != (int)EActionState.Climb)
                {
                    Facing = (Facings)moveX;
                }
                //Aiming
                LastAim = Input.GetAimVector(Facing);

                //撞墙以后的速度保持，Wall Speed Retention，用于撞开
                if (wallSpeedRetentionTimer > 0)
                {
                    if (Math.Sign(Speed.x) == -Math.Sign(wallSpeedRetained))
                        wallSpeedRetentionTimer = 0;
                    else if (!CollideCheck(Position, Vector2.right * Math.Sign(wallSpeedRetained)))
                    {
                        Speed.x = wallSpeedRetained;
                        wallSpeedRetentionTimer = 0;
                    }
                    else
                        wallSpeedRetentionTimer -= deltaTime;
                }

                //Hop Wait X
                if (this.HopWaitX != 0)
                {
                    if (Math.Sign(Speed.x) == -HopWaitX || Speed.y < 0)
                        this.HopWaitX = 0;
                    else if (!CollideCheck(Position, Vector2.right * this.HopWaitX))
                    {
                        Speed.x = this.HopWaitXSpeed;
                        this.HopWaitX = 0;
                    }
                }
            }

            //状态机更新逻辑
            stateMachine.Update(deltaTime);
            //更新位置
            UpdateCollideX(Speed.x * deltaTime);
            UpdateCollideY(Speed.y * deltaTime);

        }

        //private Color hairColor;
        //private float hairFlashTimer;
        ////Gradient gradient = new Gradient();
        
        //private void SetHairColor(Color color)
        //{
        //    Gradient gradient = new Gradient();
        //    gradient.SetKeys(
        //        new GradientColorKey[] { new GradientColorKey(color, 0.0f), new GradientColorKey(color, 1.0f) },
        //        new GradientAlphaKey[] { new GradientAlphaKey(1, 0.0f), new GradientAlphaKey(1, 0.6f), new GradientAlphaKey(0, 1.0f) }
        //    );
        //    this.player.SetTrailColor(gradient);
        //}
        //private void UpdateHair(float deltaTime)
        //{
        //    if (this.dashes == 0 && this.dashes < MaxDashes)
        //    {
        //        hairColor = Color.Lerp(hairColor, UsedHairColor, 6f * deltaTime);
        //        SetHairColor(hairColor);
        //    }
        //    else
        //    {
        //        Color color;
        //        if (this.lastDashes != this.dashes)
        //        {
        //            color = FlashHairColor;
        //            hairFlashTimer = .12f;
        //        }
        //        else if (hairFlashTimer > 0)
        //        {
        //            color = FlashHairColor;
        //            hairFlashTimer -= deltaTime;
        //        }
        //        else if (this.dashes == 2)
        //            color = Color.black;//TwoDashesHairColor;
        //        else
        //            color = NormalHairColor;

        //        this.hairColor = color;
        //        SetHairColor(hairColor);
        //    }
        //    lastDashes = dashes;
        //}

        //处理跳跃,跳跃时候，会给跳跃前方一个额外的速度
        public void Jump()
        {
            Input.Jump.ConsumeBuffer();
            this.JumpCheck?.ResetTime();
            this.WallSlide?.ResetTime();
            this.WallBoost?.ResetTime();
            this.varJumpTimer = Constants.VarJumpTime;
            this.Speed.x += Constants.JumpHBoost * moveX;
            this.Speed.y = Constants.JumpSpeed;
            //Speed += LiftBoost;
            this.varJumpSpeed = this.Speed.y;

            EventManager.instance.FireOnJump();
        }

        //SuperJump，表示在地面上或者土狼时间内，Dash接跳跃。
        //数值方便和Jump类似，数值变大。
        //蹲伏状态的SuperJump需要额外处理。
        public void SuperJump()
        {
            
        }

        //在墙边情况下的，跳跃。主要需要考虑当前跳跃朝向
        public void WallJump(int dir)
        {
            Input.Jump.ConsumeBuffer();
            Ducking = false;
            this.JumpCheck?.ResetTime();
            varJumpTimer = Constants.VarJumpTime;
            this.WallSlide?.ResetTime();
            this.WallBoost?.ResetTime();
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

            EventManager.instance.FireOnJump();
            //TODO，墙上的粒子效果。
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
            WallBoost?.Active();
        }

        //在墙边Dash时，当前按住上，不按左右时，执行SuperWallJump
        public void SuperWallJump(int dir)
        {
            Input.Jump.ConsumeBuffer();
            Ducking = false;
            this.JumpCheck?.ResetTime();
            varJumpTimer = Constants.SuperWallJumpVarTime;
            this.WallSlide?.ResetTime();
            this.WallBoost?.ResetTime();

            //TODO 考虑电梯对速度的加成
            Speed.x = Constants.SuperWallJumpH * dir;
            Speed.y = Constants.SuperWallJumpSpeed;
            //Speed += LiftBoost;
            varJumpSpeed = Speed.y;

            EventManager.instance.FireOnJump();
        }

        public bool RefillDash()
        {
            if (this.dashes < Constants.MaxDashes)
            {
                this.dashes = Constants.MaxDashes;
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

        public Vector2 Position { get; private set; }

        //表示进入爬墙状态有0.1秒时间,不发生移动，为了让玩家看清发生了爬墙的动作
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
        public EActionState Dash()
        {
            //wasDashB = Dashes == 2;
            this.dashes = Math.Max(0, this.dashes - 1);
            Input.Dash.ConsumeBuffer();
            return EActionState.Dash;
        }
        public void SetState(int state)
        {
            this.stateMachine.State = state;
        }
        #endregion

        public bool Ducking
        {
            get
            {
                return this.collider == this.duckHitbox || this.collider == this.duckHurtbox;
            }
            set
            {
                if (value)
                {
                    this.collider = this.duckHitbox;
                    return;
                }
                else
                {
                    this.collider = this.normalHitbox;
                }
                EventManager.Get().FireOnDuck(value);
            }
        }

        //检测当前是否可以站立
        public bool CanUnDuck
        {
            get
            {
                if (!Ducking)
                    return true;
                Rect lastCollider = this.collider;
                this.collider = normalHitbox;
                bool noCollide = !CollideCheck(this.Position, Vector2.zero);
                this.collider = lastCollider;
                return noCollide;
            }
        }

        public bool IsFall
        {
            get
            {
                return !this.wasOnGround && this.OnGround;
            }
        }
    }

    #region WallSlide
    
    #endregion
}