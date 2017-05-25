using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// TODO : Textにして読み込むように変更したい
public class AIData : MonoBehaviour
{
    #region Singleton

    private static AIData instance;

    public static AIData Instance
    {
        get
        {
            if (instance)
                return instance;

            instance = (AIData)FindObjectOfType(typeof(AIData));

            if (instance)
                return instance;

            GameObject obj = new GameObject("AIData");
            obj.AddComponent<AIData>();
            //Debug.Log(typeof(FieldDataChecker) + "が存在していないのに参照されたので生成");

            return instance;
        }
    }

    #endregion

    public struct RatioData
    {
        public int Wait;
        public int Move;
        public int Put;
        public int Break;

        public int SpecialWait;
        public int SpecialMove;
        public int SpecialPut;
        public int SpecialBreak;
    }

    RatioData[] _RatioDataWeak   = null;   
    RatioData[] _RatioDataNormal = null; 
    RatioData[] _RatioDataStrong = null;

    public struct RiskData
    {
        public int maxRisk;
        public int riskRange;
    };
    RiskData[] _RiskDataWeak = null;
    RiskData[] _RiskDataNormal = null;
    RiskData[] _RiskDataStrong = null;

    public void Set()
    {
        _RatioDataWeak   = new RatioData[(int)Charactor.eCharaType.MAX];
        _RatioDataNormal = new RatioData[(int)Charactor.eCharaType.MAX];
        _RatioDataStrong = new RatioData[(int)Charactor.eCharaType.MAX];
        _RiskDataWeak    = new RiskData [(int)Charactor.eCharaType.MAX];
        _RiskDataNormal  = new RiskData [(int)Charactor.eCharaType.MAX];
        _RiskDataStrong  = new RiskData [(int)Charactor.eCharaType.MAX];

        //  Balance
        #region Balance
        int b = (int)Charactor.eCharaType.BALANCE;

        #region Weak
        _RatioDataWeak[b].Wait = 60;
        _RatioDataWeak[b].Move = 10;
        _RatioDataWeak[b].Put = 20;
        _RatioDataWeak[b].Break = 10;

        _RatioDataWeak[b].SpecialWait    = 40;
        _RatioDataWeak[b].SpecialMove    = 10;
        _RatioDataWeak[b].SpecialPut     = 40;
        _RatioDataWeak[b].SpecialBreak   = 10;

        _RiskDataWeak[b].maxRisk = 20;
        _RiskDataWeak[b].riskRange = 1;
        #endregion

        #region Normal
        _RatioDataNormal[b].Wait         = 30;
        _RatioDataNormal[b].Move         = 10;
        _RatioDataNormal[b].Put          = 40;
        _RatioDataNormal[b].Break        = 20;

        _RatioDataNormal[b].SpecialWait  = 20;
        _RatioDataNormal[b].SpecialMove  = 10;
        _RatioDataNormal[b].SpecialPut   = 60;
        _RatioDataNormal[b].SpecialBreak = 10;

        _RiskDataNormal[b].maxRisk = 30;
        _RiskDataNormal[b].riskRange = 2;
        #endregion

        #region Strong
        _RatioDataStrong[b].Wait         =  0;
        _RatioDataStrong[b].Move         = 20;
        _RatioDataStrong[b].Put          = 70;
        _RatioDataStrong[b].Break        = 10;

        _RatioDataStrong[b].SpecialWait  =  0;
        _RatioDataStrong[b].SpecialMove  =  5;
        _RatioDataStrong[b].SpecialPut   = 90;
        _RatioDataStrong[b].SpecialBreak =  5;

        _RiskDataStrong[b].maxRisk = 30;
        _RiskDataStrong[b].riskRange = 3;
        #endregion

        #endregion

        //  Power
        #region Power
        int p = (int)Charactor.eCharaType.POWER;

        #region Weak
        _RatioDataWeak[p].Wait = 60;
        _RatioDataWeak[p].Move = 10;
        _RatioDataWeak[p].Put = 20;
        _RatioDataWeak[p].Break = 10;

        _RatioDataWeak[p].SpecialWait = 50;
        _RatioDataWeak[p].SpecialMove = 10;
        _RatioDataWeak[p].SpecialPut = 10;
        _RatioDataWeak[p].SpecialBreak = 30;

        _RiskDataWeak[p].maxRisk = 20;
        _RiskDataWeak[p].riskRange = 1;
        #endregion

        #region Normal
        _RatioDataNormal[p].Wait = 30;
        _RatioDataNormal[p].Move = 10;
        _RatioDataNormal[p].Put = 40;
        _RatioDataNormal[p].Break = 20;

        _RatioDataNormal[p].SpecialWait = 15;
        _RatioDataNormal[p].SpecialMove = 5;
        _RatioDataNormal[p].SpecialPut = 20;
        _RatioDataNormal[p].SpecialBreak = 60;

        _RiskDataNormal[p].maxRisk = 40;
        _RiskDataNormal[p].riskRange = 2;
        #endregion

        #region Strong
        _RatioDataStrong[p].Wait = 0;
        _RatioDataStrong[p].Move = 20;
        _RatioDataStrong[p].Put = 50;
        _RatioDataStrong[p].Break = 30;

        _RatioDataStrong[p].SpecialWait = 0;
        _RatioDataStrong[p].SpecialMove = 10;
        _RatioDataStrong[p].SpecialPut = 20;
        _RatioDataStrong[p].SpecialBreak = 70;

        _RiskDataStrong[p].maxRisk = 40;
        _RiskDataStrong[p].riskRange = 3;
        #endregion

        #endregion

        //  Speed
        #region Speed
        int s = (int)Charactor.eCharaType.SPEED;

        #region Weak
        _RatioDataWeak[s].Wait = 60;
        _RatioDataWeak[s].Move = 10;
        _RatioDataWeak[s].Put = 20;
        _RatioDataWeak[s].Break = 10;

        _RatioDataWeak[s].SpecialWait = 50;
        _RatioDataWeak[s].SpecialMove = 10;
        _RatioDataWeak[s].SpecialPut = 30;
        _RatioDataWeak[s].SpecialBreak = 10;

        _RiskDataWeak[s].maxRisk = 100;
        _RiskDataWeak[s].riskRange = 1;
        #endregion

        #region Normal
        _RatioDataNormal[s].Wait = 30;
        _RatioDataNormal[s].Move = 10;
        _RatioDataNormal[s].Put = 40;
        _RatioDataNormal[s].Break = 20;

        _RatioDataNormal[s].SpecialWait = 20;
        _RatioDataNormal[s].SpecialMove = 10;
        _RatioDataNormal[s].SpecialPut = 50;
        _RatioDataNormal[s].SpecialBreak = 20;

        _RiskDataNormal[s].maxRisk = 50;
        _RiskDataNormal[s].riskRange = 2;
        #endregion

        #region Strong
        _RatioDataStrong[s].Wait = 0;
        _RatioDataStrong[s].Move = 30;
        _RatioDataStrong[s].Put = 65;
        _RatioDataStrong[s].Break = 5;

        _RatioDataStrong[s].SpecialWait = 0;
        _RatioDataStrong[s].SpecialMove = 15;
        _RatioDataStrong[s].SpecialPut = 80;
        _RatioDataStrong[s].SpecialBreak = 5;

        _RiskDataStrong[s].maxRisk = 35;
        _RiskDataStrong[s].riskRange = 2;
        #endregion

        #endregion

        //  Technical
        #region Technical
        int t = (int)Charactor.eCharaType.TECHNICAL;

        #region Weak
        _RatioDataWeak[t].Wait = 60;
        _RatioDataWeak[t].Move = 10;
        _RatioDataWeak[t].Put = 20;
        _RatioDataWeak[t].Break = 10;

        _RatioDataWeak[t].SpecialWait = 50;
        _RatioDataWeak[t].SpecialMove = 30;
        _RatioDataWeak[t].SpecialPut = 10;
        _RatioDataWeak[t].SpecialBreak = 10;

        _RiskDataWeak[t].maxRisk = 20;
        _RiskDataWeak[t].riskRange = 1;
        #endregion

        #region Normal
        _RatioDataNormal[t].Wait = 30;
        _RatioDataNormal[t].Move = 10;
        _RatioDataNormal[t].Put = 50;
        _RatioDataNormal[t].Break = 10;

        _RatioDataNormal[t].SpecialWait = 20;
        _RatioDataNormal[t].SpecialMove = 60;
        _RatioDataNormal[t].SpecialPut = 10;
        _RatioDataNormal[t].SpecialBreak = 10;

        _RiskDataNormal[t].maxRisk = 30;
        _RiskDataNormal[t].riskRange = 2;
        #endregion

        #region Strong
        _RatioDataStrong[t].Wait = 0;
        _RatioDataStrong[t].Move = 20;
        _RatioDataStrong[t].Put = 70;
        _RatioDataStrong[t].Break = 10;

        _RatioDataStrong[t].SpecialWait = 0;
        _RatioDataStrong[t].SpecialMove = 80;
        _RatioDataStrong[t].SpecialPut = 10;
        _RatioDataStrong[t].SpecialBreak = 10;

        _RiskDataStrong[t].maxRisk = 30;
        _RiskDataStrong[t].riskRange = 3;
        #endregion

        #endregion
    }

