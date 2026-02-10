using System;
using DG.Tweening;
using Larje.Core;
using Larje.Core.Services;
using Larje.Core.Services.UI;
using ProjectConstants;
using UnityEngine;
using UnityEngine.SceneManagement;

[BindService(typeof(BootstrapperService))]
public class BootstrapperService : Service
{
    [Header("Splash")]
    [SerializeField] private UIScreenType splashScreen;
    [SerializeField] private string splashScene;
    [SerializeField] private float splashDuration;
    
    [Header("Menu")]
    [SerializeField] private UIScreenType menuScreen;
    [SerializeField] private string menuScene;
    
    [Header("Loading")]
    [SerializeField] private UIScreenType loadingScreen;
    [SerializeField] private string loadingScene;
    [SerializeField] private float minLoadingDuration;

    [Header("Play")]
    [SerializeField] private UIScreenType playScreen;

    [InjectService] private UIService _uiService;
    [InjectService] private IGameStateService _gameStateService;

    private float _transitionValue;
    
    public override void Init()
    {
        ShowSplash();        
    }

    public void LoadLocation(string sceneName, Action onComplete = null)
    {
        ShowLoading(sceneName, () =>
        {
            onComplete?.Invoke();
            _uiService.GetProcessor<UIScreenProcessor>().OpenScreen(new UIScreen.Args(playScreen)); 
        });
    }

    public void LoadMenuWithTransition()
    {
        _gameStateService.SetGameState(GameStates.Menu);

        if (LarjePostFXFeature.TryGetFX(out LarjeFXTransition.Processor transitionFX))
        {
            transitionFX.AddProvider(GetTransitionValue);
        }

        DOVirtual.Float(0f, 1f, 0.5f, value => _transitionValue = value)
            .OnComplete(() =>
            {
                LoadMenu(() =>
                {
                    DOVirtual.Float(1f, 0f, 0.5f, value => _transitionValue = value);
                });
            });
    }
    
    public void LoadMenu(Action onComplete = null)
    {
        _gameStateService.SetGameState(GameStates.Menu);

        SceneManager.LoadScene(menuScene);
        DOVirtual.DelayedCall(0.25f, () =>
        {
            onComplete?.Invoke();
            UIScreen.Args menuScreen = new MenuScreen.Args();
            _uiService.GetProcessor<UIScreenProcessor>().OpenScreen(menuScreen); 
        });
    }

    public void QuitApplication()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
    
    private void ShowSplash()
    {
        SceneManager.LoadScene(splashScene);
        UIScreen.Args splashScreen = new SplashScreen.Args(splashDuration, () => LoadMenu());
        _uiService.GetProcessor<UIScreenProcessor>().OpenScreen(splashScreen);
    }

    private float GetTransitionValue()
    {
        return _transitionValue;
    }
    
    private void ShowLoading(string sceneName, Action onComplete)
    {
        //SceneManager.LoadScene(loadingScene);
        UIScreen.Args loadingScreen = new LoadingScreen.Args(minLoadingDuration, 
            () => SceneManager.LoadSceneAsync(sceneName), onComplete);
        _uiService.GetProcessor<UIScreenProcessor>().OpenScreen(loadingScreen);
    }
}
