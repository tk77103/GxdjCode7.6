using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class SceneMgr : BaseManger<SceneMgr>
{
    #region  ����
    private SceneMgr() { }
    #region ͬ���л�����
    public void LoadScene(string sceneName,UnityAction callBack=null)
    {
        SceneManager.LoadScene(sceneName);
        callBack?.Invoke();
    }
    #endregion
    #region �첽�л�����
    public void LoadSceneAsync(string sceneName, UnityAction callBack = null)
    {
       MonoMgr.Instance.StartCoroutine(ReallyLoadSceneAsync(sceneName,callBack));
    }
    private IEnumerator ReallyLoadSceneAsync(string sceneName, UnityAction callBack)
    {
        AsyncOperation ao= SceneManager.LoadSceneAsync(sceneName);
        //ÿ֡����Ƿ񳡾��������
        while (!ao.isDone) {
            //�ڴ˴������¼����Ľ����ȷ��ͳ�
            EventCenter.Instance.EventTrigger<float>(E_EventType.E_SceneLoad,ao.progress);
            yield return 0;
        }
        //��������ع���û�����ü�������1����ȥ
        EventCenter.Instance.EventTrigger<float>(E_EventType.E_SceneLoad, 1);
        callBack?.Invoke();
    }
    #endregion
    #region ��ȡ�첽���ؽ���

    #endregion
    #endregion

}
