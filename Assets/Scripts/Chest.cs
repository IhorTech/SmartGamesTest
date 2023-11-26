using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Chest : MonoBehaviour
{
  [SerializeField]
  public Transform RewardsStartPosition;

  public Action OnChestOpen;

  public void ChestOpened()
  {
    OnChestOpen?.Invoke();
  }
}

