using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public abstract class BaseManger<T> where T : class//, new()
{
    private static T instance;
    protected static readonly object lockObj = new object();
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                lock (lockObj)
                {
                    if (instance == null)
                    {//抽象类保证外部不能去新建对象，利用反射得到子类的私有构造函数从并创建其单例
                     //下面函数为反射知识点需多家回顾
                        Type type = typeof(T);
                        ConstructorInfo info = type.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic,
                                                                   null,
                                                                   Type.EmptyTypes,
                                                                   null);
                        if (info != null) { instance = info.Invoke(null) as T; }
                        else Debug.LogError("不存在私有构造函数");
                    }
                }
            }
            return instance;
        }
    }

}
