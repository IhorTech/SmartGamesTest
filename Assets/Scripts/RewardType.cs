using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum RewardType 
{
    Coin,
    Heart,
    Search,
    Tape,
    Compass,
    Ticket
}

[Serializable]
public class Reward
{
    public RewardType RewardType;
    public int Amount;
}