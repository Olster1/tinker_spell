using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Timer_namespace;
using UnityEngine.SceneManagement;

public class TeleporterTrigger : MonoBehaviour
{
	private Timer timer;
	private bool shown;
	public string sceneNameToLoad;
	private PlayerMovement player;
	private Image fadePanel;
    private MyAssetBundleManager assetManager;

    // Start is called before the first frame update
    void Start()
    {
         // //Loading the animation spiral
        assetManager = Camera.main.GetComponent<MyAssetBundleManager>();
        MyAssetBundle a = assetManager.LoadBundle("AssetBundles/teleporter_spiral_animation");
        Object[] controller = assetManager.GetAssetsOfType(a, typeof(UnityEngine.RuntimeAnimatorController));  
        gameObject.GetComponent<Animator>().runtimeAnimatorController = (RuntimeAnimatorController)controller[0];
        // ////

        MySceneManager myManager = Camera.main.GetComponent<MySceneManager>();
        player = myManager.playerMovement;
        fadePanel = myManager.fadePanel;
        timer = new Timer(1.4f);
        timer.turnOff();

        gameObject.SetActive(false); //set inactive until it gets woken up again by tinker
    }

    // Update is called once per frame
    void Update()
    {
        if(timer.isOn()) {
        	player.canControlPlayer = false;
        	bool b = timer.updateTimer(Time.deltaTime);
        	fadePanel.color = new Color(0, 0, 0, timer.getCanoncial());
        	if(b) {
                assetManager.EmptyAllAssetBundles();
        		SceneManager.LoadScene(sceneNameToLoad);
        	}
        }
    }
    public void OnTriggerExit2D(Collider2D other) {

    }

     public void OnTriggerStay2D(Collider2D other) {
		if (other.gameObject.name == "Player" && Input.GetButtonDown("Fire4")) {
			
     		timer.turnOn();
		}
	}
}
