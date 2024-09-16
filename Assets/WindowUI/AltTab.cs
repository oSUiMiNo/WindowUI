using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using uWindowCapture;


public class AltTab : MonoBehaviour
{
    public static List<string> windowTitles = new List<string>();
    public static Dictionary<string, UwcWindow> windows = new Dictionary<string, UwcWindow>();

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.A))
        {
           GetAltTabWindows();
        }
    }

    private void Start()
    {

    }

    public static void GetAltTabWindows()
    {
        List<string> buffer_WindowTitles = new List<string>();
        Dictionary<string, UwcWindow> buffer_Windows = new Dictionary<string, UwcWindow>();
        UnityEngine.Debug.Log($"うぃんどう{UwcManager.windows.Count}");
        foreach (var window in UwcManager.windows.Values)
        {
            if (window.isAltTabWindow)
            {
                if (window.title.Contains("WindowUI - Test_WindowUI - Windows, Mac, Linux - Unity 6 Preview (6000.0.10f1)* <DX11>")) continue;
                if (window.title.Contains("WindowUI - Test_WindowUI - Windows, Mac, Linux - Unity 6 Preview (6000.0.10f1) <DX11>")) continue;

                buffer_WindowTitles.Add(window.title);
                string title = window.title;
                //if(buffer_Windows.ContainsKey(window.title)) title += 
                buffer_Windows.Add(window.title, window);

                //Process process = Process.GetProcessById(window.processId);
                //ProcessThreadCollection threads = process.Threads;
                ////ProcessThread thread = threads[window.threadId];
                //UnityEngine.Debug.Log(
                //    $"タイトル: {window.title}\n" +
                //    $"メインウィンドウ名 {process.MainWindowTitle}\n" +
                //    $"プロセスID {window.processId}\n" +
                //    $"スレッドID {window.threadId}\n" +
                //    $"ウィンドウハンドル {window.handle}\n" +
                //    $"プロセス名 {process.ProcessName}\n" +
                //    $"スレッド数 {threads.Count}\n"
                //);
            }
        }
        windowTitles = buffer_WindowTitles;
        windows = buffer_Windows;
    }
}