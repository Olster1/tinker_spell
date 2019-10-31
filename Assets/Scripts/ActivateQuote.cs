using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using Timer_namespace;
using EasyGameManager;



public enum QuoteImage {
    TINKER_HEAD,
    SPELL_HEAD,
    TREE_DIETY_HEAD,
    GOBLIN_HEAD,
}

    
public class ActivateQuote : MonoBehaviour
{
	
    public AudioSource soundSource;
    public string[] dialog;

    private Animator quoteAnimator;
    private TextWriter writer;
    private bool fadeIn;

    public bool unfreezePlayer;

    public bool setManual;
    private Journal journal;

    public bool hasJournalItem;
    public string journalItemName;
    public string journalItemSynopsis;

    public GameObject toActivate;
    public GameObject toDeactivate;

    public DialogQuestionEvent eventToCall;

    public AudioClip[] clips;

    public SpriteRenderer sp;
    private Timer fadeTimer;

    public bool automatic;
    private bool played;
    private bool firstPlay;

    public bool fuelCellEffected;
    public int fuelCellsNeeded;

    public QuoteImage quoteType;

    public BoxCollider2D boxCollider;
    
    void Start()
    {
        MySceneManager myManager = Camera.main.GetComponent<MySceneManager>();

        quoteAnimator = myManager.quoteAnimator;
        writer = myManager.textWriter;

        fadeTimer = new Timer(0.5f);
        fadeTimer.turnOff();
        sp.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);

        journal = Camera.main.GetComponent<Journal>();

        if(fuelCellEffected) {
            if(GameManager.fuelCellCount == fuelCellsNeeded) {
                boxCollider.enabled = true;
                sp.enabled = true;
            } else {
                boxCollider.enabled = false;
                sp.enabled = false;
            }
        }
    }

    void Update()
    {
        if(fuelCellEffected) {
            if(GameManager.fuelCellCount == fuelCellsNeeded) {
                boxCollider.enabled = true;
                sp.enabled = true;
            } else {
                boxCollider.enabled = false;
                sp.enabled = false;
            }
        }

        if(fadeTimer.isOn() && sp.enabled) {
            bool b = fadeTimer.updateTimer(Time.deltaTime);
            float f = fadeTimer.getCanoncial();
            if(!fadeIn) {
                f = 1.0f - f;
            }
            if(automatic && !played) {

            } else {
                sp.color = new Color(1.0f, 1.0f, 1.0f, f);        
            }
            
            if(b) {
                played = true;
                fadeTimer.turnOff();
            }
        }

    }

    public void EndQuote() {
        if(hasJournalItem && !journal.hasJournalItem(gameObject.GetInstanceID())) {
            journal.AddJournalItem(journalItemName, journalItemSynopsis, gameObject.GetInstanceID());

        }
    } 

    private void OnTriggerStay2D(Collider2D other) {
        if(!setManual) {
            if(!writer.showText) {
                if (other.gameObject.tag == "Player" && (Input.GetButtonDown("Fire4") || (automatic && !played && !firstPlay))) {
                    Activate();
                    Debug.Log("ACTIVATING");
                }

                if(!writer.showText && !fadeIn && !fadeTimer.isOn()) {
                    fadeTimer.turnOn();
                    fadeIn = true;
                }
            }
        }
    }

    public bool Activate() {
        if(!writer.showText) {
            firstPlay = true;
            quoteAnimator.SetTrigger("OffscreenIn");   
            writer.stringArray = dialog;
            writer.dialogEvent = eventToCall;
            writer.clips = clips;
            
            writer.currentQuote = this;

            writer.SetRightQuoteImage(quoteType);

            fadeTimer.turnOn();
            fadeIn = false;

            return true;
        } else {
            return false;            
        }

    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(!setManual) {
            
            if (other.gameObject.tag == "Player" && !writer.showText) {
                fadeTimer.turnOn();
                fadeIn = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (other.gameObject.tag == "Player" && !setManual) {
            fadeTimer.turnOn();
            fadeIn = false;
        }
    }
}
