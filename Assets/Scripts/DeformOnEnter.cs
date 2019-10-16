using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Timer_namespace;


public class DeformOnEnter : MonoBehaviour
{
    public SpriteRenderer sp;
    private Timer swayTimer;
    public DestructableEnvironment destruc;
    private float direction;
    // Start is called before the first frame update
    void Start()
    {   
        swayTimer = new Timer(1.0f);
        swayTimer.turnOff();
    }

    // Update is called once per frame
    void Update()
    {
        if(swayTimer.isOn()) {
            bool b = swayTimer.updateTimer(Time.deltaTime);
            float f = Mathf.Sin(Mathf.PI*swayTimer.getCanoncial());
            // sp.material.SetFloat("_hitOffset", direction*2*f);
            if(b) {
                swayTimer.turnOff();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.gameObject.name == "Player" && !other.isTrigger && !destruc.growthTimer.isOn()) {
            direction = Mathf.Sign(other.gameObject.GetComponent<Rigidbody2D>().velocity.x);
            swayTimer.turnOn();
        }
    }
}
