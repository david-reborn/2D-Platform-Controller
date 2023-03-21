

using Myd.Common;
using Myd.Platform;
using Myd.Platform.Core;
using UnityEngine;

namespace Myd.Platform
{
    /// <summary>
    /// 玩家类：包含
    /// 1、玩家显示器
    /// 2、玩家控制器（核心控制器）
    /// 并允许两者在内部进行交互
    /// </summary>
    public class Player : IPlayerContext
    {
        private PlayerRenderer playerRenderer;
        private PlayerController playerController;

        private GameScene gameScene;

        public Player(GameScene gameScene)
        {
            this.gameScene = gameScene;
        }

        //加载玩家实体
        public void Reload()
        {
            this.playerRenderer = AssetHelper.Create<PlayerRenderer>("Assets/_Prefabs/PlayerRenderer.prefab");
            this.playerRenderer.Reload();
            //初始化
            this.playerController = new PlayerController(this, playerRenderer, SceneEffectManager.Instance);
            this.playerController.Init(new Vector2(-36, 2));

            PlayerParams playerParams = AssetHelper.LoadObject<PlayerParams>("Assets/PlayerParam.asset");
            playerParams.SetReloadCallback(() => this.playerController.RefreshAbility());
            playerParams.ReloadParams();
        }

        public void Update(float deltaTime)
        {
            playerController.Update(deltaTime);
            Render();
        }

        private void Render()
        {
            playerRenderer.Render(Time.deltaTime);

            Vector2 scale = playerRenderer.transform.localScale;
            scale.x = Mathf.Abs(scale.x) * (int)playerController.Facing;
            playerRenderer.transform.localScale = scale;
            playerRenderer.transform.position = playerController.Position;

            //if (!lastFrameOnGround && this.playerController.OnGround)
            //{
            //    this.playerRenderer.PlayMoveEffect(true, this.playerController.GroundColor);
            //}
            //else if (lastFrameOnGround && !this.playerController.OnGround)
            //{
            //    this.playerRenderer.PlayMoveEffect(false, this.playerController.GroundColor);
            //}
            //this.playerRenderer.UpdateMoveEffect();

            this.lastFrameOnGround = this.playerController.OnGround;
        }

        private bool lastFrameOnGround;
    }

}
