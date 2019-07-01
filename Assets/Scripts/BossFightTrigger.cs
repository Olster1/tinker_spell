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
    // Start is called before the first frame update
    void Start()
    {
        timer = new Timer(0.5f);
        active = true;
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
    }

    public void bossDied() {
    	mixer.SetSound(musicId);
    	cam.followPlayer = true;
      //restore player health
      GameManager.playerHealth = 100;
      GameManager.updateHealth = true;

      ////////
      Destroy(gameObject);

    }

    public void OnTriggerEnter2D(Collider2D other) {
   		if (other.gameObject.name == "Player" && active) {
   			cam.followPlayer = false;
   			timer.turnOn();
   			active = false;
   			startP = camTrans.position;
   		}
   	}
}
