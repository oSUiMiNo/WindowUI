using Cysharp.Threading.Tasks;
using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using uWindowCapture;


[RequireComponent(typeof(MyButton))]
public class AltTabWindow : MonoBehaviour
{
    public MyButton button;
    public Transform background;
    public GameObject canvas;
    public MySlider soundSlider;
    public BoxCollider2D colli;
    public UwcIconTexture iconTexture;
    public UwcWindowTexture uwcWindowTexture;

    public string appName;

    public float height = 1;
    public float backGroundHeight = 0.56f;
    public float minimumWidth = 0.6f; // 最低限の幅
    public float minimizedHeight = 0.05f; // 最小化されたウィンドウのオブジェクトの高さ


    public void Init()
    {
        button = GetComponent<MyButton>();
        colli = GetComponent<BoxCollider2D>();
        background = transform.Find("Background");
        canvas = transform.Find("Canvas").gameObject;
        soundSlider = transform.Find("SoundSlider").GetComponent<MySlider>();
        iconTexture = GetComponentInChildren<UwcIconTexture>();
        Debug.Log($"あいこん{iconTexture}");
        canvas.SetActive(false);
        background.gameObject.SetActive(false);
    }

    public void Visibe(bool visible)
    {
        canvas.SetActive(visible);
        background.gameObject.SetActive(visible);
    }


    public async UniTask SetWindow(string title)
    {
        iconTexture.windowTexture = uwcWindowTexture;

        SetSoundSlider();

        await UniTask.WaitForEndOfFrame(this);

        if (uwcWindowTexture.window.isMinimized)
            uwcWindowTexture.scalePer1000Pixel *= minimizedHeight / uwcWindowTexture.transform.localScale.y;
        else
            uwcWindowTexture.scalePer1000Pixel *= height / uwcWindowTexture.transform.localScale.y;

        await UniTask.WaitForEndOfFrame(this);

        RectTransform canvasTrans = canvas.GetComponent<RectTransform>();
        
        Vector3 windowScale = uwcWindowTexture.transform.localScale;
        if (windowScale.x < minimumWidth)
            canvasTrans.SetWidth(minimumWidth);
        else
            canvasTrans.SetWidth(windowScale.x + 0.02f);
        if (windowScale.y < height)
            canvasTrans.SetHeight(height + 0.02f);
        else
            canvasTrans.SetHeight(windowScale.y + 0.02f);

        Vector2 colSize = colli.size;
        colSize.x = canvasTrans.GetWidth() + 0.05f;
        colSize.y = canvasTrans.GetHeight() + 0.05f;
        //if (windowScale.x < 0.3f) colSize.x = windowScale.x;
        //if (windowScale.y < 0.3f) colSize.y = windowScale.y;
        colli.size = colSize;
        background.transform.localScale = new Vector3(canvasTrans.GetWidth(), backGroundHeight, 1);
    }


    public void SetSoundSlider()
    {
        appName = CropStr_R(uwcWindowTexture.window.title, " - ", false);
        foreach (var audioVolume in transform.parent.GetComponentsInChildren<AudioProcessVolume>())
        {
            Debug.Log($"ぼりゅーむつなぐ0 {audioVolume.Name}");

            if (appName.Contains(audioVolume.Name, StringComparison.OrdinalIgnoreCase))
            {
                Debug.Log($"ぼりゅーむつなぐ1 {audioVolume.Name}");

                //soundSlider.OnValueChangedAsObservable().Subscribe(volume =>
                //{
                //    audioVolume.Volume.Value = volume;
                //});

                //audioVolume.Volume.Subscribe(volume =>
                //{
                //    soundSlider.value = volume;
                //});

                soundSlider.Value_Float.Subscribe(volume =>
                {
                    audioVolume.Volume.Value = volume / 10;
                }).AddTo(soundSlider.gameObject);

                audioVolume.Volume.Subscribe(volume =>
                {
                    soundSlider.Value_Float.Value = volume * 10;
                    Debug.Log($"ぼりゅーむ {volume * 10} {soundSlider.Value_Float.Value} {uwcWindowTexture.window.title}");
                }).AddTo(soundSlider.gameObject);
            }
        }
    }


    // 右側切り抜き
    public string CropStr_R(string str, string splitter, bool containSplitter)
    {
        int i = str.LastIndexOf(splitter);
        if (i < 0) return str;

        int a;
        if (containSplitter) a = 0;
        else a = splitter.Length;

        return str.Substring(i + a);
    }
}
