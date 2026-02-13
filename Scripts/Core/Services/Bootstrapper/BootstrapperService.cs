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
    
    [Header("Menu")]
    [SerializeField] private string menuScene;

    [Header("Loading")]
    [SerializeField] private float loadingDelay = 1f;
    [SerializeField] private float loadingSmoothSpeed = 0.5f;

    [InjectService] private IGameStateService _gameStateService;

    private float _splashProgress;
    private float _loadingProgress;

    public float SplashProgress => _splashProgress;
    public float LoadingProgress => _loadingProgress;

    public string MenuScene => menuScene;

    public event Action EventSplashStarted;
    public event Action EventLoadingStarted;
    public event Action EventMenuEntered;
    public event Action EventSceneLoaded;
    
    public override void Init()
    {
        ShowSplash(() => ShowMenu());        
    }

    public void ShowSplash(Action onComplete = null)
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
            StartCoroutine(LoadSceneAsyncCoroutine(sceneName, onComplete));
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
        yield return new WaitForSeconds(loadingDelay);

        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
        op.allowSceneActivation = false;

        while (!op.isDone)
        {
            float currentProgress = op.progress / 0.9f;
            _loadingProgress = Mathf.MoveTowards(_loadingProgress, currentProgress, Time.deltaTime * loadingSmoothSpeed);

            if (_loadingProgress >= 1f)
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
