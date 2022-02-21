using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Healthbar : MonoBehaviour
{
    public Image healthbar; // brings in health bar image

    public void UpdateHealth(float fraction)
    {
        healthbar.fillAmount = 0.5f; // changes how much the healthbar is filled
    }
}
