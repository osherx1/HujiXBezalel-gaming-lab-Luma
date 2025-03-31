using System;
using Enums;
using Player;
using UnityEngine;
public class Point:MonoBehaviour
{

    //adding animation.
    [SerializeField] private int state = 1;

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            state = 0;
        }
    }
}
