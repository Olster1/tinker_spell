using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Timer_namespace;

public class BlurPostProcess : MonoBehaviour
{
	 // [Range(1, 10)] 
	private int iterations;
	public Material mat;
	private Vector2 res;

    private int spriteImageIndex;
	public SpriteRenderer[] spRenderer = new SpriteRenderer[2];

	private Texture2D textureSaved;
	private bool renderToImage;

	public IBlurInterface blurInterface;
    // Start is called before the first frame update
    void Start()
    {
    	iterations = 8;
    }

    public void StartBlur(IBlurInterface i, int blurIndex) {
    	blurInterface = i;
    	renderToImage = true;
        spriteImageIndex = blurIndex;
    }
    // Update is called once per frame
    void Update()
    {
        // if(timer.isOn()) {
        // 	bool f = timer.updateTimer(Time.deltaTime);
        // 	float c = timer.getCanoncial();

        // 	if(c < 0.2f) {
        // 		iterations = 1;
        // 	} else if(c < 0.4f) {
        // 		iterations = 2;
        // 	} else if(c < 0.6f) {
        // 		iterations = 3;
        // 	} else if(c < 0.8f) {
        // 		iterations = 4;
        // 	} else if(c < 1.0f) {
        // 		iterations = 5;
        // 	} 

        // 	if(f) {
        // 		renderToImage = true;
        // 		timer.turnOff();
        // 	}
        // }
    }


    void SetUniforms(bool isHorizontal) {
    	mat.SetInt("_Horizontal", (isHorizontal ? 1 : 0));
    }

	public void SaveToTexture(RenderTexture src) {
    	textureSaved = new Texture2D(src.width, src.height, TextureFormat.RGBA32, false);
        //Read the pixels in the Rect starting at 0,0 and ending at the screen's width and height
        textureSaved.ReadPixels(new Rect(0, 0, src.width, src.height), 0, 0, false);
        textureSaved.Apply();

        Vector3 pointMin = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 10));

        Vector3 pointMax = Camera.main.ScreenToWorldPoint(new Vector3(src.width, src.height, 10));

        //get it the size of the screen
        float ratio = src.width / (pointMax.x - pointMin.x);

        spRenderer[spriteImageIndex].sprite = Sprite.Create(textureSaved, new Rect(0, 0, src.width, src.height), new Vector2(0.5f, 0.5f), ratio, 0);
	}

    void OnRenderImage(RenderTexture src, RenderTexture dest) {
    	if(!(renderToImage)) {
    		Graphics.Blit(src, dest);
    	} else {

	    	RenderTexture rt2 = RenderTexture.GetTemporary(src.width, src.height, 0, src.format);
	    	res = new Vector2(src.width, src.height);

	    	mat.SetFloat("_TexDimensionX", res.x);
	    	mat.SetFloat("_TexDimensionY", res.y);
	    	
	    	for(int i = 0; i < iterations; ++i) {
	    		if(i == (iterations - 1)){
					SetUniforms(true);
					Graphics.Blit(src, rt2, mat);
					src.DiscardContents();
	    			
	    			SetUniforms(false);
	    			Graphics.Blit(rt2, dest, mat);
	    		} else {
	    			SetUniforms(true);
					Graphics.Blit(src, rt2, mat);
					src.DiscardContents();
	    			
					SetUniforms(false);
	    			Graphics.Blit(rt2, src, mat);
	    			rt2.DiscardContents();
	    		}
	    	}

	    	if(renderToImage) {
	    		SaveToTexture(src);
	    		blurInterface.Activate(true);

	    	}

	    	renderToImage = false;
	    	RenderTexture.ReleaseTemporary(rt2);

		}
	}
}
