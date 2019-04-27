﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EasyGameManager;
using Timer_namespace;

public class CollectAmber : MonoBehaviour
{
    private Transform thisTrans;
    private float tAt;
    private float startY;
    private Timer fadeTimer;
    private AudioSource audioSrc;
    private BoxCollider2D col;
    private SpriteRenderer spRender;
    // Start is called before the first frame update
    void Start()
    {
        thisTrans = gameObject.GetComponent<Transform>();
        startY = thisTrans.position.y;
        fadeTimer = new Timer(0.5f);
        fadeTimer.turnOff();
        audioSrc = gameObject.GetComponent<AudioSource>();
        col = gameObject.GetComponent<BoxCollider2D>();
        spRender = gameObject.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        tAt += Time.deltaTime;

        thisTrans.position = new Vector3(thisTrans.position.x, startY + Mathf.Sin(tAt), thisTrans.position.z);

        if(fadeTimer.isOn()) {
            bool finished = fadeTimer.updateTimer(Time.deltaTime);
            float alphaVal = Mathf.Lerp(1, 0, fadeTimer.getCanoncial());
            spRender.color = new Vector4(spRender.color.r, spRender.color.g, spRender.color.b, alphaVal); 
            if(finished) {
                fadeTimer.turnOff();
                gameObject.SetActive(false);
            }

        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.tag == "Player") {
            GameManager.amberCount++;
            col.enabled = false;
            audioSrc.Play();
            fadeTimer.turnOn();
        }

    }
}
