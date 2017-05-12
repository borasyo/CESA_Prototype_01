using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SandMass : FieldObjectBase
{
    SpriteRenderer _spRend = null;
	
    void Start()
    {
        _spRend = GetComponent<SpriteRenderer>();
    }

	void Update () 
    {
        _spRend.color = new Color(0,0,0,0);
        List<SandData.tSandData> sandDataList = SandData.Instance.GetSandDataList;
        for (int i = 0; i < sandDataList.Count; i++)
        {
            if (sandDataList[i]._number != GetDataNumber())
                continue;

            SandData.tSandData data = sandDataList[i];
            switch (data._Type)
            {
                case SandItem.eType.PLAYER:
                    _spRend.color += new Color(255,0,0,255);
                    break;
                case SandItem.eType.ENEMY:
                    _spRend.color += new Color(0,0,255,255);
                    break;
            }
        }
	}
}
