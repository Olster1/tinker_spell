using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Timer_namespace;

public class cameraMenu : MonoBehaviour
{
	public Rigidbody2D rb;
	public float force;
    private Timer timer;
    private bool out1;
    public Image panel;
    // Start is called before the first frame update
    void Start()
    {
        timer = new Timer(0.6f);
        timer.turnOff();
    }

    // Update is called once per frame
    void Update()
    {
        if(timer.isOn()) {
            bool b = timer.updateTimer(Time.deltaTime);
            float f = timer.getCanoncial();
            if(out1) {
                //fade out
                f = 1.0f - f;
            }
            panel.color = new Color(panel.color.r, panel.color.b, panel.color.g, f);
            if(b) {
                if(out1) {
                    timer.turnOff();
                    out1 = false;        
                } else {
                    transform.position = new Vector3(0.0f, transform.position.y, transform.position.z);
                    timer.turnOn();
                    out1 = true;        
                }
                
            }
        }
    }

    void FixedUpdate() {
    	rb.velocity = new Vector2(force, 0);
    	if(transform.position.x > 8.0f && !timer.isOn()) {
    		timer.turnOn();
            out1 = false;
    	}
    	// rb.AddForce(force*Vector2.right);
    }
}
