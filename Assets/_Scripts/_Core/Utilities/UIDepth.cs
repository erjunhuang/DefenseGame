using UnityEngine;
using UnityEngine.UI;
namespace QGame.Utils
{
    public class UIDepth : MonoBehaviour
    {
        public int order;
        public bool isUI = true;
        void Start()
        {
            if (isUI)
            {
                Canvas canvas = GetComponent<Canvas>();
                if (canvas == null)
                {
                    canvas = gameObject.AddComponent<Canvas>();
                      
                    //    GraphicRaycaster GraphicRaycaster =  gameObject.AddComponent<GraphicRaycaster>();
                }
                if (GetComponent<GraphicRaycaster>() == null)
                {
                    gameObject.AddComponent<GraphicRaycaster>();
                }
                canvas.overridePixelPerfect = true;
                canvas.pixelPerfect = true;
                canvas.overrideSorting = true;
                canvas.sortingOrder = order;
  
            }
            else
            {
                Renderer[] renders = GetComponentsInChildren<Renderer>();

                foreach (Renderer render in renders)
                {
                    render.sortingOrder = order;
                    //foreach (Material material in render.materials)
                    //{
                    //    material.renderQueue = 3000;
                    //}
                }
            }
        }

        public void Restart()
        {
            Start();
        }
    }
}