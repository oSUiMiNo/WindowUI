using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using TMPro;
using UniRx.Triggers;
using uWindowCapture;
using Cysharp.Threading.Tasks;
using System;
using Unity.VisualScripting.YamlDotNet.Core.Tokens;

[RequireComponent(typeof(MyButton))]
public class WindowUI : MonoBehaviour
{
    public MyButton button;
    GameObject parent => transform.parent.gameObject;
    public GameObject window;
    public GameObject canvas;
    public GameObject background;
    public TMP_Dropdown dropdown;
    public float height = 1;
    public float minimumWidth = 0.6f; // 最低限の幅
    public float minimizedHeight = 0.05f; // 最小化されたウィンドウのオブジェクトの高さ
    public UwcWindowTexture uwcWindowTexture;

    async void Start()
    {
        button = GetComponent<MyButton>();
        window = transform.Find("uWCWindowObject").gameObject;
        canvas = transform.Find("Canvas").gameObject;
        background = transform.Find("Background").gameObject;
        background.transform.SetParent(transform);
        background.transform.localPosition = new Vector3(0, 0, 0.02f);
        dropdown = canvas.transform.Find("Dropdown").gameObject.GetComponent<TMP_Dropdown>();
        uwcWindowTexture = window.GetComponent<UwcWindowTexture>();

        uwcWindowTexture.partialWindowTitle = "*@ML";

        SetDropDownOptions();
        while (AltTab.windowTitles.Count == 0)
        {
            Debug.Log($"おるとたぶ {AltTab.windowTitles.Count}");
            await UniTask.WaitForSeconds(0.5f);
            SetDropDownOptions();
        }
        canvas.SetActive(false);

        uwcWindowTexture.On_ScaleChanged.Subscribe(async _ =>
        {
            //Debug.Log("HHスケール変更");
            await SetWindow(uwcWindowTexture.partialWindowTitle);
        }).AddTo(gameObject);

        button.On_Enter.Subscribe(_ => 
        {
            //Debug.Log("入った");
            canvas.SetActive(true);
            RectTransform canvasTrans = canvas.GetComponent<RectTransform>();
            Vector2 colSize = gameObject.GetComponent<BoxCollider2D>().size;
            colSize.x = canvasTrans.GetWidth();
            colSize.y = canvasTrans.GetHeight();
            gameObject.GetComponent<BoxCollider2D>().size = colSize;
        }).AddTo(gameObject);
        button.On_Exit.Subscribe(_ =>
        {
            //Debug.Log("でた");
            canvas.SetActive(false);
            Vector3 windowScale = window.transform.localScale;
            Vector2 colSize = gameObject.GetComponent<BoxCollider2D>().size;
            //colSize.x = windowScale.x - 0.3f;
            //colSize.y = windowScale.y - 0.3f;
            if (windowScale.x < 0.3f) colSize.x = windowScale.x;
            if (windowScale.y < 0.3f) colSize.y = windowScale.y;
            gameObject.GetComponent<BoxCollider2D>().size = colSize;
        }).AddTo(gameObject);

        dropdown.OnPointerClickAsObservable().Subscribe(_ => 
        {
            SetDropDownOptions();
        }).AddTo(gameObject);

        dropdown.onValueChanged.AsObservable()
            .TakeUntilDestroy(this)
            .ThrottleFirst(TimeSpan.FromSeconds(0.5))
            .Subscribe(async value =>
            {
                await SetWindow(dropdown.options[value].text);
            }).AddTo(gameObject);

        //uwcWindowTexture.onWindowChanged.AddListener(async (a, b) =>
        //{
        //    Debug.Log($"ウィンドウ変更 {uwcWindowTexture.partialWindowTitle}");
        //    await SetWindow(uwcWindowTexture.partialWindowTitle);
        //});
    }


    public void SetDropDownOptions()
    {
        Debug.Log("クリック");
        AltTab.GetAltTabWindows();
        dropdown.ClearOptions();
        dropdown.AddOptions(new List<string>() {"ウィンドウ未選択"});
        dropdown.AddOptions(AltTab.windowTitles);
    }


