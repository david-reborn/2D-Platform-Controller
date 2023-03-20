using System.Collections;
using UnityEngine;

namespace Myd.Common
{
    public class AssetHelper
    {
        public static GameObject CreateGameObject(string prefabPath)
        {
            GameObject prefab = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            return GameObject.Instantiate(prefab);
        }
    }
}