using UnityEngine.UI;
using Core.Utilities;
using System;
using DG.Tweening;
using UnityEngine;

public struct ShowTipParams
{
    public string msg;
    public float time;
    public Action action;
}

public class KindlyReminderUI : Singleton<KindlyReminderUI>
{
    private Text showText;
    public Vector3 offsetPos = new Vector3(0,10,0);
    private Vector3 startPos;
    private Tweener tweener;
    // Use this for initialization
    void Start () {
        showText = GetComponent<Text>();
        showText.text = "";
        startPos = transform.position;
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void ShowTip(ShowTipParams showTipParams) {
        ResetUI();
        if (showText) {
             showText.text = showTipParams.msg;
        }
        tweener =  transform.DOBlendableScaleBy(transform.localScale*0.1f, 1f);
        //Tweener tweener =  transform.DOMove(transform.position + offsetPos, 1f);
        //tweener.SetEase(Ease.InBack);
        tweener.OnComplete(ResetUI);
    }

    void ResetUI() {
        showText.text = "";
        transform.position = startPos;
        transform.localScale = Vector3.one;
        if (tweener!=null) {
            tweener.Kill();
            tweener = null;
        }
    }
}
