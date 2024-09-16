using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using uWindowCapture;
using UniRx;
using TMPro;
using Cysharp.Threading.Tasks;

public class WindowLayouter : MonoBehaviourMyExtention
{
    UwcWindowTextureManager manager_;
    [SerializeField] float marginX = 0.1f;
    [SerializeField] float marginY = 0.1f;
    [SerializeField] float windowHeight = 0.4f;
    [SerializeField] float windowMinimumWidth = 0.3f;
    [SerializeField] float windowMinimalizedHeight = 0.05f;
    [SerializeField] float width = 3;

    [SerializeField] WindowUI window;
    [SerializeField] GameObject windowMaterial;
    List<WindowUI> windows = new List<WindowUI>();
    GameObject backGround;

    async void Awake()
    {
        window = GameObject.Find("Window").GetComponent<WindowUI>();

        manager_ = window.GetComponent<UwcWindowTextureManager>();
        //backGround = GameObject.CreatePrimitive(PrimitiveType.Quad);

        await Delay.Second(1);

        await UpdateWindowList();

        //window.uwcWindowTexture.onWindowChanged.AddListener(async (o, d) =>
        //{
        //    await UpdateWindowList();
        //});

        await Delay.Second(1);

        UwcWindowTexture.On_IncOrDec.Subscribe(async _ =>
        {
            Debug.Log("HHリスト増減");
            await Delay.Second(1);
            window.SetDropDownOptions();
            await UpdateWindowList();
            await UpdateWindows();
        }).AddTo(gameObject);

        UwcWindowTextureChildrenManager.On_ChildAdded.Subscribe(async _ =>
        {
            Debug.Log("HHチャイルド追加");
            await Delay.Second(3);
            window.SetDropDownOptions();
            await UpdateWindowList();
            await UpdateWindows();
        }).AddTo(gameObject);

        await UpdateWindows();


        foreach (var windowUI in windows)   
        {
            windowUI.uwcWindowTexture.On_ScaleChanged.Subscribe(async _ =>
            {
                //Debug.Log("HHスケール変更");
                await UniTask.WaitForEndOfFrame(windowUI);
                await UpdateWindows();
            }).AddTo(windowUI.gameObject);
        }
    }


    async UniTask UpdateWindowList()
    {
        foreach (var window in windows)
        {
            Debug.Log($"HH ですとろい {window.gameObject}");
            Destroy(window.gameObject);
        }
        windows.Clear();
        await Delay.Second(0.2f);
        foreach (var title in AltTab.windowTitles)
        {
            Debug.Log($"HHおるとタブ　{title}");
            //if (a.Contains("WindowUI - Test_WindowUI - Windows, Mac, Linux - Unity 2022.3.5f1 <DX11>")) continue;
            var windowUI = Instantiate(windowMaterial.gameObject).GetComponent<WindowUI>();
            windowUI.transform.SetParent(gameObject.transform);
            windowUI.height = windowHeight;
            windowUI.minimumWidth = windowMinimumWidth;
            windowUI.minimizedHeight = windowMinimalizedHeight;

            await Delay.Frame(2);
            await windowUI.SetWindow(title);

            windows.Add(windowUI);
        }
    }


    async UniTask UpdateWindows()
    {
        float posX = 0;
        //float totalWidth = 0;
        float posY = 0;
        await Delay.Second(0.1f);

        foreach (var windowUI in windows)
        {
            Debug.Log($"HHあぷで　{windowUI}");

            await UniTask.WaitForEndOfFrame(windowUI);
            await Delay.Second(0.1f);

            //var windowWidth = windowUI.transform.localScale.x;
            //var windowWidth = windowUI.uwcWindowTexture.transform.localScale.x;
            var windowWidth = windowUI.background.transform.localScale.x;

            //Debug.Log($"はば {windowWidth}");

            if (posX + windowWidth * 0.5f + marginX > width)
            {
                posY -= windowHeight + marginY;
                posX = 0;
            }
            posX += windowWidth * 0.5f;
            //Debug.Log($"いち {posX}");
            
            windowUI.transform.localPosition = new Vector3(posX, posY, -0.1f);

            if (posX + windowWidth * 0.5f + marginX > width)
            {
                posY -= windowHeight + marginY;
                posX = 0;
            }
            posX += windowWidth * 0.5f + marginX;


            windowUI.button.On_Click.Subscribe(async _ =>
            {
                TMP_Dropdown dropdown = window.dropdown;
                for (int a = 0; a < dropdown.options.Count; a++)
                {
                    if (dropdown.options[a].text == windowUI.uwcWindowTexture.partialWindowTitle)
                    {
                        dropdown.value = a;
                    }
                }

                gameObject.SetActive(false);
            });
        }
    }
}
