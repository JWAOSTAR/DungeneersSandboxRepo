using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackgroundScroller : MonoBehaviour
{
    public Texture2D[] backgrounds;
    public Image displayImage;
    private Material mat;
    private int index;

    // Start is called before the first frame update
    void Start()
    {
        index = 0;
        mat = gameObject.GetComponent<MeshRenderer>().material;
        mat.SetTexture("_MainTex", backgrounds[index]);
        displayImage.sprite = Sprite.Create(backgrounds[index], new Rect(0, 0, backgrounds[index].width, backgrounds[index].height), new Vector2(0, 0));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void onButtonNextClick()
    {
        if((index + 1) != backgrounds.GetLength(0))
        {
            index++;
            mat.SetTexture("_MainTex", backgrounds[index]);
            displayImage.sprite = Sprite.Create(backgrounds[index], new Rect(0, 0, backgrounds[index].width, backgrounds[index].height), new Vector2(0, 0));
        }
        else
        {
            index = 0;
            mat.SetTexture("_MainTex", backgrounds[index]);
            displayImage.sprite = Sprite.Create(backgrounds[index], new Rect(0, 0, backgrounds[index].width, backgrounds[index].height), new Vector2(0, 0));
        }
    }

    public void onButtonPrevClick()
    {
        if ((index - 1) != -1)
        {
            index--;
            mat.SetTexture("_MainTex", backgrounds[index]);
            displayImage.sprite = Sprite.Create(backgrounds[index], new Rect(0, 0, backgrounds[index].width, backgrounds[index].height), new Vector2(0, 0));
        }
        else
        {
            index = backgrounds.GetLength(0) - 1;
            mat.SetTexture("_MainTex", backgrounds[index]);
            displayImage.sprite = Sprite.Create(backgrounds[index], new Rect(0, 0, backgrounds[index].width, backgrounds[index].height), new Vector2(0, 0));
        }
    }
}
