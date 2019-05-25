using UnityEngine;
using UnityEngine.UI;

public class LoadingSceneUI : MonoBehaviour
{
    public Text text;
    public delegate void LoadingEvent();
    private void Start()
    {
        text = GetComponent<Text>();
    }
}
