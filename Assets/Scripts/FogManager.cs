using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Timer_namespace;

public class FogManager : MonoBehaviour, IEvent
{
	private Timer fadeTimer; 
	private SpriteRenderer[] fogs;
	private Vector3[] startPs;
	private Vector3[] endOffsets; 
    // Start is called before the first frame update
    void Start()
    {
        fadeTimer = new Timer(1.0f);
        fadeTimer.turnOff();
        fogs = new SpriteRenderer[transform.childCount];
        startPs = new Vector3[fogs.Length];
        endOffsets = new Vector3[fogs.Length];
        for(int i = 0; i < fogs.Length; ++i) {
        	fogs[i] = transform.GetChild(i).GetComponent<SpriteRenderer>();
        	startPs[i] = fogs[i].transform.localPosition;
        	endOffsets[i] = new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f), 0);
        }
    }

    public void Run() {
    	fadeTimer.turnOn();
    } 

    // Update is called once per frame
    void Update()
    {
        if(fadeTimer.isOn()) {
        	bool finished = fadeTimer.updateTimer(Time.deltaTime);
        	float f = fadeTimer.getCanoncial();
        	for(int i = 0; i < fogs.Length; ++i) {
        		fogs[i].color = new Color(fogs[i].color.r, fogs[i].color.g, fogs[i].color.b, 1.0f - f);
        		fogs[i].transform.localPosition = Vector3.Lerp(startPs[i], startPs[i] + endOffsets[i], f);
        	}
        	if(finished) {
        		fadeTimer.turnOff();
        	}

        }
    }
}