    public int[] GetRatio(int level, Charactor.eCharaType type, bool isSpecial)
    {
        int[] ratio = new int[4];
        int idx = (int)type;
        RatioData[] data = GetForLevel(level);

        if(isSpecial)
        {
            ratio[0] = data[idx].SpecialWait;
            ratio[1] = data[idx].SpecialMove;
            ratio[2] = data[idx].SpecialPut;
            ratio[3] = data[idx].SpecialBreak;
        }
        else
        {
            ratio[0] = data[idx].Wait;
            ratio[1] = data[idx].Move;
            ratio[2] = data[idx].Put;
            ratio[3] = data[idx].Break;
        }

        return ratio;
    }

    RatioData[] GetForLevel(int level)
    {
        switch(level)
        {
            case 0:
                return _RatioDataStrong;
            case 1:
                return _RatioDataNormal;
            case 2:
                return _RatioDataWeak;
        }
        return null;
    }

    public RiskData GetRisk(int level, Charactor.eCharaType type)
    {
        RiskData risk = new RiskData();
        int idx = (int)type;
        switch(level)
        {
            case 0:
                risk = _RiskDataStrong[idx];
                    break;
            case 1:
                risk = _RiskDataNormal[idx];
                    break;
            case 2:
                risk = _RiskDataWeak[idx];
                    break;
        }
        return risk;
    }
}
