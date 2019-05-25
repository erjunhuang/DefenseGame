using UnityEngine;

public class CameraAspect : MonoBehaviour {
    //屏幕适配
    private float orthographicSize;
    private float cameraWidth;
    private float cameraHeight;
	void OnEnable () {
		Camera camera = GetComponent<Camera>();
		orthographicSize = camera.orthographicSize;

        cameraWidth = orthographicSize* Camera.main.aspect * 2;
		if (cameraWidth< orthographicSize)
        {
			camera.orthographicSize = orthographicSize / (2 * Camera.main.aspect);
			cameraWidth = orthographicSize;
		}
		cameraHeight = camera.orthographicSize*2;
        UIPathDefines.ScreenHeight = cameraHeight;
        UIPathDefines.ScreenWidth = cameraWidth;
    }
}
