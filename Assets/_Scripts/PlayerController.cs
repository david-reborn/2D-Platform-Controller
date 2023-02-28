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

        Vector2 size;
        float jumpGraceTimer;
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

        public float WallSlideTimer { get; set; } = Constants.WallSlideTime;
        public int WallSlideDir { get; set; }

        private bool onGround;
        private bool wasOnGround;

        public int ForceMoveX { get; set; }
        public float ForceMoveXTimer { get; set; }

        public int HopWaitX;   // If you climb hop onto a moving solid, snap to beside it until you get above it
        public float HopWaitXSpeed;

        public bool WallBoosting;
        public int wallBoostDir;
        public float wallBoostTimer;   // If you climb jump and then do a sideways input within this timer, switch to wall jump

        private FiniteStateMachine<BaseActionState> stateMachine;

        public SpriteRenderer Renderer { get; set; }
        private ControllerParams controllerParams;
        private Player player;
        public PlayerController(Player player, ControllerParams controllerParams)
        {
            this.player = player;
            ResetControllerParams(controllerParams);
            //TODO 临时方案
            this.Renderer = player.SpriteRenderer;

            this.stateMachine = new FiniteStateMachine<BaseActionState>((int)EActionState.Size);
            this.stateMachine.AddState(new NormalState(this));
            this.stateMachine.AddState(new DashState(this));
            this.stateMachine.AddState(new ClimbState(this));
            this.GroundMask = LayerMask.GetMask("Ground");

            this.LastAim = Vector2.right;
            this.Facing = Facings.Right;
        }

        public void ResetControllerParams(ControllerParams controllerParams)
        {
            this.controllerParams = controllerParams;
        }

        public void Init(Vector2 position)
        {
            //根据进入的方式,决定初始状态
            this.stateMachine.State = (int)EActionState.Normal;
            this.dashes = 1;
            this.Position = position;
            this.collider = normalHitbox;

            Color color = NormalHairColor;
            Gradient gradient = new Gradient();
            gradient.SetKeys(
                new GradientColorKey[] { new GradientColorKey(color, 0.0f), new GradientColorKey(color, 1.0f) },
                new GradientAlphaKey[] { new GradientAlphaKey(1, 0.0f), new GradientAlphaKey(1, 0.6f), new GradientAlphaKey(0, 1.0f) }
            );

            this.player.SetTrailColor(gradient);

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
                //TODO Highest Air Y
                //TODO Flashing
                //Wall Slide
                if (this.WallSlideDir != 0)
                {
                    this.WallSlideTimer = Math.Max(this.WallSlideTimer - deltaTime, 0);
                    this.WallSlideDir = 0;
                }

                //Wall Boost, 不消耗体力WallJump
                if (wallBoostTimer > 0)
                {
                    wallBoostTimer -= deltaTime;
                    if (moveX == wallBoostDir)
                    {
                        this.Speed.x = Constants.WallJumpHSpeed * moveX;
                        wallBoostTimer = 0;
                    }
                }
                //After Dash
                if (this.onGround && this.stateMachine.State != (int)EActionState.Climb)
                {
                    //AutoJump = false;
                    //Stamina = ClimbMaxStamina;
                    this.WallSlideTimer = Constants.WallSlideTime;
                }

                //Dash Attack

                //Jump Grace
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

                //撞墙以后的速度保持，Wall Speed Retention
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
                LastAim = Input.GetAimVector(Facing);

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
            //Physics
            //if (StateMachine.State != StDreamDash && StateMachine.State != StAttract)
            //    MoveH(Speed.X * Engine.DeltaTime, onCollideH);
            //if (StateMachine.State != StDreamDash && StateMachine.State != StAttract)
            //    MoveV(Speed.Y * Engine.DeltaTime, onCollideV);

            //更新
            UpdateSprite(deltaTime);


            UpdateHair(deltaTime);
        }

        private Color hairColor;
        private float hairFlashTimer;
        //Gradient gradient = new Gradient();
        
        private void SetHairColor(Color color)
        {
            Gradient gradient = new Gradient();
            gradient.SetKeys(
                new GradientColorKey[] { new GradientColorKey(color, 0.0f), new GradientColorKey(color, 1.0f) },
                new GradientAlphaKey[] { new GradientAlphaKey(1, 0.0f), new GradientAlphaKey(1, 0.6f), new GradientAlphaKey(0, 1.0f) }
            );
            this.player.SetTrailColor(gradient);
        }
        private void UpdateHair(float deltaTime)
        {
            if (this.dashes == 0 && this.dashes < MaxDashes)
            {
                hairColor = Color.Lerp(hairColor, UsedHairColor, 6f * deltaTime);
                SetHairColor(hairColor);
            }
            else
            {
                Color color;
                if (this.lastDashes != this.dashes)
                {
                    color = FlashHairColor;
                    hairFlashTimer = .12f;
                }
                else if (hairFlashTimer > 0)
                {
                    color = FlashHairColor;
                    hairFlashTimer -= deltaTime;
                }
                else if (this.dashes == 2)
                    color = Color.black;//TwoDashesHairColor;
                else
                    color = NormalHairColor;

                this.hairColor = color;
                SetHairColor(hairColor);
            }
            lastDashes = dashes;
        }

        public void Render()
        {
        }

        //处理跳跃,跳跃时候，会给跳跃前方一个额外的速度
        public void Jump()
        {
            Input.Jump.ConsumeBuffer();
            this.jumpGraceTimer = 0;
            this.WallSlideTimer = Constants.WallSlideTime;

            this.varJumpTimer = Constants.VarJumpTime;
            this.Speed.x += Constants.JumpHBoost * moveX;
            this.Speed.y = Constants.JumpSpeed;
            //Speed += LiftBoost;
            this.varJumpSpeed = this.Speed.y;

            this.Scale = new Vector2(.6f, 1.4f);

            this.player.PlayJumpEffect();

            //蹬墙的粒子效果
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
            jumpGraceTimer = 0;
            varJumpTimer = Constants.VarJumpTime;
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

            this.Scale = new Vector2(.6f, 1.4f);

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
            if (moveX == 0)
            {
                this.wallBoostDir = -(int)Facing;
                this.wallBoostTimer = Constants.ClimbJumpBoostTime;
            }
        }

        //在墙边Dash时，当前按住上，不按左右时，执行SuperWallJump
        public void SuperWallJump(int dir)
        {
            Input.Jump.ConsumeBuffer();
            Ducking = false;
            jumpGraceTimer = 0;
            varJumpTimer = Constants.SuperWallJumpVarTime;
            WallSlideTimer = Constants.WallSlideTime;
            //WallBoostTimer = 0;

            //TODO 考虑电梯对速度的加成
            Speed.x = Constants.SuperWallJumpH * dir;
            Speed.y = Constants.SuperWallJumpSpeed;
            //Speed += LiftBoost;
            varJumpSpeed = Speed.y;

            this.Scale = new Vector2(.6f, 1.4f);
        }

        //反弹
        public  void Bounce(float flowY)
        {

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
        public Vector2 Scale { get; set; }

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
                    this.CurrSpriteScale = DUCK_SPRITE_SCALE;
                    return;
                }
                else
                {
                    this.collider = this.normalHitbox;
                    this.CurrSpriteScale = NORMAL_SPRITE_SCALE;
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
}