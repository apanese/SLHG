using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    private List<int> foodId = new List<int>();
    public Transform LoadUI;

    public AudioSource UIAudioSource;

    public static GameManager.GameMode GameMode;

    private int HighScore;
    private int TimeHighScore;

    public Text HighScoreText;
    public Text TimeHighScoreText;
    // Start is called before the first frame update
    AsyncOperation async;
    //UGU
    public Slider m_pProgress;
    private int progress = 0;
    // Use this for initialization
    private void Awake()
    {
        HighScore = PlayerPrefs.GetInt("HighScore", 0);
        TimeHighScore = PlayerPrefs.GetInt("TimeHighScore", 0);
        if (PlayerPrefs.GetInt("IsReadNT")==0)
        {
            PlayerPrefs.SetInt("IsReadNT", 1);
            OnStandardModeButtonClick();
        }
    }
    void Start()
    {
        HighScoreText.text = HighScore.ToString();
        TimeHighScoreText.text = TimeHighScore.ToString();
    }
    IEnumerator LoadScenes()
    {
        int nDisPlayProgress = 0;
        async = SceneManager.LoadSceneAsync("Game");
        async.allowSceneActivation = false;
        while (async.progress < 0.9f)
        {
            progress = (int)async.progress * 100;
            while (nDisPlayProgress < progress)
            {
                ++nDisPlayProgress;
                m_pProgress.value = (float)nDisPlayProgress / 100;
                yield return new WaitForEndOfFrame();
            }
            yield return null;
        }
        progress = 100;
        while (nDisPlayProgress < progress)
        {
            ++nDisPlayProgress;
            m_pProgress.value = (float)nDisPlayProgress / 100;
            yield return new WaitForEndOfFrame();
        }
        async.allowSceneActivation = true;

    }
    // Update is called once per frame
    void Update()
    {
        
    }

    void ChooseFood()
    {
        for (int i = 0; i < 18; i++)
        {
            foodId.Add(i + 1001);
        }
        foodId.Add(1022);
        foodId.Add(1023);
        foodId.Add(1024);
        foodId.Add(1025);
        foodId.Add(1026);
        foodId.Add(1027);
    }
    //读取本局菜品
    void ReadGameFood()
    {
        bool successful = true;
        foreach (int i in foodId)
        {
          /*  Debug.Log(i);
            if(StartManager.GetFoodData(i)!=null)
            {
                Debug.Log(GameManager.SetGameFoodData(StartManager.GetFoodData(i)));
                Debug.Log(StartManager.GetFoodData(i).Id);
            }*/
           successful=successful && GameManager.SetGameFoodData(StartManager.GetFoodData(i));
        }
        //Debug.Log(successful);
        if (successful)
        {
            GameManager.CheckIniFoodData();
        }
        else
        {
            Debug.Log("IniFoodData is failed!");
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
    //设置本局菜品
    void SetGameFood()
    {
        ChooseFood();
        ReadGameFood();
    }
    //点击上菜
    public void OnStandardModeButtonClick()
    {
        PlayUIAudioSource();
        SetGameFood();
        LoadUI.gameObject.SetActive(true);
        GameMode = GameManager.GameMode.Standard;
        StartCoroutine(LoadScenes());
    }
    public void OnTimeModeButtonClick()
    {
        PlayUIAudioSource();
        SetGameFood();
        LoadUI.gameObject.SetActive(true);
        GameMode = GameManager.GameMode.Time;
        StartCoroutine(LoadScenes());
    }
    public void OnGoStartButtonClick()
    {
        PlayUIAudioSource();
        SceneManager.LoadScene("Start");
    }

    private void PlayUIAudioSource()
    {
        UIAudioSource.Play();
    }
}
