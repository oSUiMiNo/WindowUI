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
        // MainWindowHandle��0�̏ꍇ�A�E�C���h�E�������Ȃ����߃X�L�b�v
        // https://learn.microsoft.com/ja-jp/dotnet/api/system.diagnostics.process.mainwindowhandle?view=net-7.0#system-diagnostics-process-mainwindowhandle
        var handle = Process.MainWindowHandle;
        UnityEngine.Debug.Log("------------------------------");
        if (handle == IntPtr.Zero)
        {
            UnityEngine.Debug.Log($"===�Ȃ�{Name} | {Process.MainWindowTitle}===");
            return;
        }
        else
        {
            UnityEngine.Debug.Log($"===����{Name} | {Process.MainWindowTitle}===");
        }
        UnityEngine.Debug.Log($"����{Process.ProcessName} {ID} {handle}");

        var stringBuilder = new StringBuilder(256);
        var result = GetClassName(handle, stringBuilder, stringBuilder.Capacity);

        // 0��Ԃ��ꂽ�ꍇ�͎��s�̂��߃X�L�b�v
        // https://learn.microsoft.com/ja-jp/windows/win32/api/winuser/nf-winuser-getclassname
        if (result == 0)
        {
            UnityEngine.Debug.Log("�N���X���擾���s");
            return;
        }

        UnityEngine.Debug.Log($"{Process.ProcessName} {handle} {stringBuilder}");
    }


    [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);




    /// <summary>
    /// �w�肳�ꂽ��������܂ރE�B���h�E�^�C�g�������v���Z�X���擾���܂��B
    /// </summary>
    /// <param name="windowTitle">�E�B���h�E�^�C�g���Ɋ܂ޕ�����B</param>
    /// <returns>�Y������v���Z�X�̔z��B</returns>
    public System.Diagnostics.Process[] GetProcessesByWindowTitle(
        string windowTitle)
    {
        System.Collections.ArrayList list = new System.Collections.ArrayList();

        //���ׂẴv���Z�X��񋓂���
        foreach (System.Diagnostics.Process p
            in System.Diagnostics.Process.GetProcesses())
        {
            //�w�肳�ꂽ�����񂪃��C���E�B���h�E�̃^�C�g���Ɋ܂܂�Ă��邩���ׂ�
            if (0 <= p.MainWindowTitle.IndexOf(windowTitle))
            {
                //�܂܂�Ă�����A�R���N�V�����ɒǉ�
                list.Add(p);
            }
        }

        //�R���N�V������z��ɂ��ĕԂ�
        return (System.Diagnostics.Process[])
            list.ToArray(typeof(System.Diagnostics.Process));
    }
}