﻿using UnityEngine;
using System.Collections;


/// <summary>
/// Use standard event names for events that don't need parameters sent to them
/// </summary>
public enum StandardEventName
{
    None = 0,

    SetGraphicsQuality = 1,
    SetTerrainDetail = 2,
    MissionSuccessful = 3,
    MissionFailed = 4,
    ActivateCameraPan = 5
}


/// <summary>
/// Use boolean event names for events that need a True or False sent to them
/// </summary>
public enum BooleanEventName
{
    None = 0,

    ActivateHud = 1,
    ActivateRadar = 2,
    ActivateTargetSystem = 3,
    ActivateHealthMeter = 4,
}


/// <summary>
/// Use integer event names for events that need an integger sent to them
/// </summary>
public enum IntegerEventName
{
    None = 0,

    ChangeTerrainDetail = 1,
}


/// <summary>
/// Use string event names that need a string sent to them
/// </summary>
public enum StringEventName
{
    None = 0,

    ChangeGraphicsQuality = 1,
    PlayerDead = 2,
}


/// <summary>
/// Use float event names that need a float value sent to them
/// </summary>
public enum FloatEventName
{
    None = 0,

    SetThrustLevel = 1,
    SetMinThrustLevel = 2,
    SetMaxThrustLevel = 3,
}


/// <summary>
/// Use transform event names that need a transform component sent to them
/// </summary>
public enum TransformEventName
{
    None = 0,

    EnemyDead = 1,
}