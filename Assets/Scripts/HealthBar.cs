using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour
{

	private Color startColor; 
	private float startScale; 

    public GameObject healthInnerBar;
    private SpriteRenderer healthBarSpriteRenderer;
	
    // Start is called before the first frame update
    void Start()
    {
        healthBarSpriteRenderer = healthInnerBar.GetComponent<SpriteRenderer>();

        startColor = healthBarSpriteRenderer.color;
        startScale = healthInnerBar.transform.localScale.x;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateHealthBar(int health, int startHealth) {
    	float healthAsPercent = ((float)health / (float)startHealth);
    	healthAsPercent = Mathf.Max(0, healthAsPercent);
    	Vector3 tempScale = healthInnerBar.transform.localScale;
    	tempScale.x = startScale * healthAsPercent;
    	healthInnerBar.transform.localScale = tempScale;
    	Color originalColor = healthBarSpriteRenderer.color;
    	if(healthAsPercent < 0.6f && healthAsPercent > 0.3f) {
    	    originalColor = new Vector4(1, 1, 0, 1);
    	} else if(healthAsPercent < 0.3f) {
    	    originalColor = Color.red;
    	}
    	
    	healthBarSpriteRenderer.color = originalColor;
    }

    public void ResetHealthBar() {
    	Vector3 tempScale = healthInnerBar.transform.localScale;
    	tempScale.x = startScale;
    	healthInnerBar.transform.localScale = tempScale;
    	healthBarSpriteRenderer.color = startColor;
    }

    public void Hide() {
    	gameObject.SetActive(false);
    }

    public void Show() {
    	gameObject.SetActive(true);
    }

    public void SetName(string name) {

    }

     public void SetLevel() {
    	
    }
}
