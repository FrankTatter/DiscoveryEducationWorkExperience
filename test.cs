using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    [SerializeField] private Transform[] points;
    [SerializeField] private lineConroller line;

    private void Start()
    {
        line.setUpLine(points);
    }
}
