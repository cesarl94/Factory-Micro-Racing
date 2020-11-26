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

    void Awake()
    {
        PauseMenu.instance = this;
        enabled = false;
        Transform pauseTextNode = Utils.findNode(transform, "Text");
        pauseText = pauseTextNode.GetComponent<TextMeshProUGUI>();
        background = Utils.findNode(transform, "Background").GetComponent<Image>();
        RectTransform backgroundTransform = background.GetComponent<RectTransform>();
        Vector3 bgScale = backgroundTransform.localScale;
        bgScale.x = (float)Screen.width / 100;
        bgScale.y = (float)Screen.height / 100;
        backgroundTransform.localScale = bgScale;
        gameUI = Utils.findNode(transform.parent, "GameUI").GetComponent<GameUI>();
        activate(false);
    }

    public bool getState()
    {
        return enabled;
    }

    public void activate(bool state)
    {
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
        if (Time.realtimeSinceStartup - lastTimeChecked > textSwitchTime)
        {
            pauseText.enabled = !pauseText.enabled;
            lastTimeChecked += textSwitchTime;
        }
    }

    public void reset()
    {
        LevelParser.instance.resetGame();
        activate(false);
    }
}
