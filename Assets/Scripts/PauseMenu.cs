using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PauseMenu : MonoBehaviour
{
    public static PauseMenu instance;
    [SerializeField] float textSwitchTime;
    private TextMeshProUGUI pauseText;
    private Image background;
    private float lastTimeChecked;
    private GameUI gameUI;

    private Transform credits;
    private Transform continueButton;

    private RectTransform gameTitle;
    private RectTransform playButton;

    private AudioSource playSound;

    private TextMeshProUGUI lastLap;
    private float lastLapTimer;
    private TextMeshProUGUI youWin;
    private TextMeshProUGUI youLose;

    void Awake()
    {
        PauseMenu.instance = this;
        Transform pauseTextNode = Utils.findNode(transform, "Text");
        pauseText = pauseTextNode.GetComponent<TextMeshProUGUI>();
        background = Utils.findNode(transform, "Background").GetComponent<Image>();
        RectTransform backgroundTransform = background.GetComponent<RectTransform>();
        Vector3 bgScale = backgroundTransform.localScale;
        bgScale.x = (float)Screen.width / 100;
        bgScale.y = (float)Screen.height / 100;
        backgroundTransform.localScale = bgScale;
        gameUI = Utils.findNode(transform.parent, "GameUI").GetComponent<GameUI>();

        pauseText.enabled = false;
        gameUI.gameObject.SetActive(false);
        Time.timeScale = 0;

        playButton = Utils.findNode(transform, "PlayButton").GetComponent<RectTransform>(); ;
        gameTitle = Utils.findNode(transform, "GameTitle").GetComponent<RectTransform>(); ;

        playSound = GetComponent<AudioSource>();

        lastLap = Utils.findNode(transform, "LastLap").GetComponent<TextMeshProUGUI>();
        youWin = Utils.findNode(transform, "Win").GetComponent<TextMeshProUGUI>();
        youLose = Utils.findNode(transform, "Lost").GetComponent<TextMeshProUGUI>();
        credits = Utils.findNode(transform, "Credits");
        continueButton = Utils.findNode(transform, "ContinueButton");
    }

    public void showLastLap()
    {
        StartCoroutine(lastLapCoroutine());
    }

    private IEnumerator lastLapCoroutine()
    {
        lastLap.enabled = true;
        yield return StartCoroutine(CoroutineUtil.WaitForRealSeconds(1));
        lastLap.enabled = false;
    }

    public void endRace(bool win)
    {
        activate(true);
        continueButton.gameObject.SetActive(false);
        pauseText.enabled = false;
        if (win)
        {
            youWin.enabled = true;
        }
        else
        {
            youLose.enabled = true;
        }

        foreach (Driver driver in LevelParser.instance.sortedDrivers)
        {
            driver.car.engine.Stop();
        }
    }

    public bool getState()
    {
        return enabled;
    }

    public void activate(bool state)
    {
        if (!state && (youWin.enabled || youLose.enabled))
        {
            //Queremos quitar la pausa cuando la partida terminó
            return;
        }
        if (!enabled && pauseText.text != "PAUSE")
        {
            //Es el ready set go
            return;
        }
        if (gameTitle != null)
        {
            //Es el menu principal
            playGame();
            return;
        }

        enabled = state;
        pauseText.enabled = state;
        background.enabled = state;
        credits.gameObject.SetActive(state);

        foreach (Button button in GetComponentsInChildren<Button>(true))
        {
            button.gameObject.SetActive(state);
        }

        gameUI.gameObject.SetActive(!state);

        lastTimeChecked = Time.realtimeSinceStartup;
        Time.timeScale = state ? 0 : 1;
    }



    void Update()
    {
        if (gameTitle != null)
        {
            float scale = 0.70f + (Mathf.Sin(Time.realtimeSinceStartup) + 0.5f) * 0.1f;
            float rotation = Mathf.Cos(Time.realtimeSinceStartup * 0.2f) * 5f;
            gameTitle.localScale = new Vector3(scale, scale, 1);
            gameTitle.localRotation = Quaternion.Euler(new Vector3(0, 0, rotation));

            float buttonScale = 1.70f + (Mathf.Sin(Time.realtimeSinceStartup * 3f) + 0.5f) * 0.1f;
            playButton.localScale = new Vector3(buttonScale, buttonScale, 1);

            if (Input.GetKeyDown(KeyCode.Space))
            {
                playGame();
            }
        }
        else if (Time.realtimeSinceStartup - lastTimeChecked > textSwitchTime && !youWin.enabled && !youLose.enabled)
        {
            pauseText.enabled = !pauseText.enabled;
            lastTimeChecked += textSwitchTime;
        }
    }

    public void reset()
    {

        StartCoroutine(readySetGo());
    }

    public void playGame()
    {
        Destroy(gameTitle.gameObject);
        gameTitle = null;
        Destroy(playButton.gameObject);
        playButton = null;
        StartCoroutine(readySetGo());
    }

    private IEnumerator readySetGo()
    {
        LevelParser.instance.resetGame();
        playSound.Play();
        lastLap.enabled = false;
        youWin.enabled = false;
        youLose.enabled = false;
        activate(true);
        enabled = false;
        foreach (Button button in GetComponentsInChildren<Button>(true))
        {
            button.gameObject.SetActive(false);
        }

        gameUI.gameObject.SetActive(true);


        pauseText.text = "Ready?";
        yield return StartCoroutine(CoroutineUtil.WaitForRealSeconds(0.75f));
        pauseText.text = "Set";
        yield return StartCoroutine(CoroutineUtil.WaitForRealSeconds(0.75f));
        pauseText.text = "GO!";
        yield return StartCoroutine(CoroutineUtil.WaitForRealSeconds(0.75f));
        pauseText.text = "PAUSE";
        //Lo ponemos en true e inmediatamente despues lo seteamos a falso, para que no crea que seguimos en el readySetGo
        enabled = true;
        activate(false);
    }
}
