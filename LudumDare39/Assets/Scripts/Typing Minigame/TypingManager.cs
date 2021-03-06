﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TypingManager : AbstarcMinigameManager
{
    private bool go=false;

    [SerializeField]
    private GameObject letterFallPref;

    private float fallingVelocity=5.0f;
    private float timeSpawnLetter =1.0f;
    private float timeGame=20.0f;
    private float timer=0.0f;
    private int numLet=3;
    private float glitchVelocity;

    [SerializeField]
    private GameObject letterTriggerPref;
    [SerializeField]
    private AudioClip audioHit;
    [SerializeField]
    private AudioClip audioMiss;
    [SerializeField]
    private Canvas canvas;
    private Canvas newCanvas;
    private AudioSource playEffect;
    [SerializeField]
    private List<KeyCode> typeCode;
    private Queue<GameObject> letterFallPool;

    [SerializeField]
    private List<GameObject> letterTriggerList;
    private int points = 0;
    private int maxPoint = 0;

    [SerializeField]
    private List<Sprite> typeItem;

    public override void StartMinigame(int i_Type, float i_TimeValue, float i_NumItemValue, float i_ItemVelocityValue)
    {
        ColorizeBckManager.BckTypes type = ColorizeBckManager.BckTypes.BCK_PLANNING;
        switch (i_Type)
        {
            case 0: type = ColorizeBckManager.BckTypes.BCK_PHYSICS; break;
            case 1: type = ColorizeBckManager.BckTypes.BCK_MONEY; break;
            case 2: type = ColorizeBckManager.BckTypes.BCK_SOCIAL; break;
        }
        FindObjectOfType<ColorizeBckManager>().SetUncoloredBckType(type);

        numLet = 3 + Mathf.Clamp((int)Mathf.Lerp(4.0f, 0.0f, i_NumItemValue),0,3);
        timeGame = 10.0f + (10.0f * i_TimeValue);
        glitchVelocity = Mathf.Lerp(30.0f, 0.0f, i_ItemVelocityValue);

        playEffect = gameObject.AddComponent<AudioSource>();
        Camera camera = FindObjectOfType<Camera>();
        newCanvas = Instantiate(canvas);
        newCanvas.worldCamera = camera;
        letterTriggerList = new List<GameObject>();

        float left = camera.orthographicSize * camera.aspect * -1;
        float space = (camera.orthographicSize * camera.aspect * 2) / (numLet + 1);
        for (int index = 1; index <= numLet; index++)
        {
            GameObject letterTrigger = Instantiate(letterTriggerPref, new Vector3(left + (space * index), (camera.orthographicSize * 0.85f) * -1, -1), Quaternion.identity, newCanvas.transform);//0.9f= 90% of camera hight
            letterTriggerList.Add(letterTrigger);
            letterTrigger.name = typeCode[index - 1].ToString();
            Transform text = letterTrigger.transform.Find("Text");

            text.GetComponent<Text>().text = typeCode[index - 1].ToString();
            LetterTrigger letterListener = letterTrigger.GetComponent<LetterTrigger>();
            letterListener.MyKey = typeCode[index - 1];
            letterListener.onLetterHit += OnLetterHit;
            letterListener.onLetterMiss += OnLetterMiss;
        }
        letterFallPool = new Queue<GameObject>();
        for (int i = 0; i < 10; i++)
        {
            GameObject letterFalling = Instantiate(letterFallPref);
            letterFalling.GetComponent<SpriteRenderer>().sprite = typeItem[i_Type];
            LetterFall letterFallListener = letterFalling.GetComponent<LetterFall>();
            letterFallListener.onLetterPass += OnLetterPass;
            letterFalling.SetActive(false);
            letterFallPool.Enqueue(letterFalling);
        }
        StartTimer(gameObject.transform, begin, endGame, "Texting", "Press the corresponding key to take the item ", 3, "Time out", (int)timeGame);
    }

    public void begin()
    {
        go = true;
    }

    //for testing alone
    /*void Start()
    {
        StartMinigame(1, 1.0f, 0.0f, 0.0f);

    }*/

    public void Update()
    {
        if (go)
        {
            timer -= Time.deltaTime;
            timeGame -= Time.deltaTime;

            if (timer < 0.0f && timeGame > 0.0f)
            {
                timer = timeSpawnLetter + UnityEngine.Random.Range(-0.4f, 0.4f);
                GameObject letterFallSpawn = letterFallPool.Dequeue();
                int random = UnityEngine.Random.Range(0, letterTriggerList.Count);
                float positionX = letterTriggerList[random].transform.position.x;

                letterFallSpawn.transform.position = new Vector3(positionX, (Camera.main.orthographicSize * 1.2f), 0.0f);
                letterFallSpawn.SetActive(true);
                float addGlitchVelocity = 0.0f;
                if (glitchVelocity > UnityEngine.Random.Range(1, 101))
                {
                    addGlitchVelocity = 2.0f;
                }
                letterFallSpawn.GetComponent<LetterFall>().Velocity = fallingVelocity + addGlitchVelocity;
            }

        }
    }

    public void endGame()
    {
        float finalScore = ((100.0f / maxPoint) * points) / 100;
        SceneEnded(finalScore);
    }

    public void OnLetterHit(GameObject letterFall)
    {
        letterFall.SetActive(false);
        letterFallPool.Enqueue(letterFall);
        playEffect.PlayOneShot(audioHit,0.3f);
        points++;
        maxPoint++;
        newCanvas.GetComponent<Animator>().SetTrigger("onResizeHit");
    }

    public void OnLetterPass(GameObject letterFall)
    {
        letterFall.SetActive(false);
        letterFallPool.Enqueue(letterFall);
        newCanvas.GetComponent<Animator>().SetTrigger("onResizePass");
        maxPoint++;
    }

    public void OnLetterMiss()
    {
        if (go)
        {
            newCanvas.GetComponent<Animator>().SetTrigger("onResizeMiss");
            playEffect.PlayOneShot(audioMiss, 1.0f);
            timeGame -= 0.3f;
            DecreaseTime(0.3f);
        }
    }


}
