using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class RewardPrefab : MonoBehaviour
{
    public RewardType RewardType;
    public Image Image;
    public TMP_Text AmountText;
    
    public void Init (int amount)
    {
        AmountText.text = "x" + amount.ToString();
    }
}
