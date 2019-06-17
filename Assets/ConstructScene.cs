using Core.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum MappingType
{
    WorldToScreen,
    ScreenToWorld,
    None
}
public class ConstructScene : MonoBehaviour {
    public MappingType selfMapping = MappingType.ScreenToWorld;
    // Use this for initialization
    void Awake () {
        //场景现在偏离了一点点 运行生成场景的时候把坐标转回来
        transform.position = VectorHelper.ScreenToWorld(transform.position, transform.position);
        //然后把编辑好的2D场景转成3D的
        VectorHelper.MappingAllChildren(this.gameObject, selfMapping);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
