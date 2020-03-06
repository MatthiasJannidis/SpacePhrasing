using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameSettings", menuName = "Create GameSettings")]
public class GameSettingsSO : ScriptableObject
{
    public KeyCode boostKey;
    public float boostSpeed;
}
