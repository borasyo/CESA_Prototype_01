using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ResultChecker : MonoBehaviour
{
    [SerializeField]
    GameObject intervalCanvas = null;
    [SerializeField]
    GameObject lastCanvas = null;

    //  
    void Start ()
    {
        //  ここでIntervalかLastか判断
        if(RoundCounter.nNowWinerPlayer >= 0)
        {
            Instantiate(intervalCanvas); //.transform.SetParent(transform.parent);
            //  flameも変更
        }
        else
        {
            Instantiate(lastCanvas); //.transform.SetParent(transform.parent);
            //  flameも変更
        }
    }
}
