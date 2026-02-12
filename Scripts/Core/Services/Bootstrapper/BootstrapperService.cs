using System;
using System.Collections;
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
    [SerializeField] private string splashScene;
    [SerializeField] private float splashDuration;
    
    [Header("Loading")]
    [SerializeField] private string loadingScene;
    [SerializeField] private float minLoadingDuration;
    
    [Header("Menu")]
    [SerializeField] private string menuScene;

    [InjectService] private IGameStateService _gameStateService;

    private float _splashProgress;
    private float _loadingProgress;

    public float SplashProgress => _splashProgress;
    public float LoadingProgress => _loadingProgress;

    public event Action EventSplashStarted;
    public event Action EventLoadingStarted;
    public event Action EventMenuEntered;
    public event Action EventSceneLoaded;
    
    public override void Init()
    {
        ShowSplash(() => ShowMenu());        
    }

    private void ShowSplash(Action onComplete = null)
    {
        if (_gameStateService.CurrentState != GameStates.Splash)
        {
            _splashProgress = 0f;
            _gameStateService.SetGameState(GameStates.Splash);
            SceneManager.LoadScene(splashScene);

            EventSplashStarted?.Invoke();

            DOVirtual.Float(0, 1, splashDuration, t => _splashProgress = t)
                .OnComplete(() => 
                {
                    onComplete?.Invoke();
                });
        }
    }

    public void ShowMenu(Action onComplete = null)
    {
        if (_gameStateService.CurrentState != GameStates.Menu)
        {
            _gameStateService.SetGameState(GameStates.Menu);
            SceneManager.LoadScene(menuScene);

            EventMenuEntered?.Invoke();
        }
    }

    public void LoadSceneAsync(string sceneName, Action onComplete)
    {        
        if (_gameStateService.CurrentState != GameStates.Loading)
        {
            _loadingProgress = 0f;
            _gameStateService.SetGameState(GameStates.Loading);

            EventLoadingStarted?.Invoke();

            DOVirtual.DelayedCall(0.25f, () => 
            {
                SceneManager.LoadScene(loadingScene);
                StartCoroutine(LoadSceneAsyncCoroutine(sceneName, onComplete));
            }, ignoreTimeScale: true);
        }
    }

    public void QuitApplication()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private IEnumerator LoadSceneAsyncCoroutine(string sceneName, Action onComplete)
    {
        Debug.Log("Start LoadSceneAsyncCoroutine");

        var op = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
        op.allowSceneActivation = false;
        while (!op.isDone)
        {
            _loadingProgress = Mathf.Clamp01(op.progress / 0.9f);
            Debug.Log("Update ciriutine progress: " + _loadingProgress);
            if (_loadingProgress >= 1f - 0.0001f)
            {
                yield return null;
                op.allowSceneActivation = true;
            }

            yield return null;
        }

        onComplete?.Invoke();
        EventSceneLoaded?.Invoke();
    }
}
