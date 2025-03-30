using System;
using UnityEngine;
using UnityEngine.Serialization;

public class Trigger : MonoBehaviour
{
    [Header("CloudComponent to follow")]
     
    // the associatedClouds why public?
    [SerializeField] private int state = 1;
    [FormerlySerializedAs("triggerdColor")] [SerializeField] private ColorType triggerdColorType;
    // [SerializeField] public CloudComponent[] associatedClouds;
    public static event Action<ColorType> Vanish;
    private Transform cloudTransform;

    public void SetupCloud(Transform cloudTransform)
    {
        this.cloudTransform = cloudTransform;
    }
    
    private void Update()
    {
        if (cloudTransform != null)
        {
            transform.position = new Vector3(cloudTransform.position.x,
                cloudTransform.position.y, cloudTransform.position.z - 3);
        }
    }
    
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            state = 0;
            Vanish?.Invoke(triggerdColorType);
        }
    }

    public void SetColor(ColorType colorType)
    {
        triggerdColorType = colorType;
    }
    
}