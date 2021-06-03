using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using TMPro.Examples;
using UnityEngine;
using UnityEngine.UI;

public class TextController : MonoBehaviour {
    private TextMeshProUGUI text;

    public String Text {
        set => text.SetText(value);
    }

    private void Start() {
        text = GetComponent<TextMeshProUGUI>();
    }
}
