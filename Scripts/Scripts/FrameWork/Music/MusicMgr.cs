using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MusicMgr : BaseManger<MusicMgr>
{
    #region 数据容器及构造函数
    //背景音乐播放组件
    private AudioSource bkMusic = null;
    private float bkMusicValue = 0.1f;
    private float soundValue = 0.1f;
    //用于音效组件依附的对象

    //管理正在播放的音效
    private List<AudioSource> soundList = new List<AudioSource>();
    //音效是否在播放
    private bool soundIsPlay = true;
    private MusicMgr()
    {
        MonoMgr.Instance.AddUpdateListener(Update);
    }
    /// <summary>
    /// 移除正在播放的音效
    /// </summary>
    private void Update()
    {
        if (!soundIsPlay)
            return;
        for (int i = soundList.Count - 1; i >= 0; i--)
        {
            if (!soundList[i].isPlaying)
            {
                soundList[i].clip = null;
                PoolMgr.Instance.PushObj(soundList[i].gameObject);
                soundList.RemoveAt(i);
            }
        }
    }
    #endregion
    #region 背景音乐相关
    #region 播放背景音乐
    public void PlayBKMusic(string name)
    {
        //动态创建背景音乐组件并在过场景时不移除 背景音乐唯一
        if (bkMusic == null)
        {
            GameObject obj = new GameObject();
            obj.name = "BKMusic";
            GameObject.DontDestroyOnLoad(obj);
            bkMusic = obj.AddComponent<AudioSource>();
        }
        ABMgr.Instance.LoadResAsync<AudioClip>("music", name, (clip) =>
        {
            bkMusic.clip = clip;
            bkMusic.loop = true;
            bkMusic.volume = bkMusicValue;
            bkMusic.Play();
        }, true);
    }
    #endregion
    #region 停止背景音乐
    public void StopBKMusic()
    {
        if (bkMusic == null)
            return;
        bkMusic.Stop();
    }
    #endregion
    #region 暂停背景音乐
    public void PauseBKMusic()
    {
        if (bkMusic == null)
            return;
        bkMusic.Pause();
    }
    #endregion
    #region 设置背景音乐大小
    public void ChangeBKMusic(float v)
    {
        bkMusicValue = v;
        if (bkMusic == null)
            return;
        bkMusic.volume = v;
    }
    #endregion
    #endregion
    #region 音效相关
    #region 播放音效
    /// <summary>
    /// 播放音效
    /// </summary>
    /// <param name="name">音效切片文件名</param>
    /// <param name="isAsync">是否异步加载</param>
    /// <param name="isLoop">音效是否循环</param>
    /// <param name="callBack">加载结束后的回调</param>
    public void PlaySound(string name, bool isAsync = false, bool isLoop = false, UnityAction<AudioSource> callBack = null)
    {  
        ABMgr.Instance.LoadResAsync<AudioClip>("sound", name, (clip) =>
        {
            AudioSource source =PoolMgr.Instance.GetObj("Sound/SoundObj").GetComponent<AudioSource>();
            source.Stop();
            source.clip = clip;
            source.loop = isLoop;
            source.volume = soundValue;
            source.Play();
            //存储音效播放器 用于记录 方便判断是否停止
            if(!soundList.Contains(source))
            soundList.Add(source);
            //传递给外部使用
            callBack?.Invoke(source);
        }, isAsync);
    }
    #endregion
    #region 停止播放指定音效
    public void StopSound(AudioSource source)
    {
        if (soundList.Contains(source))
        {
            source.Stop();
            soundList.Remove(source);
           source.clip=null;
            PoolMgr.Instance.PushObj(source.gameObject);
        }
    }
    #endregion
    #region 改变音效的大小
    public void ChangeSoundValeue(float v)
    {
        soundValue = v;
        for (int i = 0; i < soundList.Count; i++)
        {
            soundList[i].volume = v;
        }
    }
    #endregion
    #region 播放或暂停一个音效
    /// <summary>
    /// 继续播放或者暂停所有音效
    /// </summary>
    /// <param name="isPlay"></param>
    public void PlayOrPauseSound(bool isPlay)
    {
        if (isPlay)
        {
            soundIsPlay = true;
            for (int i = 0; i < soundList.Count; i++)
            {
                soundList[i].Play();

            }
        }
        else
        {
            soundIsPlay = false;
            for (int i = 0; i < soundList.Count; i++)
            {
                soundList[i].Pause();
            }
        }
    }
    /// <summary>
    /// 清空音效相关记录 过场景时候在清空缓存池之前去调用它
    /// </summary>
    public void ClearSound()
    {
        for (int i=0;i<soundList.Count;i++)
        {
            soundList[i].Stop();
            soundList[i].clip = null;
            PoolMgr.Instance.PushObj(soundList[i].gameObject);
        }
        soundList.Clear();
    }
    #endregion
    #endregion
}
