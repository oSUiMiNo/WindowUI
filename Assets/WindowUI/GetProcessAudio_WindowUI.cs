using NAudio.CoreAudioApi;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;

public class GetProcessAudio_WindowUI : MonoBehaviour
{
    void Start()
    {
        List<AudioProcessUnit> audioProcesses = GetAudioProcesses();
        audioProcesses.ForEach(a =>
        {
            UnityEngine.Debug.Log($"{a.ID}  |  {a.Name}  |  {a.Volume} | {a.Process.MainWindowTitle}");
        });

        //List<Process> processes = Process.GetProcesses().ToList();
        //processes.ForEach(a =>
        //{
        //    //UnityEngine.Debug.Log(a.ProcessName);
        //    if(a.MainWindowTitle.ToString() != "null") UnityEngine.Debug.Log(a.MainWindowTitle);
        //});
    }


    void Update()
    {
        
    }


    //これ実行するとプロセスid,プロセス名,オーディオ音量をリストにして返してくれる
    public List<AudioProcessUnit> GetAudioProcesses()
    {
        var audioProcesses = new List<AudioProcessUnit>();
        var enumerator = new MMDeviceEnumerator();
        var devices = enumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active);

        foreach (var device in devices)
        {
            UnityEngine.Debug.Log(device.DeviceFriendlyName);

            var sessions = device.AudioSessionManager.Sessions;

            for (int i = 0; i < sessions.Count; i++)
            {
                var session = sessions[i];
                var processId = session.GetProcessID;
                var process = GetProcessFromId((int)processId);

                if (process != null)
                {
                    AudioProcessUnit audioProcess;
                    if (GameObject.Find(process.ProcessName) == null)
                    {
                        audioProcess = new GameObject(process.ProcessName).AddComponent<AudioProcessUnit>();
                    }
                    else
                    {
                        audioProcess = GameObject.Find(process.ProcessName).AddComponent<AudioProcessUnit>();
                    }
                    audioProcess.transform.SetParent(transform);
                    audioProcess.Init(process, session);
                    audioProcesses.Add(audioProcess);
                }
            }
        }

        return audioProcesses;
    }



    private Process GetProcessFromId(int processId)
    {
        try
        {
            return Process.GetProcessById(processId);
        }
        catch (ArgumentException)
        {
            // プロセスが存在しない場合はnullを返す
            return null;
        }
    }
}
