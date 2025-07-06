using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 类似配置表 避免人为的拼写错误 用时在枚举中记录即可
/// </summary>
public enum E_EventType
{
    E_SceneLoad,
    E_Input_Horizontal,
    E_Input_Vertical,
    E_Input_Skill1,
    E_Input_Skill2,
    #region 世界时间相关
    E_WorldClock_NewYear,
    E_WorldClock_NewMonth,

    #endregion
    #region 自营企业相关
    //自营企业创立
    E_GameSelfEmpoled_SetUp,
    //自营企业分红
    E_GameSelfEmpoled_Dividends,
    #endregion
}
