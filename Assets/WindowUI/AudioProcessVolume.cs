using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

public class AudioProcessVolume : MonoBehaviour
{
    public string Name;
    public FloatReactiveProperty Volume = new FloatReactiveProperty(1);
    public List<AudioProcessUnit> units = new List<AudioProcessUnit>();


    private void Start()
    {
        Volume.Subscribe(volume =>
        {
            foreach (var unit in units)
            {
                unit.Volume.Value = volume;
            }
        });
    }

    public void Init(string name)
    {
        Name = name;
        units = GetComponents<AudioProcessUnit>().ToList();
    }

    private void OnApplicationQuit()
    {
        Volume.Value = 1;
    }
}
