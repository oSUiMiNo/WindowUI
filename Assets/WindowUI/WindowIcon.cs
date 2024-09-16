using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Codice.Client.Common;
using UnityEngine.UI;

public class WindowIcon : MonoBehaviourMyExtention
{
    WindowUI windowUI;
    [SerializeField] GameObject windows;


    private void Start()
    {
        windowUI = transform.parent.parent.GetComponent<WindowUI>();

        Button button = GetComponent<Button>();
        button.OnClickAsObservable().Subscribe(_ => 
        {
            windows.SetActive(true);
        });
    }
}
 