using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialFlow : MonoBehaviour
{
    [SerializeField]
    GameObject _RightRange = null;
    [SerializeField]
    GameObject _LeftRange = null;

    [SerializeField]
    GameObject _UpArrow = null;
    [SerializeField]
    GameObject _UpArrow2 = null;
    [SerializeField]
    GameObject _DownArrow = null;
    [SerializeField]
    GameObject _RightArrow = null;

    void Start()
    {
        _RightRange.SetActive(false);
        _LeftRange.SetActive(false);
        _UpArrow.SetActive(false);
        _UpArrow2.SetActive(false);
        _DownArrow.SetActive(false);
        _RightArrow.SetActive(false);

        StartCoroutine(Flow());
    }

    IEnumerator Flow()
    {
        yield return null;
        yield return new WaitWhile(() => Time.timeScale <= 0);

        TutorialCharacter tutoChara = FindObjectOfType<TutorialCharacter>();
        CharacterGauge charaGauge = tutoChara.GetComponent<CharacterGauge>();
        CharacterInput charaInput = tutoChara.GetComponent<CharacterInput>();

        //  左側をタッチさせる
        _LeftRange.SetActive(true);
        yield return new WaitWhile(() => 
        {
            if (charaInput.GetMoveInput(Character.eDirection.RIGHT))
                return false;

            if (Input.touchCount > 0 && Input.GetTouch(0).position.x < Screen.width / 2.0f)
                return false;

            return true;
        });

        //  右側へ移動させる
        _LeftRange.SetActive(false);
        _RightArrow.SetActive(true);
        tutoChara.SetNowAction = TutorialCharacter.eNowAction.RIGHT;

        yield return new WaitWhile(() => tutoChara.GetDataNumber() != 70);
        yield return new WaitWhile(() => tutoChara.GetPosForNumber(tutoChara.GetDataNumber()).x - tutoChara.transform.position.x > 0.1f);

        //  上を向かせる
        _RightArrow.SetActive(false);
        _UpArrow.SetActive(true);
        tutoChara.SetNowAction = TutorialCharacter.eNowAction.FORWARD;

        yield return new WaitWhile(() => tutoChara.GetDataNumberForDir() != tutoChara.GetDataNumber() + GameScaler._nWidth);

        //  上に置かせる
        _UpArrow.SetActive(false);
        _RightRange.SetActive(true);
        tutoChara.SetNowAction = TutorialCharacter.eNowAction.PUT;
        charaGauge.GaugeMax();

        yield return new WaitWhile(() => !FieldData.Instance.GetObjData(tutoChara.GetDataNumber() + GameScaler._nWidth));

        //  下を向かせる
        _RightRange.SetActive(false);
        _DownArrow.SetActive(true);
        tutoChara.SetNowAction = TutorialCharacter.eNowAction.BACK;

        yield return new WaitWhile(() => tutoChara.GetDataNumberForDir() != tutoChara.GetDataNumber() - GameScaler._nWidth);

        //  下に置かせる
        _DownArrow.SetActive(false);
        _RightRange.SetActive(true);
        tutoChara.SetNowAction = TutorialCharacter.eNowAction.PUT;
        charaGauge.GaugeMax();

        yield return new WaitWhile(() => !FieldData.Instance.GetObjData(tutoChara.GetDataNumber() - GameScaler._nWidth));

        //  上を向かせる
        _RightRange.SetActive(false);
        _UpArrow2.SetActive(true);
        tutoChara.SetNowAction = TutorialCharacter.eNowAction.FORWARD;
        
        yield return new WaitWhile(() => tutoChara.GetDataNumberForDir() != tutoChara.GetDataNumber() + GameScaler._nWidth);

        // 壊させる
        _UpArrow2.SetActive(false);
        _RightRange.SetActive(true);
        tutoChara.SetNowAction = TutorialCharacter.eNowAction.BREAK;
        charaGauge.GaugeMax();

        yield return new WaitWhile(() => FieldData.Instance.GetObjData(tutoChara.GetDataNumberForDir()));

        //  右側へ移動させる
        _RightRange.SetActive(false);
        _RightArrow.SetActive(true);
        tutoChara.SetNowAction = TutorialCharacter.eNowAction.RIGHT;

        yield return new WaitWhile(() => tutoChara.GetDataNumber() != 74);
        yield return new WaitWhile(() => tutoChara.GetPosForNumber(tutoChara.GetDataNumber()).x - tutoChara.transform.position.x > 0.1f);

        _RightArrow.SetActive(false);
        _RightRange.SetActive(true);
        tutoChara.SetNowAction = TutorialCharacter.eNowAction.PUT;
        yield return new WaitWhile(() => !FieldData.Instance.GetObjData(tutoChara.GetDataNumber() + 1));

        tutoChara.SetNowAction = TutorialCharacter.eNowAction.MAX;
        _RightRange.SetActive(false);
    }
}
