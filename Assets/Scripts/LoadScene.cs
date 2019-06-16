using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
	public string sceneNameToLoad;
	public Animator panelFade;
	private bool setTrigger;

    // Start is called before the first frame update
    void Start()
    {
    	LoadNewScene();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadNewScene() {
		StartCoroutine(AsyncLoadScene());
    }

    IEnumerator AsyncLoadScene() {
    	// yield return new WaitForSeconds(3);

    	AsyncOperation async = SceneManager.LoadSceneAsync(sceneNameToLoad);

       // While the asynchronous operation to load the new scene is not yet complete, continue waiting until it's done.
       while (!async.isDone) {
       		if(async.progress > 0.5f && !setTrigger) {
       			panelFade.SetTrigger("FadeOut");
       			setTrigger = true;
       		}
           yield return null;
       }


    }


}
