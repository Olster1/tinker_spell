using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControlMagic : MonoBehaviour
{
	public GameObject[] objects;
    public PlayerMovement pm;
    public Image circle1;
    public Image circle2;
    public Image circle3;
    // Start is called before the first frame update
    void Start()
    {
        circle1.fillAmount = 0.0f;   
        circle2.fillAmount = 0.0f;   
        circle3.fillAmount = 0.0f;   
    }

    // Update is called once per frame
    void Update()
    {
        if(pm.earthTimer.isOn()) {
            bool f = pm.earthTimer.updateTimer(Time.deltaTime);
            circle1.fillAmount = pm.earthTimer.getCanoncial();

            if(f) {
                circle1.fillAmount = 0.0f;
                pm.earthTimer.turnOff();
            }
        }

        if(pm.fireTimer.isOn()) {
            bool f = pm.fireTimer.updateTimer(Time.deltaTime);
            circle2.fillAmount = pm.fireTimer.getCanoncial();
            
            if(f) {
                circle2.fillAmount = 0.0f;
                pm.fireTimer.turnOff();
            }
        }

        if(pm.waterTimer.isOn()) {
            bool f = pm.waterTimer.updateTimer(Time.deltaTime);
            circle3.fillAmount = pm.waterTimer.getCanoncial();
            
            if(f) {
                circle3.fillAmount = 0.0f;
                pm.waterTimer.turnOff();
            }
        }
        if(Input.GetButton("Fire4")) {
        	for(int i = 0; i < objects.Length; ++i) {
        		objects[i].SetActive(true);
        	}    		
    	} else {
			for(int i = 0; i < objects.Length; ++i) {
				objects[i].SetActive(false);
			}    		
    	}
    }
}
