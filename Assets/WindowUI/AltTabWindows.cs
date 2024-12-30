using Cysharp.Threading.Tasks;
using UnityEngine;
using uWindowCapture;
using UniRx;
using TMPro;
using System.Collections.Generic;
using System.Linq;
using System;


//[RequireComponent(typeof(UwcWindowTextureManager))]
public class AltTabWindows : MonoBehaviourMyExtention
{
    AltTabWindowTextureManager manager_;

    [SerializeField] float marginX = 0.06f;
    [SerializeField] float marginY = 0.32f;
    [SerializeField] float backGroundHeight = 0.6f;
    [SerializeField] float windowHeight = 0.4f;
    [SerializeField] float windowMinimumWidth = 0.6f;
    [SerializeField] float windowMinimalizedHeight = 0.05f;
    [SerializeField] float width = 3;
    [SerializeField] WindowUI windowUI;
    public GameObject altTabWindow;
    // 無視したいウィンドウの名前
    public List<string> IgnoreWindows = new List<string>()
    {
        "WindowUI - Test_WindowUI - Windows, Mac, Linux - Unity 6 Preview (6000.0.10f1)* <DX11>",
        "WindowUI - Test_WindowUI - Windows, Mac, Linux - Unity 6 Preview (6000.0.10f1) <DX11>"
    };


    List<UwcWindowTexture> windowTextures; 


    async void Awake()
    {
        manager_ = GetComponent<AltTabWindowTextureManager>();
        windowUI = GameObject.Find("Window").GetComponent<WindowUI>();

        await Delay.Second(1);

        UwcManager.onWindowAdded.AddListener(async (window) =>
        {
            if (string.IsNullOrEmpty(window.title)) return;
            Debug.Log($"ウィンドウ追加 {window.title}");
            await UpdateLayout();
        });

        UwcManager.onWindowRemoved.AddListener(async (window) =>
        {
            Debug.Log($"ウィンドウ削除0 {window.title}");
            //if (string.IsNullOrEmpty(window.title)) return;
            Debug.Log($"ウィンドウ削除1 {window.title}");
            await UpdateLayout();
        });


        await UpdateLayout();

        foreach (var windowTexture in windowTextures)
        {
            Debug.Log($"== {windowTexture.window.title} ==========================");
            Debug.Log($"ハンドル {windowTexture.window.handle}");
            Debug.Log($"プロセスID {windowTexture.window.processId}");
            Debug.Log($"ウィンドウID {windowTexture.window.id}");
        }
    }


    private async UniTask UpdateLayout()
    {
        float posX = 0;
        float posY = 0;

        windowTextures = new List<UwcWindowTexture>();

        foreach (var windowTexture in manager_.windows.Values)
        {
            windowTexture.transform.SetParent(transform);

            // 無視するウィンドウの処理。アプリ自体のウィンドウなど
            bool ignore = false;
            foreach (var ignoreWindow in IgnoreWindows)
                if (windowTexture.window.title.Contains(ignoreWindow))
                {
                    windowTexture.gameObject.SetActive(false);
                    ignore = true;
                }
            if(ignore) continue;

            windowTextures.Add(windowTexture);
        }

        foreach (var a in GetComponentsInChildren<AltTabWindow>())
        {
            if (a.transform.Find("uWC Window Object(Clone)") == null)
            {
                a.uwcWindowTexture = null;
                Destroy(a.gameObject);
            }
        }


        foreach (var windowTexture in windowTextures)
        {
            //Debug.Log($"うぃんどうてくすちゃ {windowTexture.window.title}");
            AltTabWindow altTabWindow = Instantiate(this.altTabWindow).GetComponent<AltTabWindow>();
            altTabWindow.height = windowHeight;
            altTabWindow.backGroundHeight = backGroundHeight;
            altTabWindow.minimumWidth = windowMinimumWidth;
            altTabWindow.minimizedHeight = windowMinimalizedHeight;
            altTabWindow.uwcWindowTexture = windowTexture;
            altTabWindow.transform.SetParent(transform);
            altTabWindow.transform.localPosition = Vector3.zero;
            altTabWindow.appName = CropStr_R(windowTexture.window.title, " - ", false);
            
            altTabWindow.Init();
            altTabWindow.Visibe(false);

            windowTexture.transform.SetParent(altTabWindow.transform);
            windowTexture.transform.localPosition = Vector3.zero;

            await altTabWindow.SetWindow(windowTexture.window.title);

            //foreach (var audioVolume in GetComponentsInChildren<AudioProcessVolume>())
            //{
            //    Debug.Log($"ぼりゅーむつなぐ0 {audioVolume.Name}");

            //    if (altTabWindow.appName.Contains(audioVolume.Name, StringComparison.OrdinalIgnoreCase))
            //    {
            //        Debug.Log($"ぼりゅーむつなぐ1 {audioVolume.Name}");

            //        altTabWindow.soundSlider.OnValueChangedAsObservable().Subscribe(value =>
            //        {
            //            audioVolume.Volume.Value = value;
            //        });
            //    }
            //}


            windowTexture.On_ScaleChanged.Subscribe(async _ =>
            {
                //Debug.Log("HHスケール変更");
                await UniTask.WaitForEndOfFrame(windowTexture);
                await UpdateLayout();
            }).AddTo(windowTexture.gameObject);    
        }

       


        foreach (var altTabWindow in GetComponentsInChildren<AltTabWindow>())
        {
            //Debug.Log("配置");
            await UniTask.WaitForEndOfFrame(altTabWindow);

            var windowWidth = altTabWindow.background.transform.localScale.x;

            //Debug.Log($"はば {windowWidth}");

            if (posX + windowWidth * 0.5f + marginX > width)
            {
                posY -= windowHeight + marginY;
                posX = 0;
            }
            posX += windowWidth * 0.5f;
            //Debug.Log($"いち {posX}");

            altTabWindow.transform.localPosition = new Vector3(posX, posY, -0.1f);

            if (posX + windowWidth * 0.5f + marginX > width)
            {
                posY -= windowHeight + marginY;
                posX = 0;
            }
            posX += windowWidth * 0.5f + marginX;


            altTabWindow.button.On_Click.Subscribe(_ =>
            {
                for (int a = 0; a < windowUI.dropdown.options.Count; a++)
                {
                    if (windowUI.dropdown.options[a].text == altTabWindow.uwcWindowTexture.window.title)
                    {
                        windowUI.dropdown.value = a;
                    }
                }

                gameObject.SetActive(false);
            });

            altTabWindow.Visibe(true);
            //foreach (var a in GetComponentsInChildren<AltTabWindow>())
            //{
            //    if (a.transform.Find("uWC Window Object(Clone)") == null)
            //    {
            //        a.uwcWindowTexture = null;
            //        Destroy(a.gameObject);
            //    }
            //}
        }

        transform.localPosition = new Vector3(-width / 2, 0, -0.1f);
    }

    public void SetSoundSlider(string title)
    {
        foreach (var audioVolume in transform.parent.GetComponentsInChildren<AudioProcessVolume>())
        {
           
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
