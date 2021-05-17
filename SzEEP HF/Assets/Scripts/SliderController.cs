using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderController : MonoBehaviour {
    private Image _image;

    public float Slider {
        set => _image.fillAmount = value;
    }
    
    void Awake() {
        _image = GetComponent<Image>();
    }
}
