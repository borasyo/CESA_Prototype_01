using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialFlow : MonoBehaviour
{
    [SerializeField]
    TutorialRange _RightRange = null;
    [SerializeField]
    TutorialRange _LeftRange = null;

    [SerializeField]
    TutorialArrow _UpArrow = null;
    [SerializeField]
    TutorialArrow _UpArrow2 = null;
    [SerializeField]
    TutorialArrow _DownArrow = null;
    [SerializeField]
    TutorialArrow _RightArrow = null;

    [SerializeField]
    TutorialDescription _Description = null;

    void Start()
    {
        StartCoroutine(Flow());
        Camera.main.GetComponent<SetCameraPos>().AdjustmentPos(new Vector3(0,0,1));
    }

    IEnumerator Flow()
    {
        yield return null;
        yield return new WaitWhile(() => Time.timeScale <= 0);

        TutorialCharacter tutoChara = FindObjectOfType<TutorialCharacter>();
        CharacterGauge charaGauge = tutoChara.GetComponent<CharacterGauge>();
        CharacterInput charaInput = tutoChara.GetComponent<CharacterInput>();

        StartCoroutine(_Description.OnWindow());
        yield return new WaitWhile(() => !_Description.IsNext);

        //  左側をタッチさせる
        StartCoroutine(_LeftRange.OnWindow());
        yield return new WaitWhile(() => !_LeftRange.IsNext);

        yield return new WaitWhile(() => 
        {
            if (charaInput.GetMoveInput(Character.eDirection.RIGHT))
                return false;

            if (Input.touchCount > 0 && Input.GetTouch(0).position.x < Screen.width / 2.0f)
                return false;

            return true;
        });
        tutoChara.SetNowAction = TutorialCharacter.eNowAction.MAX;
        
        //  右側へ移動させる
        StartCoroutine(_LeftRange.OffWindow());
        yield return new WaitWhile(() => !_LeftRange.IsNext);
        StartCoroutine(_RightArrow.OnWindow());
        yield return new WaitWhile(() => !_RightArrow.IsNext);
        tutoChara.SetNowAction = TutorialCharacter.eNowAction.RIGHT;
        yield return new WaitWhile(() => tutoChara.GetDataNumber() != 109);
        yield return new WaitWhile(() => tutoChara.GetPosForNumber(tutoChara.GetDataNumber()).x - tutoChara.transform.position.x > 0.1f);
        tutoChara.SetNowAction = TutorialCharacter.eNowAction.MAX;
        StartCoroutine(_RightArrow.OffWindow());
        yield return new WaitWhile(() => !_RightArrow.IsNext);
        StartCoroutine(_Description.OnWindow());
        yield return new WaitWhile(() => !_Description.IsNext);

        //  上を向かせる
        StartCoroutine(_UpArrow.OnWindow());
        yield return new WaitWhile(() => !_UpArrow.IsNext);
        tutoChara.SetNowAction = TutorialCharacter.eNowAction.FORWARD;
        yield return new WaitWhile(() => tutoChara.GetDataNumberForDir() != tutoChara.GetDataNumber() + GameScaler._nWidth);
        tutoChara.SetNowAction = TutorialCharacter.eNowAction.MAX;
        
        //  上に置かせる
        StartCoroutine(_UpArrow.OffWindow());
        yield return new WaitWhile(() => !_UpArrow.IsNext);
        StartCoroutine(_RightRange.OnWindow());
        yield return new WaitWhile(() => !_RightRange.IsNext);
        tutoChara.SetNowAction = TutorialCharacter.eNowAction.PUT;
        charaGauge.GaugeMax();
        yield return new WaitWhile(() => !FieldData.Instance.GetObjData(tutoChara.GetDataNumber() + GameScaler._nWidth));
        tutoChara.SetNowAction = TutorialCharacter.eNowAction.MAX;
        yield return new WaitForSeconds(1.0f);
        StartCoroutine(_RightRange.OffWindow());
        yield return new WaitWhile(() => !_RightRange.IsNext);
        StartCoroutine(_Description.OnWindow());
        yield return new WaitWhile(() => !_Description.IsNext);
        StartCoroutine(_Description.OnWindow());
        yield return new WaitWhile(() => !_Description.IsNext);
        StartCoroutine(_Description.OnWindow());
        yield return new WaitWhile(() => !_Description.IsNext);

        //  左を向かせる
        StartCoroutine(_DownArrow .OnWindow());
        yield return new WaitWhile(() => !_DownArrow.IsNext);
        tutoChara.SetNowAction = TutorialCharacter.eNowAction.LEFT;
        yield return new WaitWhile(() => tutoChara.GetDataNumberForDir() != tutoChara.GetDataNumber() - 1);
        tutoChara.SetNowAction = TutorialCharacter.eNowAction.MAX;

        //  左に置かせる
        StartCoroutine(_DownArrow.OffWindow());
        yield return new WaitWhile(() => !_DownArrow.IsNext);
        StartCoroutine(_RightRange.OnWindow());
        yield return new WaitWhile(() => !_RightRange.IsNext);
        tutoChara.SetNowAction = TutorialCharacter.eNowAction.PUT;
        charaGauge.GaugeMax();
        yield return new WaitWhile(() => !FieldData.Instance.GetObjData(tutoChara.GetDataNumber() - 1));
        tutoChara.SetNowAction = TutorialCharacter.eNowAction.MAX;
        yield return new WaitForSeconds(1.0f);
        StartCoroutine(_RightRange.OffWindow());
        yield return new WaitWhile(() => !_RightRange.IsNext);
        StartCoroutine(_Description.OnWindow());
        yield return new WaitWhile(() => !_Description.IsNext);

        //  上を向かせる
        StartCoroutine(_UpArrow2.OnWindow());
        yield return new WaitWhile(() => !_UpArrow2.IsNext);
        tutoChara.SetNowAction = TutorialCharacter.eNowAction.FORWARD;
        yield return new WaitWhile(() => tutoChara.GetDataNumberForDir() != tutoChara.GetDataNumber() + GameScaler._nWidth);
        tutoChara.SetNowAction = TutorialCharacter.eNowAction.MAX;

        // 壊させる
        StartCoroutine(_UpArrow2.OffWindow());
        yield return new WaitWhile(() => !_UpArrow2.IsNext);
        StartCoroutine(_RightRange.OnWindow());
        yield return new WaitWhile(() => !_RightRange.IsNext);
        tutoChara.SetNowAction = TutorialCharacter.eNowAction.BREAK;
        charaGauge.GaugeMax();
        yield return new WaitWhile(() => FieldData.Instance.GetObjData(tutoChara.GetDataNumberForDir()));
        tutoChara.SetNowAction = TutorialCharacter.eNowAction.MAX;
        StartCoroutine(_RightRange.OffWindow());
        yield return new WaitWhile(() => !_RightRange.IsNext);
        StartCoroutine(_Description.OnWindow());
        yield return new WaitWhile(() => !_Description.IsNext);

        //  右側へ移動させる
        StartCoroutine(_RightArrow.OnWindow());
        yield return new WaitWhile(() => !_RightArrow.IsNext);
        tutoChara.SetNowAction = TutorialCharacter.eNowAction.RIGHT;
        yield return new WaitWhile(() => tutoChara.GetDataNumber() != 115);
        yield return new WaitWhile(() => tutoChara.GetPosForNumber(tutoChara.GetDataNumber()).x - tutoChara.transform.position.x > 0.1f);
        tutoChara.SetNowAction = TutorialCharacter.eNowAction.MAX;

        //  右側に置かせる
        StartCoroutine(_RightArrow.OffWindow());
        yield return new WaitWhile(() => !_RightArrow.IsNext);
        StartCoroutine(_RightRange.OnWindow());
        yield return new WaitWhile(() => !_RightRange.IsNext);
        tutoChara.SetNowAction = TutorialCharacter.eNowAction.PUT;
        yield return new WaitWhile(() => !FieldData.Instance.GetObjData(tutoChara.GetDataNumber() + 1));
        tutoChara.SetNowAction = TutorialCharacter.eNowAction.MAX;
        yield return new WaitForSeconds(1.0f);
        StartCoroutine(_RightRange.OffWindow());
        yield return new WaitWhile(() => !_RightRange.IsNext);

        //  最後
        yield return new WaitForSeconds(1.0f);
        FieldData.Instance.GetCharactors[0].Win();
        yield return new WaitWhile(() => Time.timeScale <= 0.0f);

        StartCoroutine(_Description.OnWindow());
        yield return new WaitWhile(() => !_Description.IsNext);
        StartCoroutine(_Description.OnWindow());
        yield return new WaitWhile(() => !_Description.IsNext);
        StartCoroutine(_Description.OnWindow());
        yield return new WaitWhile(() => !_Description.IsNext);
        StartCoroutine(_Description.OnWindow());
        yield return new WaitWhile(() => !_Description.IsNext);
        StartCoroutine(_Description.OnWindow());
        yield return new WaitWhile(() => !_Description.IsNext);
        StartCoroutine(_Description.OnWindow());
        yield return new WaitWhile(() => !_Description.IsNext);


        SceneChanger.Instance.ChangeScene("ModeSelect", true);
    }
}
