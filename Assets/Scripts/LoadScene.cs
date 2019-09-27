using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Timer_namespace;

public class LoadScene : MonoBehaviour
{
	public string sceneNameToLoad;
	public Animator panelFade;
	private bool setTrigger;
  public SpriteRenderer sp;
  private Timer fadeTimer;

    // Start is called before the first frame update
    void Start()
    {
    	LoadNewScene();
      fadeTimer = new Timer(0.5f);
      fadeTimer.turnOff();
    }

    // Update is called once per frame
    void Update()
    {
        // if(fadeTimer.isOn()) {
        //   bool b = fadeTimer.updateTimer(Time.deltaTime);
        //   float f = fadeTimer.getCanoncial();
        //   sp.color = new Color(1, 1, 1, 1.0f - f);
        //   if(b) {
        //     fadeTimer.turnOff();
        //   }
        // }
    }

    public void LoadNewScene() {
		StartCoroutine(AsyncLoadScene());
    }

    IEnumerator AsyncLoadScene() {
    	// yield return new WaitForSeconds(3);

    	AsyncOperation async = SceneManager.LoadSceneAsync(sceneNameToLoad);

       // While the asynchronous operation to load the new scene is not yet complete, continue waiting until it's done.
       while (!async.isDone) {
       		if(async.progress > 0.5f && !fadeTimer.isOn()) {
            // fadeTimer.turnOn();
       		// 	panelFade.SetTrigger("FadeOut");
       		// 	setTrigger = true;
       		}
          yield return null;
       }


    }


}
