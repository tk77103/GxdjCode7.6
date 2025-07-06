using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class SceneMgr : BaseManger<SceneMgr>
{
    #region  函数
    private SceneMgr() { }
    #region 同步切换场景
    public void LoadScene(string sceneName,UnityAction callBack=null)
    {
        SceneManager.LoadScene(sceneName);
        callBack?.Invoke();
    }
    #endregion
    #region 异步切换场景
    public void LoadSceneAsync(string sceneName, UnityAction callBack = null)
    {
       MonoMgr.Instance.StartCoroutine(ReallyLoadSceneAsync(sceneName,callBack));
    }
    private IEnumerator ReallyLoadSceneAsync(string sceneName, UnityAction callBack)
    {
        AsyncOperation ao= SceneManager.LoadSceneAsync(sceneName);
        //每帧检测是否场景加载完毕
        while (!ao.isDone) {
            //在此次利用事件中心将进度发送出
            EventCenter.Instance.EventTrigger<float>(E_EventType.E_SceneLoad,ao.progress);
            yield return 0;
        }
        //避免因加载过快没有来得及将进度1传出去
        EventCenter.Instance.EventTrigger<float>(E_EventType.E_SceneLoad, 1);
        callBack?.Invoke();
    }
    #endregion
    #region 获取异步加载进度

    #endregion
    #endregion

}
