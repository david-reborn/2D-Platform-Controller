using Myd.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Myd.Platform
{
    /// <summary>
    /// 当前场景关卡
    /// 包含当前关卡实体和特效
    /// </summary>
    public class GameLevelScene
    {
        //场景特效管理器
        private SceneEffectManager sceneEffectManager;

        public void Reload()
        {
            if (sceneEffectManager != null)
            {
                AssetHelper.Destroy(sceneEffectManager.gameObject);
            }
            sceneEffectManager = AssetHelper.Create<SceneEffectManager>("Assets/_Prefabs/SceneEffectManager.prefab");
            sceneEffectManager.Reload();
        }

        public void Update(float deltaTime)
        {

        }
    }
}