    public async UniTask SetWindow(string title)   
    {
        Debug.Log($"ウィンドウテクスチャ {uwcWindowTexture}");
        Debug.Log($"タイトル０　{uwcWindowTexture.partialWindowTitle}");
        Debug.Log($"タイトル１　{title}");


        uwcWindowTexture.partialWindowTitle = title;

        await UniTask.WaitForEndOfFrame(this);

        int windowHeight = AltTab.windows[uwcWindowTexture.partialWindowTitle].height;
        int windowWidth = AltTab.windows[uwcWindowTexture.partialWindowTitle].width;
        float longerSideLength = Mathf.Max(windowHeight, windowWidth);

        Debug.Log($"長い方{longerSideLength}");

        await UniTask.WaitForEndOfFrame(this);
        //await UniTask.WaitForEndOfFrame(this);
        await Delay.Second(0.05f);

        //uwcWindowTexture.scalePer1000Pixel = 1000 / longerSideLength;
        //if (uwcWindowTexture.gameObject.GetComponent<Renderer>().material.mainTexture == null)
        if (uwcWindowTexture.window.isMinimized)
        {
            Debug.Log($"{uwcWindowTexture}");
            Debug.Log($"{window}");
            //if (uwcWindowTexture == null) return; 
            uwcWindowTexture.scalePer1000Pixel *= minimizedHeight / window.transform.localScale.y;
        }
        else
        {
            Debug.Log($"{uwcWindowTexture}");
            Debug.Log($"{window}");
            //if (uwcWindowTexture != null) return;
            uwcWindowTexture.scalePer1000Pixel *= height / window.transform.localScale.y;
        }

        await UniTask.WaitForEndOfFrame(this);

        Vector3 windowScale = window.transform.localScale;

        RectTransform canvasTrans = canvas.GetComponent<RectTransform>();

        if (windowScale.x < minimumWidth)
            canvasTrans.SetWidth(0.6f);
        else 
            canvasTrans.SetWidth(windowScale.x + 0.02f);
        if (windowScale.y < height) 
            canvasTrans.SetHeight(height + 0.02f);
        else
            canvasTrans.SetHeight(windowScale.y + 0.02f);


        Vector2 colSize = gameObject.GetComponent<BoxCollider2D>().size;
        colSize.x = canvasTrans.GetWidth() + 0.05f;
        colSize.y = canvasTrans.GetHeight() + 0.05f;
        //if (windowScale.x < 0.3f) colSize.x = windowScale.x;
        //if (windowScale.y < 0.3f) colSize.y = windowScale.y;
        gameObject.GetComponent<BoxCollider2D>().size = colSize;
        background.transform.localScale = new Vector3(canvasTrans.GetWidth(), canvasTrans.GetHeight(), 1);
    }

    //public async UniTask SetWindow(int value)
    //{
    //    uwcWindowTexture.partialWindowTitle = dropdown.options[value].text;

    //    await UniTask.WaitForEndOfFrame(this);
    //    await UniTask.WaitForSeconds(0.03f);

    //    int windowHeight = AltTab.windows[uwcWindowTexture.partialWindowTitle].height;
    //    int windowWidth = AltTab.windows[uwcWindowTexture.partialWindowTitle].width;
    //    float longerSideLength = Mathf.Max(windowHeight, windowWidth);

    //    uwcWindowTexture.scalePer1000Pixel = 1000 / longerSideLength;

    //    await UniTask.WaitForEndOfFrame(this);
    //    await UniTask.WaitForSeconds(0.03f);
    //    Vector3 windowScale = window.transform.localScale;

    //    RectTransform canvasTrans = canvas.GetComponent<RectTransform>();
    //    canvasTrans.SetWidth(windowScale.x);
    //    canvasTrans.SetHeight(windowScale.y);

    //    if(windowScale.x < 0.6f) canvasTrans.SetWidth(0.6f);
    //    if(windowScale.y < 0.6f) canvasTrans.SetHeight(0.6f);

    //    Vector2 colSize = gameObject.GetComponent<BoxCollider2D>().size;
    //    //colSize.x = windowScale.x - 0.3f;
    //    //colSize.y = windowScale.y - 0.3f;
    //    if (windowScale.x < 0.3f) colSize.x = windowScale.x;
    //    if (windowScale.y < 0.3f) colSize.y = windowScale.y;
    //        gameObject.GetComponent<BoxCollider2D>().size = colSize;
    //}
}
