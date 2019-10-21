using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using Timer_namespace;
using EasyGameManager;

public class ActivateQuote : MonoBehaviour
{
	public Animator quoteAnimator;
    public AudioSource soundSource;
    public GameObject uiImage;
    public string[] dialog;
    public TextWriter writer;
    private bool fadeIn;

    public bool unfreezePlayer;

    public bool setManual;
    private Journal journal;

    // public bool completeJournalItem;
    public bool hasJournalItem;
    public string journalItemName;
    public string journalItemSynopsis;

    public GameObject[] imageObjs;

    public GameObject toActivate;
    public GameObject toDeactivate;

    public DialogQuestionEvent eventToCall;

    public AudioClip[] clips;

    public enum QuoteImage {
        TINKER_HEAD,
        SPELL_HEAD,
        TREE_DIETY_HEAD,
        GOBLIN_HEAD,
    }

    public SpriteRenderer sp;
    private Timer fadeTimer;

    public bool automatic;
    private bool played;
    private bool firstPlay;

    public bool fuelCellEffected;
    public int fuelCellsNeeded;

    public QuoteImage quoteType;

    public BoxCollider2D boxCollider;
    
    // Start is called before the first frame update
    void Start()
    {
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

    void updateTime() {
        
    }
    // Update is called once per frame
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
            if(quoteType == QuoteImage.TINKER_HEAD) {
                imageObjs[0].SetActive(true);
                imageObjs[1].SetActive(false);
                imageObjs[2].SetActive(false);
                imageObjs[3].SetActive(false);
            } else if(quoteType == QuoteImage.TREE_DIETY_HEAD) {
                imageObjs[1].SetActive(true);
                imageObjs[0].SetActive(false);
                imageObjs[2].SetActive(false);
                imageObjs[3].SetActive(false);
            } else if(quoteType == QuoteImage.SPELL_HEAD) {
                imageObjs[2].SetActive(true);
                imageObjs[1].SetActive(false);
                imageObjs[0].SetActive(false);
                imageObjs[3].SetActive(false);
            } else if(quoteType == QuoteImage.GOBLIN_HEAD) {
                imageObjs[2].SetActive(false);
                imageObjs[1].SetActive(false);
                imageObjs[0].SetActive(false);
                imageObjs[3].SetActive(true);
            }
            
            // interact.SetTrigger("Out");
            fadeTimer.turnOn();
            fadeIn = false;

            // soundSource.Play();
            // uiImage.SetActive(false);
            return true;
        } else {
            return false;            
        }

    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(!setManual) {
            if (other.gameObject.tag == "Player" && !writer.showText) {
                // interact.SetTrigger("In");
                fadeTimer.turnOn();
                fadeIn = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (other.gameObject.tag == "Player" && !setManual) {
            // interact.SetTrigger("Out");
            fadeTimer.turnOn();
            fadeIn = false;
        	// quoteAnimator.SetTrigger("OffscreenOut");
         //    soundSource.Stop();
         //    uiImage.SetActive(true);
        }
    }
}
