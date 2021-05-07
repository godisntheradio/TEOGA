using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DynamicText : MonoBehaviour
{
    private Text text;

    private void Awake()
    {
        Initialize();
    }
    void Start()
    {
        Initialize();
    }
    public void UpdateText(int value)
    {
        text.text = value.ToString();
    }
    public void UpdateText(string value)
    {
        text.text = value;
    }

    public void Initialize()
    {
        text = GetComponent<Text>();
    }
}
