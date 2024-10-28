using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SceneDataHolder : MonoBehaviour
{
    public TMP_Text txtScore;
    public TMP_Text txtDeathCount;

    public static SceneDataHolder instance;
    private void Awake()
    {
        instance = this;
    }
}
