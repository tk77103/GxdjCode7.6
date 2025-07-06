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
                    {//�����ౣ֤�ⲿ����ȥ�½��������÷���õ������˽�й��캯���Ӳ������䵥��
                     //���溯��Ϊ����֪ʶ�����һع�
                        Type type = typeof(T);
                        ConstructorInfo info = type.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic,
                                                                   null,
                                                                   Type.EmptyTypes,
                                                                   null);
                        if (info != null) { instance = info.Invoke(null) as T; }
                        else Debug.LogError("������˽�й��캯��");
                    }
                }
            }
            return instance;
        }
    }

}
