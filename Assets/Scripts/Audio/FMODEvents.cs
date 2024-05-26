using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class FMODEvents : MonoBehaviour
{
    [field: Header("Music")]
    [field: SerializeField] public EventReference music { get; private set; }

    [field: Header("Player sounds")]
    [field: SerializeField] public EventReference footsteps { get; private set; }
    [field: SerializeField] public EventReference jump { get; private set; }
    [field: SerializeField] public EventReference doubleJump { get; private set; }
    [field: SerializeField] public EventReference landing { get; private set; }
    [field: SerializeField] public EventReference dash { get; private set; }

    public static FMODEvents instance { get; private set; }

    private void Awake() {
        //singleton
        if (instance != null) {
            Debug.LogError("More than one FMODEvents instance");
        }
        instance = this;
    }
}
