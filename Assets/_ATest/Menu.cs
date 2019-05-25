using UnityEngine;

public class Menu : MonoBehaviour {

	public Animator animator;
    private GameObject PlayButton, QuitButton;
    private void Start()
    {
        //PlayButton = GameObject.Find("UI/PlayCanvas/PlayButton");
        //QuitButton = GameObject.Find("UI/QuitCanvas/QuitButton");

        //EventTriggerListener.Get(PlayButton).onPointerClick = new TouchHandle(StartGame);
        //EventTriggerListener.Get(QuitButton).onPointerClick = new TouchHandle(Quit);
    }
    public void StartGame(GameObject _sender, object _args, params object[] _params)
    {
        animator.SetTrigger("Start");
    }

	public void Quit (GameObject _sender, object _args, params object[] _params)
	{
		Debug.Log("QUITTING");
        Application.Quit();
	}

	public void LoadLevel()
	{
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex + 1);
    }

}
