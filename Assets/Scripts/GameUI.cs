﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameUI : MonoBehaviour
{
    [SerializeField] Texture2D carTexture;
    TextMeshProUGUI positions;
    TextMeshProUGUI laps;
    TextMeshProUGUI velocity;
    Image[] carSprites;

    //Uso el Start y no el Awake porque necesito que el level parser ya se haya creado
    void Start()
    {
        List<Image> carSpritesList = new List<Image>();
        Transform positionsNode = Utils.findNode(transform, "Positions");
        positions = positionsNode.GetComponent<TextMeshProUGUI>();
        Transform carSpriteNode = Utils.findNode(positionsNode, "CarSprite");
        carSpritesList.Add(carSpriteNode.GetComponent<Image>());
        Driver[] drivers = LevelParser.instance.sortedDrivers;
        int duplicateCount = drivers.Length - 1;
        for (int i = 0; i < duplicateCount; i++)
        {
            Transform newCarSpriteNode = Instantiate(carSpriteNode.gameObject, carSpriteNode.parent).transform;
            newCarSpriteNode.position -= new Vector3(0, positions.fontSize * (i + 1), 0);
            carSpritesList.Add(newCarSpriteNode.GetComponent<Image>());
        }
        carSprites = carSpritesList.ToArray();

        Transform lapsNode = Utils.findNode(transform, "Laps");
        laps = lapsNode.GetComponent<TextMeshProUGUI>();
        Transform velocityNode = Utils.findNode(transform, "Velocity");
        velocity = velocityNode.GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        Driver[] sortedDrivers = LevelParser.instance.sortedDrivers;
        string positionsText = "Positions:\n";

        for (int i = 0; i < sortedDrivers.Length; i++)
        {
            Driver driver = sortedDrivers[i];
            positionsText += "     " + (i + 1).ToString() + ") " + driver.name + "\n";
            carSprites[i].color = carTexture.GetPixel(driver.car.color, 14) + new Color(0.1f, 0.1f, 0.1f);

        }

        positions.text = positionsText;

        Driver player = LevelParser.instance.player;
        laps.text = "Laps " + Mathf.Max(player.laps + 1, 1) + "/" + LevelParser.instance.raceInfo.laps;
        velocity.text = Mathf.FloorToInt(player.car.velocity).ToString() + " KM/H";
    }
}
