using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Timer_namespace;

public class RaockPhysics : MonoBehaviour
{
	private Timer fadeTimer;
	public float direction;
	public Rigidbody2D body;
	public SpriteRenderer sp;
    // Start is called before the first frame update
    void Start()
    {
        fadeTimer = new Timer(3.0f);
        fadeTimer.turnOn();
        float dir = direction*2*Mathf.PI;
        Vector2 v = new Vector2(Mathf.Cos(dir), Mathf.Sin(dir));
        body.AddForce(v);
    }

    // Update is called once per frame
    void Update()
    {
        bool fin = fadeTimer.updateTimer(Time.deltaTime);
        float t01 = 1.0f - fadeTimer.getCanoncial();
        Color c = sp.color;
        c.a = t01;
        sp.color = c;
        if(fin) {
        	fadeTimer.turnOff();
        	Destroy(gameObject);
        }
    }
}
