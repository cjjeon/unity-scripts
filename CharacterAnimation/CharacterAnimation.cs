using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CharacterAnimation : MonoBehaviour
{
    private Animator _animator;
    
    // Start is called before the first frame update
    private void Start()
    {
        _animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    private void Update()
    {
        // Depending on the setup, Set animation parameter for your need.
        // In this example, Speed_f is parameter to tell if character is in which state.
        // Set Animation to Idle
        _animator.SetFloat("Speed_f", 0f);
        
        // Set Animation to Running
        _animator.SetFloat("Speed_f", 0.5f);
    }
}