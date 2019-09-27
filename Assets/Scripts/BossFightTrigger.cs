using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using Timer_namespace;
using EasyGameManager;

public class BossFightTrigger : MonoBehaviour
{

	private Timer timer;
	public CameraFollowPlayer cam;
	public Transform camTrans;
	private bool active;
	public Vector3 offset;
	private Vector3 startP;
	public SoundMixer.MusicId musicId;
	public SoundMixer mixer;
  public SoundChanger soundChanger;
  public AudioSource audioSrc;

  public RectTransform anim1;
  public RectTransform anim2;
  private Timer blackBarsTimer;
  private bool bbOut;
  private ExperienceManager xpManager;
    // Start is called before the first frame update
    void Start()
    {
        timer = new Timer(0.5f);
        active = true;
        blackBarsTimer = new Timer(1.0f);
        blackBarsTimer.turnOff();

        xpManager = Camera.main.GetComponent<ExperienceManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if(timer.isOn()) {
        	bool b = timer.updateTimer(Time.deltaTime);
        	float f = timer.getCanoncial();
        	camTrans.position = Vector3.Lerp(startP, transform.position + offset, f);
        	if(b) {
        		timer.turnOff();
        		Assert.IsTrue(!active);
        	}

        }

        if(blackBarsTimer.isOn()) {
          bool b1 = blackBarsTimer.updateTimer(Time.deltaTime);
          float f1 = blackBarsTimer.getCanoncial();
          

          if(bbOut) {
            f1 = 1.0f - f1;
          } 

          float newY1 = Mathf.Lerp(25, -25, f1);
          float newY2 = Mathf.Lerp(-25, 25, f1);
          
          
          anim1.anchoredPosition = new Vector2(anim1.anchoredPosition.x, newY1);
          anim2.anchoredPosition = new Vector2(anim2.anchoredPosition.x, newY2);
          
          if(b1) {
            blackBarsTimer.turnOff();
            if(bbOut) {
              ////////
              Destroy(gameObject);
            }
          }

        }
    }

    public void bossDied() {
      soundChanger.enabled = false;
    	mixer.SetSound(musicId);
    	cam.followPlayer = true;
      //restore player health
      GameManager.playerHealth = xpManager.maxHealth;
      GameManager.updateHealth = true;
      bbOut = true;
      blackBarsTimer.turnOn();
      

    }

    public void OnTriggerEnter2D(Collider2D other) {
   		if (other.gameObject.name == "Player" && active) {
        audioSrc.Play();
   			cam.followPlayer = false;
   			timer.turnOn();
   			active = false;
   			startP = camTrans.position;
        bbOut = false;
        blackBarsTimer.turnOn();
   		}
   	}
}
