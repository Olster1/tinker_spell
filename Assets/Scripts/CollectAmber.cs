using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using EasyGameManager;

using Timer_namespace;


public enum AmberType {
    AMBER_HEALTH,
    AMBER_AMBER,
    AMBER_MANA,
    AMBER_SENTINEL_HEAD
}

public class CollectAmber : MonoBehaviour
{
    private Transform thisTrans;
    private float tAt;
    private float startY;
    private Timer fadeTimer;
    private AudioSource audioSrc;
    private BoxCollider2D col;
    private SpriteRenderer spRender;
    private bool gravityAffected;
    private Rigidbody2D amberRb;
    private worldTUI createAmberUIObject;
    private Timer forceTimer;

    public ActivateQuote quoteToActivate;

    public float relVel;

    public Sprite amberSprite;
    public Sprite manaSprite;
    public Sprite healthSprite;
    public Sprite sentinelHeadSprite;

    private GameObject player;
    

    public AmberType type;
    // Start is called before the first frame update
    void Start()
    {
        forceTimer = new Timer(0.3f);
        forceTimer.turnOn();
        GameObject amberUI = GameObject.Find("AmberUI");
        player = GameObject.Find("Player");
        createAmberUIObject = amberUI.GetComponent<worldTUI>();
        thisTrans = gameObject.GetComponent<Transform>();
        startY = thisTrans.position.y;
        fadeTimer = new Timer(0.3f);
        fadeTimer.turnOff();
        audioSrc = gameObject.GetComponent<AudioSource>();
        col = gameObject.GetComponent<BoxCollider2D>();
        spRender = gameObject.GetComponent<SpriteRenderer>();
        amberRb = gameObject.transform.parent.GetComponent<Rigidbody2D>();
        Assert.IsTrue(amberRb != null);
        gravityAffected = (amberRb.bodyType == RigidbodyType2D.Dynamic);

        switch(type) {
            case AmberType.AMBER_AMBER: {
                spRender.sprite = amberSprite;
            } break;
            case AmberType.AMBER_MANA: {
                spRender.sprite = manaSprite;
            } break;
            case AmberType.AMBER_HEALTH: {
                spRender.sprite = healthSprite;
            } break;
            case AmberType.AMBER_SENTINEL_HEAD: {
                spRender.sprite = sentinelHeadSprite;
            } break;
            default: {

            } break;
        }

        if(!gravityAffected) {
            forceTimer.turnOff();
        }
        
    }

    public void SetQuoteOnCollect(ActivateQuote quote) 
    {   
        this.quoteToActivate = quote;
    }   

    // Update is called once per frame
    void Update()
    {
        gravityAffected = (amberRb.bodyType == RigidbodyType2D.Dynamic);
        if(!gravityAffected) {

            tAt += Time.deltaTime;

            thisTrans.position = new Vector3(thisTrans.position.x, startY + Mathf.Sin(tAt), thisTrans.position.z);
        } else {
            if(forceTimer.isOn()) {
                bool bo = forceTimer.updateTimer(Time.deltaTime);
                if(bo) {
                    forceTimer.turnOff();
                }
            } else {
                Vector2 forceToPlayer = player.transform.position - thisTrans.position;
                amberRb.AddForce(1000*forceToPlayer);
            }
        }

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


    void collectAmber() {
        col.enabled = false;
        audioSrc.Play();
        fadeTimer.turnOn();
        amberRb.simulated = false;

        if(quoteToActivate != null) {
            quoteToActivate.Activate();
        }

        switch(type) {
            case AmberType.AMBER_AMBER: {
                createAmberUIObject.createAmberUI(gameObject.transform.position);
            } break;
            case AmberType.AMBER_MANA: {
                createAmberUIObject.createManaUI(gameObject.transform.position, transform.localScale);
            } break;
            case AmberType.AMBER_HEALTH: {
                createAmberUIObject.createHealthUI(gameObject.transform.position, transform.localScale);
            } break;
            case AmberType.AMBER_SENTINEL_HEAD: {
                GameManager.senintelHeadCount++;
                createAmberUIObject.createSentinelHeadUI(gameObject.transform.position);
            } break;
            default: {

            } break;
        }
    }

    private void OnTriggerStay2D(Collider2D other) {
        if (other.gameObject.tag == "Player" && !fadeTimer.isOn() && !forceTimer.isOn()) {
            collectAmber();
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.tag == "Player" && !fadeTimer.isOn() && !forceTimer.isOn()) {
            collectAmber();
        }
    }
}
