
using System.Collections;
using UnityEngine;

namespace Myd.Platform
{
    enum EGameState
    {
        Load,   //加载中
        Play,   //游戏中
        Pause,  //游戏暂停
        Fail,   //游戏失败
    }
    public class Game : MonoBehaviour
    {
        public static Game Instance;
        //玩家
        Player player;
        //游戏场景
        GameScene gameScene;

        EGameState gameState;

        void Awake()
        {
            Instance = this;
            gameState = EGameState.Load;

            gameScene = new GameScene();
            player = new Player(gameScene);
        }

        IEnumerator Start()
        {
            //加载场景
            gameScene.Reload();
            yield return null;
            //加载玩家
            player.Reload();
            this.gameState = EGameState.Play;
            yield return null;
        }

        public void Update()
        {
            float deltaTime = Time.unscaledDeltaTime;
            if (UpdateTime(deltaTime))
            {
                if (this.gameState == EGameState.Play)
                {
                    GameInput.Update(deltaTime);
                    //更新玩家逻辑数据
                    player.Update(deltaTime);
                    //更新场景逻辑数据
                    gameScene.Update(deltaTime);
                }
            }
            //if (FreezeTimer > 0f)
            //{
            //    Game.FreezeTimer = Mathf.Max(Game.FreezeTimer - deltaTime, 0f);
            //}
            //else
            //{
            //    Time.timeScale = 1;
            //    if (this.gameState == EGameState.Play)
            //    {
            //        GameInput.Update(deltaTime);
            //        //更新玩家逻辑数据
            //        player.Update(deltaTime);
            //        //更新场景逻辑数据
            //        gameScene.Update(deltaTime);
            //    }
            //}
        }

        #region 冻帧
        private float freezeTime;

        //更新顿帧数据，如果不顿帧，返回true
        public bool UpdateTime(float deltaTime)
        {
            if (freezeTime > 0f)
            {
                freezeTime = Mathf.Max(freezeTime - deltaTime, 0f);
                return false;
            }
            if (Time.timeScale == 0)
            {
                Time.timeScale = 1;
            }
            return true;
        }

        //冻帧
        public void Freeze(float freezeTime)
        {
            this.freezeTime = Mathf.Max(this.freezeTime, freezeTime);
            if (this.freezeTime > 0)
            {
                Time.timeScale = 0;
            }
            else
            {
                Time.timeScale = 1;
            }
        }
        #endregion
    }

}
