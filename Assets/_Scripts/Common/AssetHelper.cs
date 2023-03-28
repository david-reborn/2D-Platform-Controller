using System.Collections;
using UnityEngine;

namespace Myd.Common
{
    public static class AssetHelper
    {
        public static GameObject CreateGameObject(string prefabPath)
        {
            GameObject prefab = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            return GameObject.Instantiate(prefab);
        }

        public static T LoadObject<T>(string path) where T : Object
        {
            T obj = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(path);
            return obj; 
        }

        public static T Create<T>(string prefabPath) where T : Object
        {
            T prefab = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(prefabPath);
            return Object.Instantiate<T>(prefab);
        }

        public static Object[] LoadAllObject(string path)
        {
            return UnityEditor.AssetDatabase.LoadAllAssetsAtPath(path);
        }

        public static void Destroy(GameObject go)
        {
            GameObject.Destroy(go);
        }
    }
}