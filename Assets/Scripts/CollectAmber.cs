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

    public ActivateQuote quoteToActivate;

    public float relVel;

    public Sprite amberSprite;
    public Sprite manaSprite;
    public Sprite healthSprite;
    public Sprite sentinelHeadSprite;
    

    public AmberType type;
    // Start is called before the first frame update
    void Start()
    {
        GameObject amberUI = GameObject.Find("AmberUI");
        createAmberUIObject = amberUI.GetComponent<worldTUI>();
        thisTrans = gameObject.GetComponent<Transform>();
        startY = thisTrans.position.y;
        fadeTimer = new Timer(0.5f);
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


    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.tag == "Player" && !fadeTimer.isOn()) {
            
            col.enabled = false;
            audioSrc.Play();
            fadeTimer.turnOn();

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

    }
}
