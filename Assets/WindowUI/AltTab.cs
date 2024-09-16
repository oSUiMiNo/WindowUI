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
        UnityEngine.Debug.Log($"������ǂ�{UwcManager.windows.Count}");
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
                //    $"�^�C�g��: {window.title}\n" +
                //    $"���C���E�B���h�E�� {process.MainWindowTitle}\n" +
                //    $"�v���Z�XID {window.processId}\n" +
                //    $"�X���b�hID {window.threadId}\n" +
                //    $"�E�B���h�E�n���h�� {window.handle}\n" +
                //    $"�v���Z�X�� {process.ProcessName}\n" +
                //    $"�X���b�h�� {threads.Count}\n"
                //);
            }
        }
        windowTitles = buffer_WindowTitles;
        windows = buffer_Windows;
    }
}