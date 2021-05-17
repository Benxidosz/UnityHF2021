using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StartingFlag", menuName = "ScriptableObjects/newStartingFlag", order = 0)]
public class StartingFlagsSO : ScriptableObject {
    public bool playerVsPlayer;
    public bool playerIsCat;
}
