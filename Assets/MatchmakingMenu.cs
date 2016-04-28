using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MatchmakingMenu : MonoBehaviour 
{
    private float FadeInSeconds = 0.3f;
    public RtsMatchmaker Matcher;

    public PanelFader Fader;

    private float ChildFadeInSeconds = 0.5f;

    public PanelFader[] SubMenus;
    PanelFader _currentChild;

    // Use this for initialization
    void Start () 
	{
        Assert.IsNotNull(Fader);
        Assert.IsNotNull(Matcher);

        Matcher.OnMatchFound += OnMatchFound;
        Matcher.OnMatchFailed += OnMatchFailed;
        
        if(SubMenus.Length > 0)
        {
            _currentChild = SubMenus[0];
            _currentChild.gameObject.SetActive(true);
        }
        for(int i = 1; i < SubMenus.Length; i++)
        {
            SubMenus[i].gameObject.SetActive(false);
        }

        Fader.FadeIn(FadeInSeconds, 0.5f);
    }
    
    public void OnFindMatchPressed()
    {
        Matcher.Matchmake();
        OpenMenu("Finding");
    }
    
    public void OnMatchFound()
    {
        OpenMenu("Found");
        LeanTween.delayedCall(gameObject, 1f, () => OpenMenu("Lobby"));
    }
    
    public void OnMatchFailed()
    {
        OpenMenu("Failed");
        LeanTween.delayedCall(gameObject, 1f, () => OpenMenu("Start"));
    }
    
    public void OnMatchStarting()
    {
        OpenMenu("Starting");
    }
    
    private void OpenMenu(string menuName)
    {
        foreach(var child in SubMenus)
        {
            if(child.name == menuName)
            {
                _currentChild.FadeOut(ChildFadeInSeconds, 0f);
                child.gameObject.SetActive(true);
                child.FadeIn(ChildFadeInSeconds, 0f);
                _currentChild = child;
            }
        }
    }
}
