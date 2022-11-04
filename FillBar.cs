using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FillBar : MonoBehaviour
{

    public Slider slider;

    public void SetMaxValue ( int barValue )
    {
        slider.maxValue = barValue;
        slider.value = barValue;
    }

    public void SetCurrentValue ( int barValue )
    {
        slider.value = barValue;
    }

}
