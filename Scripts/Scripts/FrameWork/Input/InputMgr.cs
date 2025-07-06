using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class InputInfo
{
    #region 数据容器
    public enum E_KeyOrMouse
    {
        Key,
        Mouse
    }
    public enum E_InputType
    {
        Down,
        Up,
        Hold
    }
    public E_KeyOrMouse keyOrMouse;
    public E_InputType inputType;
    public KeyCode key;
    public int mouseID;
    #endregion
    #region 函数
    public InputInfo(E_InputType inputType, KeyCode key)
    {
        this.keyOrMouse = E_KeyOrMouse.Key;
        this.inputType = inputType;
        this.key = key;
    }
    public InputInfo(E_InputType inputType, int mouseID)
    {
        this.keyOrMouse = E_KeyOrMouse.Mouse;
        this.inputType = inputType;
        this.mouseID = mouseID;
    }
    #endregion
}
/// <summary>
/// 采用消息 降低了耦合性，避免重复代码
/// </summary>
public class InputMgr : BaseManger<InputMgr>
{
    #region 数据容器
    private Dictionary<E_EventType, InputInfo> inputDic = new Dictionary<E_EventType, InputInfo>();
    //当前遍历时取出的输入信息  声明在外部避免重复声明节约性能
    private InputInfo nowInputInfo;
    private bool isStart;
    //用于在改键时获取输入信息的委托 在update获取信息时再通过委托传递给外部
    private UnityAction<InputInfo> getInputInfoCallBack;
    //是否开始获取键位
    private bool isBeginCheckInput=false;
    #endregion
    #region 函数
    //构造函数在此次封装一个mono脚本在update中检测输入
    private InputMgr()
    {
        MonoMgr.Instance.AddUpdateListener(InputUpdate);
    }
    #region 开关输入检测
    public void StartOrCloseInputMgr(bool isStart)
    {
        this.isStart = isStart;
    }
    #endregion
    #region 改键方法
    #region 提供给外部改键或初始化的方法
    public void ChangeKeyboardInfo(E_EventType eventType, KeyCode keyCode, InputInfo.E_InputType inputType)
    {//初始化
        if (!inputDic.ContainsKey(eventType))
        {
            inputDic.Add(eventType, new InputInfo(inputType, keyCode));
        }
        //字典中存在事件也就是进行改建
        else
        {
            inputDic[eventType].keyOrMouse = InputInfo.E_KeyOrMouse.Key;
            inputDic[eventType].inputType = inputType;
            inputDic[eventType].key = keyCode;

        }
    }
    #endregion
    #region 提供给外部改鼠标设置或初始化的方法
    public void ChangeMouseInfo(E_EventType eventType, int mouseID, InputInfo.E_InputType inputType)
    {//初始化
        if (!inputDic.ContainsKey(eventType))
        {
            inputDic.Add(eventType, new InputInfo(inputType, mouseID));
        }
        //字典中存在事件也就是进行改建
        else
        {
            inputDic[eventType].keyOrMouse = InputInfo.E_KeyOrMouse.Mouse;
            inputDic[eventType].inputType = inputType;
            inputDic[eventType].mouseID = mouseID;

        }
    }
    #endregion
    #endregion
    #region 获取下一次的输入信息
    /// <summary>
    /// 在update中逻辑无法每帧去给出信息，在检测到后通过委托将信息传出
    /// 延迟一帧获取 当点击换键调用此方法时可能在这一帧直接获取输入（比如鼠标左键点击就只是获取鼠标点击，无法获得后续按键
    /// 此处我们让他等待直到获取我们真正想要的键
    /// </summary>
    /// <param name="callBack"></param>
    public void GetInputInfo(UnityAction<InputInfo> callBack)
    {
        getInputInfoCallBack = callBack;
        MonoMgr.Instance.StartCoroutine(BeginCheakInput());
    }
    private IEnumerator BeginCheakInput()
    {//等一帧
        yield return 0;
        isBeginCheckInput = true;
    }
    #endregion
    #region 在每帧检测键盘鼠标输入
    private void InputUpdate()
    {//当委托不为空时将 键信息传递给外部
        if (isBeginCheckInput)
        {
            if (Input.anyKeyDown)
            {
                InputInfo inputInfo = null;
                //遍历所有键位的按下来得到对应输入的信息
                Array keyCodes = Enum.GetValues(typeof(KeyCode));
                foreach (KeyCode inputKey in keyCodes)
                {

                    if (Input.GetKeyDown(inputKey))
                    {
                        inputInfo = new InputInfo(InputInfo.E_InputType.Down, inputKey);
                        break;
                    }
                }
                for (int i = 0; i < 3; i++)
                {
                    if (Input.GetMouseButtonDown(i))
                    {
                        inputInfo = new InputInfo(InputInfo.E_InputType.Down, i);
                        break;
                    }
                }
                getInputInfoCallBack.Invoke(inputInfo);
                getInputInfoCallBack = null;
                //在委托调用后取消检测
                isBeginCheckInput = false;
            }
        }

        if (!isStart) return;
        foreach (E_EventType eventType in inputDic.Keys)
        {
            nowInputInfo = inputDic[eventType];
            if (nowInputInfo.keyOrMouse == InputInfo.E_KeyOrMouse.Key)
            {
                switch (nowInputInfo.inputType)
                {
                    case InputInfo.E_InputType.Down:
                        if (Input.GetKeyDown(nowInputInfo.key))
                            EventCenter.Instance.EventTrigger(eventType);
                        break;
                    case InputInfo.E_InputType.Up:
                        if (Input.GetKeyUp(nowInputInfo.key))
                            EventCenter.Instance.EventTrigger(eventType);
                        break;
                    case InputInfo.E_InputType.Hold:
                        if (Input.GetKey(nowInputInfo.key))
                            EventCenter.Instance.EventTrigger(eventType);
                        break;
                    default:
                        break;
                }
            }
            else
            {
                switch (nowInputInfo.inputType)
                {
                    case InputInfo.E_InputType.Down:
                        if (Input.GetMouseButtonDown(nowInputInfo.mouseID))
                            EventCenter.Instance.EventTrigger(eventType);
                        break;
                    case InputInfo.E_InputType.Up:
                        if (Input.GetMouseButtonUp(nowInputInfo.mouseID))
                            EventCenter.Instance.EventTrigger(eventType);
                        break;
                    case InputInfo.E_InputType.Hold:
                        if (Input.GetMouseButton(nowInputInfo.mouseID))
                            EventCenter.Instance.EventTrigger(eventType);
                        break;
                    default:
                        break;
                }
            }
        }

        EventCenter.Instance.EventTrigger(E_EventType.E_Input_Horizontal, Input.GetAxis("Horizontal"));
        EventCenter.Instance.EventTrigger(E_EventType.E_Input_Vertical, Input.GetAxis("Vertical"));

    }
    #endregion
    #region 移除输入检测
    public void RemoveInputInfo(E_EventType eventType)
    {
        if (inputDic.ContainsKey(eventType))
            inputDic.Remove(eventType);
        else return;
    }
    #endregion
    #endregion


}
