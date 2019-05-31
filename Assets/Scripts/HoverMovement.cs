using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Timer_namespace;

public class HoverMovement : MonoBehaviour
{
	private float tAt;
	private Timer fadeTimer;
	public SpriteRenderer spRender;
	public bool isIn;
	private float startY;
    // Start is called before the first frame update
    void Start()
    {
        fadeTimer = new Timer(0.5f);
        fadeTimer.turnOff();
        isIn = true;
        startY = transform.position.y;

    }

    public void fadeIn() {
    	
  //   	if(fadeTimer.isOn()) {
  //   		if(!isIn) {
  //   			fadeTimer.tAt = fadeTimer.period - fadeTimer.tAt;
  //   		}
		// } else {
			fadeTimer.turnOn();			
		// }
		isIn = true;
    }

    public void fadeOut() {
  //   	if(fadeTimer.isOn()) {
  //   		if(isIn) {
  //   			fadeTimer.tAt = fadeTimer.period - fadeTimer.tAt;
  //   		}
		// } else {
			fadeTimer.turnOn();			
		// }
		isIn = false;
    }

    // Update is called once per frame
    void Update()
    {
        
        tAt += Time.deltaTime;
        transform.position = new Vector3(transform.position.x, startY + Mathf.Sin(tAt), transform.position.z);

        if(fadeTimer.isOn()) {
            bool finished = fadeTimer.updateTimer(Time.deltaTime);
            float alphaVal = 1;
            if(isIn) {
            	alphaVal = Mathf.Lerp(0, 1, fadeTimer.getCanoncial());	
            } else {
            	alphaVal = Mathf.Lerp(1, 0, fadeTimer.getCanoncial());	
            }
            
            spRender.color = new Vector4(spRender.color.r, spRender.color.g, spRender.color.b, alphaVal); 
            if(finished) {
                fadeTimer.turnOff();
            }

        }
    }
}
