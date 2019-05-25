using System.Collections;
using System.Collections.Generic;
using TargetDefense.Nodes;
using UnityEngine;

public class MapManager : MonoBehaviour {
    public List<Node> linkedNodes;
    // Use this for initialization
    void Start () {
        CreateMap();
    }
	
	// Update is called once per frame
	void Update () {
		
	}


#if UNITY_EDITOR
    protected virtual void OnDrawGizmos()
    {
        CreateMap();
    }
#endif

    void CreateMap() {
        //foreach (Node nodes1 in linkedNodes)
        //{
        //    foreach (Node nodes2 in linkedNodes)
        //    {
        //        if (nodes1.m_Index > 0 && nodes2.m_Index > 0)
        //        {
        //            if ((nodes1.m_Index + 1) == nodes2.m_Index)
        //            {
        //                FixedNodeSelector fixedNodeSelector = nodes1.GetComponent<FixedNodeSelector>();
        //                fixedNodeSelector.linkedNodes.Clear();
        //                fixedNodeSelector.linkedNodes.Add(nodes2);
        //            }
        //        }
        //    }
        //}
    }
}
