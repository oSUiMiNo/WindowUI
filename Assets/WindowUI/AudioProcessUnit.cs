using NAudio.CoreAudioApi;
using System.Diagnostics;
using UnityEngine;
using UniRx;
using System;
using UnityEditor;
using System.Text;
using System.Runtime.InteropServices;

[RequireComponent(typeof(AudioProcessVolume))]
public class AudioProcessUnit : MonoBehaviour
{
    public int ID;
    public string Name;
    public FloatReactiveProperty Volume = new FloatReactiveProperty(1);

    public Process Process;
    public AudioSessionControl Session;

    public void Init(Process process, AudioSessionControl session)
    {
        Session = session;
        Process = process;
        ID = (int)session.GetProcessID;
        Name = process.ProcessName;
        session.SimpleAudioVolume.Volume = 1;
        Volume.Value = session.SimpleAudioVolume.Volume;
        //UnityEngine.Debug.Log("-----------------------");
        //UnityEngine.Debug.Log(session.SimpleAudioVolume.Volume.ToString());
        //UnityEngine.Debug.Log($"{ID}  |  {Name}  |  {Volume.Value}");

        GetComponent<AudioProcessVolume>().Init(Name);

        Volume.Subscribe(value =>
        {
            session.SimpleAudioVolume.Volume = value;
            //UnityEngine.Debug.Log($"{ID}  |  {Name}  |  {Volume.Value}");
        });


        GetWindowInfo();
    }

    void GetWindowInfo()
    {
        // MainWindowHandleが0の場合、ウインドウを持たないためスキップ
        // https://learn.microsoft.com/ja-jp/dotnet/api/system.diagnostics.process.mainwindowhandle?view=net-7.0#system-diagnostics-process-mainwindowhandle
        var handle = Process.MainWindowHandle;
        UnityEngine.Debug.Log("------------------------------");
        if (handle == IntPtr.Zero)
        {
            UnityEngine.Debug.Log($"===なし{Name} | {Process.MainWindowTitle}===");
            return;
        }
        else
        {
            UnityEngine.Debug.Log($"===あり{Name} | {Process.MainWindowTitle}===");
        }
        UnityEngine.Debug.Log($"あり{Process.ProcessName} {ID} {handle}");

        var stringBuilder = new StringBuilder(256);
        var result = GetClassName(handle, stringBuilder, stringBuilder.Capacity);

        // 0を返された場合は失敗のためスキップ
        // https://learn.microsoft.com/ja-jp/windows/win32/api/winuser/nf-winuser-getclassname
        if (result == 0)
        {
            UnityEngine.Debug.Log("クラス名取得失敗");
            return;
        }

        UnityEngine.Debug.Log($"{Process.ProcessName} {handle} {stringBuilder}");
    }


    [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);




    /// <summary>
    /// 指定された文字列を含むウィンドウタイトルを持つプロセスを取得します。
    /// </summary>
    /// <param name="windowTitle">ウィンドウタイトルに含む文字列。</param>
    /// <returns>該当するプロセスの配列。</returns>
    public System.Diagnostics.Process[] GetProcessesByWindowTitle(
        string windowTitle)
    {
        System.Collections.ArrayList list = new System.Collections.ArrayList();

        //すべてのプロセスを列挙する
        foreach (System.Diagnostics.Process p
            in System.Diagnostics.Process.GetProcesses())
        {
            //指定された文字列がメインウィンドウのタイトルに含まれているか調べる
            if (0 <= p.MainWindowTitle.IndexOf(windowTitle))
            {
                //含まれていたら、コレクションに追加
                list.Add(p);
            }
        }

        //コレクションを配列にして返す
        return (System.Diagnostics.Process[])
            list.ToArray(typeof(System.Diagnostics.Process));
    }
}