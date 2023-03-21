

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
        //玩家
        Player player;
        //游戏场景
        GameScene gameScene;

        EGameState gameState;

        void Awake()
        {
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
            float deltaTime = Time.deltaTime;

            if (this.gameState == EGameState.Play) {
                GameInput.Update(deltaTime);
                //更新玩家逻辑数据
                player.Update(deltaTime);
                //更新场景逻辑数据
                gameScene.Update(deltaTime);
            }
        }

    }

}
