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

    public SpriteRenderer sp;
    private Timer fadeTimer;

    public bool automatic;
    private bool played;
    private bool firstPlay;

    public bool fuelCellEffected;
    public int fuelCellsNeeded;

    public BoxCollider2D boxCollider;
    // Start is called before the first frame update
    void Start()
    {
        fadeTimer = new Timer(0.5f);
        fadeTimer.turnOff();
        sp.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);

        if(fuelCellEffected) {
            if(GameManager.fuelCellCount == fuelCellsNeeded) {
                boxCollider.enabled = true;
            } else {
                boxCollider.enabled = false;
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
            } else {
                boxCollider.enabled = false;
            }
        }

        if(fadeTimer.isOn()) {
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

    private void OnTriggerStay2D(Collider2D other) {
        if (other.gameObject.tag == "Player" && !writer.showText && (Input.GetButtonDown("Jump") || (automatic && !played && !firstPlay))) {
            firstPlay = true;
        	quoteAnimator.SetTrigger("OffscreenIn");   
            writer.stringArray = dialog;
            
            // interact.SetTrigger("Out");
            fadeTimer.turnOn();
            fadeIn = false;

            // soundSource.Play();
            // uiImage.SetActive(false);

        }

        if(!writer.showText && !fadeIn && !fadeTimer.isOn()) {
            fadeTimer.turnOn();
            fadeIn = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.tag == "Player" && !writer.showText) {
            // interact.SetTrigger("In");
            fadeTimer.turnOn();
            fadeIn = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (other.gameObject.tag == "Player") {
            // interact.SetTrigger("Out");
            fadeTimer.turnOn();
            fadeIn = false;
        	// quoteAnimator.SetTrigger("OffscreenOut");
         //    soundSource.Stop();
         //    uiImage.SetActive(true);
        }
    }
}
