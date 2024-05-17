using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class BlindEffect : MonoBehaviour
{
    [SerializeField] private Image img;
    public static BlindEffect activeInstance; 

    private int width, height;

    private Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        activeInstance = this;
        width = Screen.width;
        height = Screen.height;
    }

    public void GoBlind()
    {
        StartCoroutine(GoBlindCo());
    }

    private IEnumerator GoBlindCo()
    {
        yield return new WaitForEndOfFrame();
        Texture2D texture = new Texture2D(width, height, TextureFormat.RGB24, false);
        texture.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        texture.Apply();

        img.sprite = Sprite.Create(texture, new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f), 100);

        anim.SetTrigger("GoBlind");
    }
}
