using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���ڳ����ϴ���һ������������Ƴ����Լ��ĵ���GameObject
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
