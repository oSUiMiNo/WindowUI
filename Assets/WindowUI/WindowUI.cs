using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using TMPro;
using UniRx.Triggers;
using uWindowCapture;
using Cysharp.Threading.Tasks;
using System;
using UnityEngine.UI;

[RequireComponent(typeof(MyButton))]
public class WindowUI : MonoBehaviour
{
    [SerializeField] public GameObject altTabWindows; // �C���X�y�N�^�[����ݒ�
    [HideInInspector] public MyButton button;
    [HideInInspector] public Button windowIconButton;
    [HideInInspector] public GameObject window;
    [HideInInspector] public GameObject canvas;
    [HideInInspector] public GameObject background;
    [HideInInspector] public TMP_Dropdown dropdown;
    [HideInInspector] public Slider soundSlider;
    
    public string appName;

    public float height = 1;
    public float minimumWidth = 1f; // �Œ���̕�
    public float minimizedHeight = 0.05f; // �ŏ������ꂽ�E�B���h�E�̃I�u�W�F�N�g�̍���
    public UwcWindowTexture uwcWindowTexture;

    async void Start()
    {
        button = GetComponent<MyButton>();
        windowIconButton = transform.Find("Canvas/WindowIconButton").GetComponent<Button>();
        window = transform.Find("uWCWindowObject").gameObject;
        canvas = transform.Find("Canvas").gameObject;
        background = transform.Find("Background").gameObject;
        background.transform.SetParent(transform);
        background.transform.localPosition = new Vector3(0, 0, 0.02f);
        dropdown = canvas.transform.Find("Dropdown").gameObject.GetComponent<TMP_Dropdown>();
        uwcWindowTexture = window.GetComponent<UwcWindowTexture>();
        //soundSlider = canvas.transform.Find("SoundSlider").GetComponent<Slider>();

        //altTabWindows = transform.Find("AltTabWIndows").gameObject;

        uwcWindowTexture.partialWindowTitle = "*@ML";

        SetDropDownOptions();
        while (AltTab.windowTitles.Count == 0)
        {
            Debug.Log($"����Ƃ��� {AltTab.windowTitles.Count}");
            await UniTask.WaitForSeconds(0.5f);
            SetDropDownOptions();
        }
        canvas.SetActive(false);

        windowIconButton.OnClickAsObservable().Subscribe(_ =>
        {
            altTabWindows.SetActive(true);
        });

        uwcWindowTexture.On_ScaleChanged.Subscribe(async _ =>
        {
            //Debug.Log("HH�X�P�[���ύX");
            await SetWindow(uwcWindowTexture.partialWindowTitle);
        }).AddTo(gameObject);

        button.On_Enter.Subscribe(_ => 
        {
            //Debug.Log("������");
            canvas.SetActive(true);
            RectTransform canvasTrans = canvas.GetComponent<RectTransform>();
            Vector2 colSize = gameObject.GetComponent<BoxCollider2D>().size;
            colSize.x = canvasTrans.GetWidth();
            colSize.y = canvasTrans.GetHeight();
            gameObject.GetComponent<BoxCollider2D>().size = colSize;
        }).AddTo(gameObject);
        button.On_Exit.Subscribe(_ =>
        {
            //Debug.Log("�ł�");
            canvas.SetActive(false);
            Vector3 windowScale = window.transform.localScale;
            Vector2 colSize = gameObject.GetComponent<BoxCollider2D>().size;
            colSize.x = windowScale.x + 0.02f;
            colSize.y = windowScale.y + 0.02f;
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
                Debug.Log("�h���b�v�_�E���o�����[�ύX");
                await SetWindow(dropdown.options[value].text);
            }).AddTo(gameObject);

        //UwcManager.onWindowRemoved.AddListener(async (window) =>
        //{
        //    Debug.Log($"�E�B���h�E�폜0 {window.title}");
        //    if(this.window.GetComponent<Renderer>().material.mainTexture == null) 
        //});

        //uwcWindowTexture.onWindowChanged.AddListener(async (a, b) =>
        //{
        //    Debug.Log($"�E�B���h�E�ύX {uwcWindowTexture.partialWindowTitle}");
        //    await SetWindow(uwcWindowTexture.partialWindowTitle);
        //});
    }


    public void SetDropDownOptions()
    {
        Debug.Log("�N���b�N");
        AltTab.GetAltTabWindows();
        dropdown.ClearOptions();
        dropdown.AddOptions(new List<string>() {"�E�B���h�E���I��"});
        dropdown.AddOptions(AltTab.windowTitles);
    }


    public async UniTask SetWindow(string title)   
    {
        Debug.Log($"�E�B���h�E�e�N�X�`�� {uwcWindowTexture}");
        Debug.Log($"�^�C�g���O�@{uwcWindowTexture.partialWindowTitle}");
        Debug.Log($"�^�C�g���P�@{title}");
        //SetSoundSlider();

        uwcWindowTexture.partialWindowTitle = title;

        await UniTask.WaitForEndOfFrame(this);

        int windowHeight = AltTab.windows[uwcWindowTexture.partialWindowTitle].height;
        int windowWidth = AltTab.windows[uwcWindowTexture.partialWindowTitle].width;
        float longerSideLength = Mathf.Max(windowHeight, windowWidth);

        Debug.Log($"������{longerSideLength}");

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
            canvasTrans.SetWidth(minimumWidth);
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
        background.transform.localScale = new Vector3(windowScale.x, windowScale.y, 1);
    }


    //public void SetSoundSlider()
    //{
    //    appName = CropStr_R(uwcWindowTexture.partialWindowTitle, " - ", false);
    //    foreach (var audioVolume in altTabWindows.GetComponentsInChildren<AudioProcessVolume>())
    //    {
    //        Debug.Log($"�ڂ��[�ނȂ�0 {audioVolume.Name}");

    //        if (appName.Contains(audioVolume.Name, StringComparison.OrdinalIgnoreCase))
    //        {
    //            Debug.Log($"�ڂ��[�ނȂ�1 {audioVolume.Name}");

    //            soundSlider.OnValueChangedAsObservable().Subscribe(volume =>
    //            {
    //                audioVolume.Volume.Value = volume;
    //            });

    //            audioVolume.Volume.Subscribe(volume =>
    //            {
    //                soundSlider.value = volume;
    //            });
    //        }
    //    }
    //}


    //// �E���؂蔲��
    //public string CropStr_R(string str, string splitter, bool containSplitter)
    //{
    //    int i = str.LastIndexOf(splitter);
    //    if (i < 0) return str;

    //    int a;
    //    if (containSplitter) a = 0;
    //    else a = splitter.Length;

    //    return str.Substring(i + a);
    //}
}
