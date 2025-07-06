using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// �������ñ� ������Ϊ��ƴд���� ��ʱ��ö���м�¼����
/// </summary>
public enum E_EventType
{
    E_SceneLoad,
    E_Input_Horizontal,
    E_Input_Vertical,
    E_Input_Skill1,
    E_Input_Skill2,
    #region ����ʱ�����
    E_WorldClock_NewYear,
    E_WorldClock_NewMonth,

    #endregion
    #region ��Ӫ��ҵ���
    //��Ӫ��ҵ����
    E_GameSelfEmpoled_SetUp,
    //��Ӫ��ҵ�ֺ�
    E_GameSelfEmpoled_Dividends,
    #endregion
}
