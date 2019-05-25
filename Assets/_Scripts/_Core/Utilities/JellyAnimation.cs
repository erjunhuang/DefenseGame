using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JellyAnimation : MonoBehaviour {
    private float time;
    public float scale;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        time += 10f*Time.deltaTime;
        //transform.localScale.x += sign(IN.pos.x)*sin(_Time.w)/50;
        //IN.pos.y += sign(IN.pos.y)*cos(_Time.w)/50;
        //Mathf.Sign(transform.localScale.x) * Mathf.Sin(Time.deltaTime) / 50
        transform.localScale += new Vector3(Mathf.Sign(transform.localScale.x) * Mathf.Sin(time) / scale, Mathf.Sign(transform.localScale.y) * Mathf.Cos(time) / scale/1.8f, 0);
    }
}
