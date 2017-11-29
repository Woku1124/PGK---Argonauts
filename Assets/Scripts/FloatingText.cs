using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FloatingText : MonoBehaviour
{

    public Animator animator;
    private Text resourceToStationText;

	// Use this for initialization
	void OnEnable ()
	{

	    AnimatorClipInfo[] clipInfo = animator.GetCurrentAnimatorClipInfo(0);
        Destroy(gameObject,clipInfo[0].clip.length);
	    resourceToStationText = animator.GetComponent<Text>();
	}
	
	// Update is called once per frame
	public void SetText (string text)
	{
	   resourceToStationText.text = text;
	}
}
