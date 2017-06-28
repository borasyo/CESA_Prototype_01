using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UniRx;
using UniRx.Triggers;

//  上の電気エフェクトが膨張して壊れるという設定にしている

public class SandItem : FieldObjectBase
{
    static GameObject _SandItemHolder = null;

    public enum eType
    {
        ONE_P = 0,
        TWO_P,
        THREE_P,
        FOUR_P,
        BLOCK,
        ALL,

        MAX,
    };
    [SerializeField] eType _Type;
    eType _SetType;
    public eType GetType { get { return _Type; } set { _Type = value; } }
    [SerializeField] float _fMaxLife_Sec = 15.0f;
    [SerializeField] GameObject _breakEffect = null;

    static public bool _IsTutorial = false;

    void Awake()
    {
        transform.position += new Vector3(0, 0.516f, 0);
        FieldData.Instance.SetObjData(this, GetDataNumber());
        FieldData.Instance.ExceptionChangeField();
        _SetType = _Type;
        _Type = eType.MAX;

        if (_SandItemHolder)
            return;
        
        _SandItemHolder = new GameObject ("SandItemHolder");
    }

    void Start()
    {
        StartCoroutine(Init());
    }

    IEnumerator Init()
    {
        _sandItemData = this;
        transform.SetParent(_SandItemHolder.transform);

        MeshRenderer meRend = GetComponentInChildren<MeshRenderer>();
        meRend.enabled = false;
        for (int i = 0; i < transform.childCount; i++)
            transform.GetChild(i).gameObject.SetActive(false);

        DelayPut delayPut = GetComponent<DelayPut>();
        if (delayPut)
            delayPut.SetType = _SetType;

        yield return new WaitForSeconds(1.0f);

        //  テクニカルじゃなければ表示
        if (!delayPut)
        {
            meRend.enabled = true;
            for (int i = 0; i < transform.childCount; i++)
                transform.GetChild(i).gameObject.SetActive(true);

            FieldData.Instance.ExceptionChangeField();
            _Type = _SetType;
            SoundManager.Instance.PlaySE(SoundManager.eSeValue.THUNDER);
        }

        float life = _fMaxLife_Sec;

        List<ParticleSystem> particleList = new List<ParticleSystem> ();
        for(int i = 0; i < transform.childCount; i++)
            particleList.Add(transform.GetChild(i).GetComponent<ParticleSystem>());
        
        //Color initColor = particleList[0].startColor;
        List<float> initSize = new List<float>();
        foreach (ParticleSystem particle in particleList)
            initSize.Add(particle.startSize);

        if (_IsTutorial)
            yield break;

        this.UpdateAsObservable()
            .Subscribe(_ =>
            {
                life -= Time.deltaTime;

                float per = life / _fMaxLife_Sec;
                if (per <= 1.0f / 3.0f)
                {
                    per *= 3.0f;

                    //initColor.a = 1.0f * per;
                    //float nowSize = initSize + (initSize * (1.0f - per));
                    for (int i = 0; i < particleList.Count; i++)
                    { 
                        //particleList[i].startColor = initColor;
                        particleList[i].startSize = initSize[i] + (initSize[i] * (1.0f - per) * 0.5f);
                    }
                }

                if (life > 0.0f)
                    return;

                Break();
            });
    }
	
    void Update()
    {
        DataUpdate();
    }

    public void Break()
    {
        FieldData.Instance.SetObjData(null, GetDataNumber());
        FieldData.Instance.ExceptionChangeField();
        Instantiate(_breakEffect, transform.position, _breakEffect.transform.rotation);
        SoundManager.Instance.PlaySE(SoundManager.eSeValue.BREAK, 0.5f);
        Destroy(this.gameObject);
    }
}
