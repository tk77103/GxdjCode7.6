using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcGowthAI : BaseManger<NpcGowthAI>
{
    #region ��������
    System.Random r = new();
    #endregion
    #region ����
    private NpcGowthAI() { }
    #region ʱ���ݻ�����npc��������

    public void GenerateSkill(NpcBase npc)
    {
        //npc.myJobSkill.skillPoint = (int)npc.intellegence;
        switch (npc.companyAcademy)
        {
            case "��":
                break;
            case "�Ĵ�":
                break;
            case "����":
                break;
            case "��ҵ":
                break;
            case "����":
                break;
            case "����":
                break;
        }
    }
    #endregion
    #region npc���ܳɳ�(�����ж�)
    public void NpcIntelligentGrow()
    {
        foreach (var npc in WorldSceneMgr.Instance.worldAllNpc.Values)
        {
            npc.npcAge++;
            if (npc.npcAge > 40)
            {
                switch (npc.growType)
                {
                    case 1:
                    case 3:
                        if (r.Next(1, 101) <= 10)
                            npc.isStopGrow = true;
                        break;
                    case 2:
                        if (r.Next(1, 101) <= 5)
                            npc.isStopGrow = true;
                        break;
                }
            }
            if (!npc.isStopGrow)
            {
                switch (npc.growType)
                {
                    case 1:
                        npc.intellegence += 0.5f;
                        break;
                    case 2:
                        npc.intellegence += 1;
                        break;
                    case 3:
                        npc.intellegence += r.Next(1, 3) > 1 ? 2 : 1;
                        break;
                }
            }
            if (npc.industry == "�����")
                continue;
                DegreeUp(npc);
        }
    }
    #endregion
    #region ѧλ���� ����Ϊ��ʿʱ�л�����룩
    public void DegreeUp(NpcBase npc)
    {
        if (npc.degree >= Degree.��ʿ)
            return;
        else
        {
            if (r.Next(1, 101) <= 5)
            {
                switch (npc.degree)
                {
                    case Degree.����:
                        if (npc.intellegence > 10)
                        {
                            npc.degree = Degree.����;
                            npc.myJobSkill.skillPoint += 10;
                        }
                        break;
                    case Degree.����:
                        if (npc.intellegence > 20)
                        {
                            npc.degree = Degree.ѧʿ;
                            npc.myJobSkill.skillPoint += 20;
                        }
                        break;
                    case Degree.ѧʿ:
                        if (npc.intellegence > 30)
                        {
                            npc.degree = Degree.˶ʿ;
                            npc.myJobSkill.skillPoint += 50;
                        }
                        break;
                    case Degree.˶ʿ:
                        if (npc.intellegence > 40)
                        {
                            npc.degree = Degree.��ʿ;
                            npc.myJobSkill.skillPoint += 100;
                        }
                        break;
                }
            }

        }
    }
    #endregion
    #endregion
}
