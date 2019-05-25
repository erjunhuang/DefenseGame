using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenSpaceSnow : MonoBehaviour {
    private Material _material;
    public Vector4 _Rotation = new Vector4(45, 0, 0, 0);
    void OnEnable()
    {
        // dynamically create a material that will use our shader
        _material = new Material(Shader.Find("Ditto/CameraMapping"));
        
    }
    private void Update()
    {
    }
    //void OnRenderImage(RenderTexture src, RenderTexture dest)
    //{
         
    //        // set shader properties
    //    _material.SetTexture("_MainTex", src);
    //    _material.SetVector("_Rotation", _Rotation);
    //    // execute the shader on input texture (src) and write to output (dest)
    //    Graphics.Blit(src, dest, _material);
    //}

}
