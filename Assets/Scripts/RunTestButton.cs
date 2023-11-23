using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RunTestButton : MonoBehaviour
{
    [SerializeField] private List<Reward> _rewards;
    [SerializeField] private Button _button;

    private void Start()
    {
        _button.onClick.AddListener(ReceiveReward);           
    }

    private void ReceiveReward()
    {
        ChestController.Instance.ReceiveReward(_rewards);
    }
}
