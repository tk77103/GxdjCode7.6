using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class InputInfo
{
    #region ��������
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
    #region ����
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
/// ������Ϣ ����������ԣ������ظ�����
/// </summary>
public class InputMgr : BaseManger<InputMgr>
{
    #region ��������
    private Dictionary<E_EventType, InputInfo> inputDic = new Dictionary<E_EventType, InputInfo>();
    //��ǰ����ʱȡ����������Ϣ  �������ⲿ�����ظ�������Լ����
    private InputInfo nowInputInfo;
    private bool isStart;
    //�����ڸļ�ʱ��ȡ������Ϣ��ί�� ��update��ȡ��Ϣʱ��ͨ��ί�д��ݸ��ⲿ
    private UnityAction<InputInfo> getInputInfoCallBack;
    //�Ƿ�ʼ��ȡ��λ
    private bool isBeginCheckInput=false;
    #endregion
    #region ����
    //���캯���ڴ˴η�װһ��mono�ű���update�м������
    private InputMgr()
    {
        MonoMgr.Instance.AddUpdateListener(InputUpdate);
    }
    #region ����������
    public void StartOrCloseInputMgr(bool isStart)
    {
        this.isStart = isStart;
    }
    #endregion
    #region �ļ�����
    #region �ṩ���ⲿ�ļ����ʼ���ķ���
    public void ChangeKeyboardInfo(E_EventType eventType, KeyCode keyCode, InputInfo.E_InputType inputType)
    {//��ʼ��
        if (!inputDic.ContainsKey(eventType))
        {
            inputDic.Add(eventType, new InputInfo(inputType, keyCode));
        }
        //�ֵ��д����¼�Ҳ���ǽ��иĽ�
        else
        {
            inputDic[eventType].keyOrMouse = InputInfo.E_KeyOrMouse.Key;
            inputDic[eventType].inputType = inputType;
            inputDic[eventType].key = keyCode;

        }
    }
    #endregion
    #region �ṩ���ⲿ��������û��ʼ���ķ���
    public void ChangeMouseInfo(E_EventType eventType, int mouseID, InputInfo.E_InputType inputType)
    {//��ʼ��
        if (!inputDic.ContainsKey(eventType))
        {
            inputDic.Add(eventType, new InputInfo(inputType, mouseID));
        }
        //�ֵ��д����¼�Ҳ���ǽ��иĽ�
        else
        {
            inputDic[eventType].keyOrMouse = InputInfo.E_KeyOrMouse.Mouse;
            inputDic[eventType].inputType = inputType;
            inputDic[eventType].mouseID = mouseID;

        }
    }
    #endregion
    #endregion
    #region ��ȡ��һ�ε�������Ϣ
    /// <summary>
    /// ��update���߼��޷�ÿ֡ȥ������Ϣ���ڼ�⵽��ͨ��ί�н���Ϣ����
    /// �ӳ�һ֡��ȡ ������������ô˷���ʱ��������һֱ֡�ӻ�ȡ���루���������������ֻ�ǻ�ȡ��������޷���ú�������
    /// �˴����������ȴ�ֱ����ȡ����������Ҫ�ļ�
    /// </summary>
    /// <param name="callBack"></param>
    public void GetInputInfo(UnityAction<InputInfo> callBack)
    {
        getInputInfoCallBack = callBack;
        MonoMgr.Instance.StartCoroutine(BeginCheakInput());
    }
    private IEnumerator BeginCheakInput()
    {//��һ֡
        yield return 0;
        isBeginCheckInput = true;
    }
    #endregion
    #region ��ÿ֡�������������
    private void InputUpdate()
    {//��ί�в�Ϊ��ʱ�� ����Ϣ���ݸ��ⲿ
        if (isBeginCheckInput)
        {
            if (Input.anyKeyDown)
            {
                InputInfo inputInfo = null;
                //�������м�λ�İ������õ���Ӧ�������Ϣ
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
                //��ί�е��ú�ȡ�����
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
    #region �Ƴ�������
    public void RemoveInputInfo(E_EventType eventType)
    {
        if (inputDic.ContainsKey(eventType))
            inputDic.Remove(eventType);
        else return;
    }
    #endregion
    #endregion


}
