using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 会在场景上创建一个不会过场景移除的自己的单例GameObject
/// </summary>
/// <typeparam name="T"></typeparam>
public class SingleToAutoMono<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject obj = new GameObject(typeof(T).ToString());
                instance = obj.AddComponent<T>();
                DontDestroyOnLoad(obj);
            }
            return instance;
        }
    }
}
