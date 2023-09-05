using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    public Vector2Int blockLoc;
    public int blockValue;
    private PlayControl playControl;

    private void Awake()
    {
        playControl = FindObjectOfType<PlayControl>();
    }

    public void OnButtonClick()
    {
        playControl.FillBlock(blockValue, blockLoc);
    }
}
