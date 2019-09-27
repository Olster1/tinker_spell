using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Timer_namespace;
using EasyGameManager;

public class MainMenuContols : MonoBehaviour
{
  private int indexOn;
  public Text[] texts;
	public string sceneNameToLoad;
  private Timer waitTimer;
  private Timer[] glowTimers;
  private Timer fadeoutTimer;
  public Image panel;
  public AudioSource bgSound;
  public AudioSource changeSound;
  public AudioSource confirmSound;
  private bool readyToLoadLevel;
  private bool exit;

    // Start is called before the first frame update
    void Start()
    {
        waitTimer = new Timer(0.2f);
        waitTimer.turnOff();

        glowTimers = new Timer[texts.Length];
        for(int i = 0; i < texts.Length; ++i) {
          glowTimers[i] = new Timer(2.5f);
          if(i == 0) {
            glowTimers[i].turnOn();  
          } else {
            glowTimers[i].turnOff();  
          }
          
        }
        

        fadeoutTimer = new Timer(1.0f);
        fadeoutTimer.turnOff();
    }

    // Update is called once per frame
    void Update()
    {
        if(waitTimer.isOn()) {
          bool boo = waitTimer.updateTimer(Time.deltaTime);
          if(boo) {
            waitTimer.turnOff();
          }

        } else {
          float f = Input.GetAxis("Vertical");
          if(Mathf.Abs(f) > 0.25f) { //deadzone
            int lstIndex = indexOn;
            if(f > 0) {
              indexOn++;
              if(indexOn >= texts.Length) {
                indexOn = 0;
              }

            } else {
              indexOn--;
              if(indexOn < 0) {
                indexOn = texts.Length - 1;
              }
            }
            glowTimers[lstIndex].turnOff();
            glowTimers[indexOn].turnOn();

            waitTimer.turnOn();
            changeSound.Play();
          }

          if(Input.GetButtonDown("Jump")) {
            confirmSound.Play();
            if(indexOn == 0) {
              fadeoutTimer.turnOn();
              // LoadNewScene();
            } else if(indexOn == 1) {
              fadeoutTimer.turnOn();
              exit = true;
            }
          }
        }

        

        if(fadeoutTimer.isOn()) {
          bool bo = fadeoutTimer.updateTimer(Time.deltaTime);
          bgSound.volume = 1.0f - fadeoutTimer.getCanoncial();
          panel.color = new Color(panel.color.r, panel.color.b, panel.color.g, fadeoutTimer.getCanoncial());
          if(bo) {
            if(exit) {
              ExitGame();
            } else {  
              GameManager.manaCount = 100;
              GameManager.playerHealth = 100;
              GameManager.senintelHeadCount = 0;
              GameManager.hasEarth1 = true;
              GameManager.fuelCellCount = 0;
              fadeoutTimer.turnOff();
              SceneManager.LoadScene(sceneNameToLoad);
              // readyToLoadLevel = true;    
            }
            
          }
        }

    }

    public void LoadNewScene() {
		StartCoroutine(AsyncLoadScene());
    }

    public void ExitGame() {
		Application.Quit();
    }

    IEnumerator AsyncLoadScene() {
    	// yield return new WaitForSeconds(3);

    	AsyncOperation async = SceneManager.LoadSceneAsync(sceneNameToLoad);

       // While the asynchronous operation to load the new scene is not yet complete, continue waiting until it's done.
       while (true){//!async.isDone || !readyToLoadLevel) {
          // panel.color = new Color(panel.color.r, panel.color.b, panel.color.g, async.progress);
       		// if(async.progress > 0.5f && !setTrigger) {
       		// 	// panelFade.SetTrigger("FadeOut");
       		// 	setTrigger = true;
       		// }
           yield return null;
       }


    }

    void FixedUpdate() {
      for(int i = 0; i < texts.Length; ++i) {
        Timer glowTimer = glowTimers[i];
        if(glowTimer.isOn()) {
          float residue = (glowTimer.tAt + Time.fixedDeltaTime) - glowTimer.period;
          bool b = glowTimer.updateTimer(Time.fixedDeltaTime);
          float alpha = (float)-Mathf.Cos(2*Mathf.PI*glowTimer.getCanoncial()) + 1.0f;
          alpha = (alpha / 2.0f);
          // Debug.Log(alpha);
          texts[i].color = new Color(alpha, alpha, 0, 1);

          if(b) {
            if(indexOn == i) {
              glowTimer.turnOn();
            } else {
              glowTimer.turnOff();
            }
            
          }
        } else {
          texts[i].color = new Color(1, 1, 0, 1);
        }
      }
    }
}
