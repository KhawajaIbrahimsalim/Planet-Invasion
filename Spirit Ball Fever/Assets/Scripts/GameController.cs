using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

[System.Serializable]
public class GameController : MonoBehaviour
{
    private void Awake()
    {

    }

    public void Quit()
    {
        Application.Quit();
    }
}
