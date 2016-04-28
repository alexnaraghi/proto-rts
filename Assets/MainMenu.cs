using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour 
{
	public PanelFader Fader;
    private float FadeOutSeconds = 0.5f;

    private float alpha;

    void Start()
	{
        Assert.IsNotNull(Fader);
    }

    public void OnStartPressed()
	{
		SceneManager.LoadScene("Matchmaking", LoadSceneMode.Additive);

        Fader.FadeOut(FadeOutSeconds, 0f);
        LeanTween.delayedCall(gameObject, FadeOutSeconds, DestroyUI);
    }
	
	private void DestroyUI()
	{
        Destroy(gameObject);
    }
}
