using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Timer_namespace;

public class ItemEmitter : MonoBehaviour
{
	  public List<LagGoodie> lagGoodies;
	  public GameObject amber;
    // Start is called before the first frame update
    void Start()
    {
        lagGoodies = new List<LagGoodie>();
    }

    // Update is called once per frame
    void Update()
    {
        for(int i = 0; i < lagGoodies.Count; ) {
            bool rem = lagGoodies[i].update(this);
            if(rem) {
                lagGoodies.RemoveAt(i);
            } else {
                i++;
            }
        }

    }
    
    public class LagGoodie {
        Timer timer;
        AmberType type;
        float angle;
        ActivateQuote quote;
        Vector3 pos;
        
        public LagGoodie(AmberType type, float lagTime, float angle, ActivateQuote quote, Vector3 pos) {
            this.type = type;
            timer = new Timer(lagTime);
            timer.turnOn();
            this.angle = angle;
            this.quote = quote;
            this.pos = pos;
        }
        
        public bool update(ItemEmitter emitter) {
            bool result = false;
            if(timer.isOn()) {
                bool finished = timer.updateTimer(Time.deltaTime);
                if(finished) {
                    emitter.CreateGoodie(type, angle, quote, pos);
                    result = true;
                    timer.turnOff();        
                }
            }
            return result;
        }
    }

    public void CreateGoodie(AmberType type, float angle, ActivateQuote quote, Vector3 pos) {
        
        GameObject objAmber = Instantiate(amber, pos,  Quaternion.identity);
        Rigidbody2D amberRb = objAmber.GetComponent<Rigidbody2D>();
        CollectAmber amberCollect = objAmber.transform.GetChild(0).gameObject.GetComponent<CollectAmber>();
        amberCollect.type = (AmberType)type;
   
        
        if(type == AmberType.AMBER_HEALTH || type == AmberType.AMBER_MANA) {
            float randScale = Random.Range(0.7f, 1.3f);
            objAmber.transform.localScale = new Vector3(randScale, randScale, 1);
            objAmber.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().sortingOrder = 3;
        } else if (type == AmberType.AMBER_SENTINEL_HEAD) {
            objAmber.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().sortingOrder = 4;
            if(quote != null) {
                amberCollect.SetQuoteOnCollect(quote);
            }
        } else {
            objAmber.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().sortingOrder = 0;
        }
        float randomAngle = angle;
        if(angle == 0.0f) {
            randomAngle = Mathf.Lerp(0.25f*Mathf.PI, 0.75f*Mathf.PI, Random.Range(0.0f, 1.0f));
        }
        Vector2 newForce = new Vector3(Mathf.Cos(randomAngle), Mathf.Sin(randomAngle));
        amberRb.bodyType = RigidbodyType2D.Dynamic;
        float lerpVal = Mathf.Lerp(80000, 90000, Random.Range(0.0f, 1.0f));
        amberRb.AddForce(lerpVal*newForce);
    }
    
    public void emitAmber(AmberType type, int number, Vector3 pos, ActivateQuote quote = null, float angle = 0.0f, float lagTime = 0.0f) {
        for(int i = 0; i < number; ++i) {
            if(lagTime > 0.0f) {
                lagGoodies.Add(new LagGoodie(type, lagTime, angle, quote, pos));
            } else {
                CreateGoodie(type, 0, quote, pos);
            }
            
        }
    }
}
