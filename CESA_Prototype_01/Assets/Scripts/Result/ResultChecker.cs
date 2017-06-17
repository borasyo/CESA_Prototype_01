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
        GameObject canvas = null;

        //  ここでIntervalかLastか判断
        if(RoundCounter.nNowWinerPlayer >= 0)
        {
            canvas = intervalCanvas;
            //  flameも変更
        }
        else
        {
            canvas = lastCanvas;
            //  flameも変更
        }

        if(PhotonNetwork.inRoom)
        {
            if (!PhotonNetwork.isMasterClient)
                return;

            PhotonNetwork.Instantiate("Prefabs/Result/" + canvas.name, canvas.transform.position, canvas.transform.rotation, 0);
        }
        else
        {
            Instantiate(canvas);
        }
    }
}
