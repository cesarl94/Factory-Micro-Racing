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

    private RectTransform gameTitle;
    private RectTransform playButton;
    private float gameTitleOriginalScale;
    private float gameTitleOriginalRotation;

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
        gameTitleOriginalScale = gameTitle.localScale.x;
        gameTitleOriginalRotation = gameTitle.localRotation.z;
    }

    public bool getState()
    {
        return enabled;
    }

    public void activate(bool state)
    {
        if (!enabled && gameUI.gameObject.active)
        {
            //Es el ready set go
            Debug.Log("HOLA");
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
        else if (Time.realtimeSinceStartup - lastTimeChecked > textSwitchTime)
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
