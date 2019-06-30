using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Timer_namespace;

public class AreaTextTrigger : MonoBehaviour
{
	private Timer timer;
	public Text text;
	public string placeStr;
    public bool showOnce;
    public bool showed;
    // Start is called before the first frame update
    void Start()
    {
        timer = new Timer(1.5f);
    }

    // Update is called once per frame
    void Update()
    {
        if(timer.isOn()) {

        	bool b = timer.updateTimer(Time.deltaTime);
        	float f = timer.getCanoncial();
        	float alpha = ((float)-Mathf.Cos(2*Mathf.PI*f) + 1.0f) / 2.0f;
        	text.color = new Color(1, 1, 1, alpha);
        	if(b) {
        		timer.turnOff();
        		text.color = new Color(0, 0, 0, 0);
        	}

        }
    }

    public void OnTriggerEnter2D(Collider2D other) {
        if((showOnce && !showed) || !showOnce) {
       		if (other.gameObject.name == "Player") {
       			text.text = placeStr;
       			timer.turnOn();
                showed = true;
       		}
        }
   	}
}
