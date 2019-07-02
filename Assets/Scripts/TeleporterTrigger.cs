using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Timer_namespace;
using UnityEngine.SceneManagement;

public class TeleporterTrigger : MonoBehaviour
{
	public SpriteRenderer interactSp;
	private Timer timer;
	private bool shown;
	public string sceneNameToLoad;
	public PlayerMovement player;
	public Image fadePanel;
	
    // Start is called before the first frame update
    void Start()
    {
        timer = new Timer(1.4f);
        timer.turnOff();
    }

    // Update is called once per frame
    void Update()
    {
        if(timer.isOn()) {
        	player.canControlPlayer = false;
        	bool b = timer.updateTimer(Time.deltaTime);
        	fadePanel.color = new Color(0, 0, 0, timer.getCanoncial());
        	if(b) {
        		SceneManager.LoadScene(sceneNameToLoad);
        	}
        }
    }
    public void OnTriggerExit2D(Collider2D other) {

    }

     public void OnTriggerStay2D(Collider2D other) {
		if (other.gameObject.name == "Player" && Input.GetButtonDown("Fire1")) {
			
     		timer.turnOn();
		}
	}
}
