using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Timer_namespace;

public class CardFlip : MonoBehaviour
{
	public bool active;
	private Timer t;
	private bool hidden;
    // Start is called before the first frame update
    void Start()
    {
     	t = new Timer(0.5f);
     	t.turnOff();   
     	hidden = false;
    }

    // Update is called once per frame
    void Update()
    {
    	if(active) {
    		t.turnOn();
    		active = false;
    		hidden = !hidden;
    	}
        if(t.isOn()) {
        	bool b = t.updateTimer(Time.deltaTime);
        	float at = Mathf.Lerp(hidden ? 0 : 180, hidden ? 180 : 0, t.getCanoncial());
        	transform.rotation =  Quaternion.Euler(0, at, 0);

        	if(b) {
        		t.turnOff();
        	}
        }
    }
}
