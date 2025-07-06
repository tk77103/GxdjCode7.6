using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MathUtil
{
    #region 函数
    #region 弧度角度的转换
    #region 角度转弧度
    /// <summary>
    /// 弧度转角度的方法
    /// </summary>
    /// <param name="deg">角度</param>
    /// <returns></returns>
    public static float Deg2Rad(float deg)
    {
        return deg * Mathf.Deg2Rad;
    }
    #endregion
    #region 角度转弧度
    /// <summary>
    /// 角度转弧度
    /// </summary>
    /// <param name="rad">弧度</param>
    /// <returns></returns>
    public static float Rad2Deg(float rad)
    {
        return rad * Mathf.Rad2Deg;
    }
    #endregion
    #endregion
    #region 距离计算相关  
    #region XZ平面的距离
    /// <summary>
    /// 获取xz平面上俩点的距离
    /// </summary>
    /// <param name="srcPos">点1</param>
    /// <param name="targetPos">点2</param>
    /// <returns></returns>
    public static float GetObjDistanceXZ(Vector3 srcPos, Vector3 targetPos)
    {
        srcPos.y = 0;
        targetPos.y = 0;
        return Vector3.Distance(srcPos, targetPos);
    }
    /// <summary>
    /// 判断俩点在XZ平面距离是否小于目标距离
    /// </summary>
    /// <param name="srcPos"></param>
    /// <param name="targetPos"></param>
    /// <param name="distance"></param>
    /// <returns></returns>
    public static bool CheckObjDistanceXZ(Vector3 srcPos, Vector3 targetPos, float distance)
    {
        return GetObjDistanceXZ(srcPos, targetPos) <= distance;
    }
    #endregion
    #region XY平面的距离
    /// <summary>
    /// 获取xy平面上俩点的距离
    /// </summary>
    /// <param name="srcPos">点1</param>
    /// <param name="targetPos">点2</param>
    /// <returns></returns>
    public static float GetObjDistanceXY(Vector3 srcPos, Vector3 targetPos)
    {
        srcPos.z = 0;
        targetPos.z = 0;
        return Vector3.Distance(srcPos, targetPos);
    }
    /// <summary>
    /// 判断俩点在XY平面距离是否小于目标距离
    /// </summary>
    /// <param name="srcPos"></param>
    /// <param name="targetPos"></param>
    /// <param name="distance"></param>
    /// <returns></returns>
    public static bool CheckObjDistanceXY(Vector3 srcPos, Vector3 targetPos, float distance)
    {
        return GetObjDistanceXY(srcPos, targetPos) <= distance;
    }
    #endregion
    #endregion
    #region 位置判断相关
    #region 判断是否在屏幕外
    /// <summary>
    /// 判断世界坐标系下的某个点 是否在屏幕可见范围外
    /// </summary>
    /// <param name="pos">所要判断的位置</param>
    /// <returns></returns>如果在屏幕外返回true
    public static bool IsWorldPosOutScreen(Vector3 pos)
    {
        Vector3 screenPos = Camera.main.WorldToScreenPoint(pos);
        if (screenPos.x >= 0 && screenPos.x <= Screen.width &&
            screenPos.y >= 0 && screenPos.y <= Screen.height)
            return false;
        return true;
    }
    #endregion
    #region 判断是否在扇形范围内
    /// <summary>
    /// 判断某一位置是否在指定扇形范围内
    /// </summary>
    /// <param name="pos">扇形中心点</param>
    /// <param name="forward">对象面朝向</param>
    /// <param name="targetPos">目标位置</param>
    /// <param name="radius">扇形半径</param>
    /// <param name="angle">扇形的角度</param>
    /// <returns>true则在范围内</returns>
    public static bool IsInSectorRangeXZ(Vector3 pos, Vector3 forward, Vector3 targetPos,
        float radius, float angle)
    {
        pos.y = 0;
        forward.y = 0;
        targetPos.y = 0;
        return Vector3.Distance(pos, targetPos) <= radius
            && Vector3.Angle(forward, targetPos - pos) <= angle / 2f;
    }
    #endregion
    #endregion
    #region 射线检测相关
    #region 获取射线获取单个对象
    /// <summary>
    /// 射线检测获取一个对象 指定距离 指定层级
    /// </summary>
    /// <param name="ray">射线</param>
    /// <param name="callBack">获取到碰撞信息后传入hitInfo的回调</param>
    /// <param name="maxDistance">最大射线检测距离</param>
    /// <param name="layeMask">层级 int类型</param>
    public static void RayCast(Ray ray, UnityAction<RaycastHit> callBack, float maxDistance,
        int layeMask)
    {
        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo, maxDistance, layeMask))
            callBack.Invoke(hitInfo);
    }
    /// <summary>
    /// 射线检测获取一个对象 指定距离 指定层级
    /// </summary>
    /// <param name="ray">射线</param>
    /// <param name="callBack">获取到碰撞信息后传入gameObject上挂载的T类型脚本的回调</param>
    /// <param name="maxDistance">最大射线检测距离</param>
    /// <param name="layeMask">层级 int类型</param>
    public static void RayCast<T>(Ray ray, UnityAction<T> callBack, float maxDistance,
        int layeMask)
    {
        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo, maxDistance, layeMask))
            callBack.Invoke(hitInfo.collider.gameObject.GetComponent<T>());
        callBack.Invoke(hitInfo.collider.gameObject.GetComponent<T>());
    }
    #endregion
    #region 射线获取多个对象
    /// <summary>
    /// 射线检测获取多个对象 指定距离 指定层级
    /// </summary>
    /// <param name="ray">射线</param>
    /// <param name="callBack">获取到碰撞信息后传入hitInfo的回调 每个对象调用一次</param>
    /// <param name="maxDistance">最大射线检测距离</param>
    /// <param name="layeMask">层级 int类型</param>
    public static void RayCastAll(Ray ray, UnityAction<RaycastHit> callBack, float maxDistance,
        int layeMask)
    {
        RaycastHit[] hitInfo = Physics.RaycastAll(ray, maxDistance, layeMask);
        for (int i = 0; i < hitInfo.Length; i++)
            callBack.Invoke(hitInfo[i]);
    }
    /// <summary>
    /// 射线检测获取多个对象 指定距离 指定层级
    /// </summary>
    /// <param name="ray">射线</param>
    /// <param name="callBack">获取到碰撞信息后传入gameObject的回调 每个对象调用一次</param>
    /// <param name="maxDistance">最大射线检测距离</param>
    /// <param name="layeMask">层级 int类型</param>
    public static void RayCastAll(Ray ray, UnityAction<GameObject> callBack, float maxDistance,
        int layeMask)
    {
        RaycastHit[] hitInfo = Physics.RaycastAll(ray, maxDistance, layeMask);
        for (int i = 0; i < hitInfo.Length; i++)
            callBack.Invoke(hitInfo[i].collider.gameObject);
    }
    /// <summary>
    /// 射线检测获取多个对象 指定距离 指定层级
    /// </summary>
    /// <param name="ray">射线</param>
    /// <param name="callBack">获取到碰撞信息后传入gameObject挂载的T类型脚本的回调 每个对象调用一次</param>
    /// <param name="maxDistance">最大射线检测距离</param>
    /// <param name="layeMask">层级 int类型</param>
    public static void RayCastAll<T>(Ray ray, UnityAction<T> callBack, float maxDistance,
        int layeMask)
    {
        RaycastHit[] hitInfo = Physics.RaycastAll(ray, maxDistance, layeMask);
        for (int i = 0; i < hitInfo.Length; i++)
            callBack.Invoke(hitInfo[i].collider.gameObject.GetComponent<T>());
    }
    #endregion
    #endregion
    #region 范围检测相关
    /// <summary>
    /// 盒状范围检测
    /// </summary>
    /// <typeparam name="T">想获取的类型</typeparam>
    /// <param name="center">盒子中心位置</param>
    /// <param name="rotation">盒子2旋转角度</param>
    /// <param name="halfExtents">盒子长宽高的一半</param>
    /// <param name="layerMask">层级筛选</param>
    /// <param name="callBack">回调函数</param>
    public static void OverLapBox<T>(Vector3 center,Quaternion rotation,Vector3 halfExtents,
        int layerMask,UnityAction<T> callBack)where T :class
    {Type type = typeof(T);
        Collider[] colliders= Physics.OverlapBox(center, halfExtents, rotation, layerMask,QueryTriggerInteraction.Collide);
        for(int i = 0;i < colliders.Length; i++)
        {
            if (type ==typeof(Collider))
                callBack.Invoke(colliders[i]as T);
            else if (type == typeof(GameObject))
                callBack.Invoke(colliders[i].gameObject as T);
            else
                callBack.Invoke(colliders[i].gameObject.GetComponent<T>());
        }
    }
    /// <summary>
    /// 球体范围检测
    /// </summary>
    /// <typeparam name="T">想获取的类型</typeparam>
    /// <param name="center">球体中心位置</param>
    /// <param name="radius">球半径</param>
    /// <param name="layerMask">层级筛选</param>
    /// <param name="callBack">回调函数</param>
    public static void OverLapSphere<T>(Vector3 center, float radius, 
        int layerMask, UnityAction<T> callBack) where T : class
    {
        Type type = typeof(T);
        Collider[] colliders = Physics.OverlapSphere(center, radius, layerMask, QueryTriggerInteraction.Collide);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (type == typeof(Collider))
                callBack.Invoke(colliders[i] as T);
            else if (type == typeof(GameObject))
                callBack.Invoke(colliders[i].gameObject as T);
            else
                callBack.Invoke(colliders[i].gameObject.GetComponent<T>());
        }
    }
    #endregion
    #endregion
}
