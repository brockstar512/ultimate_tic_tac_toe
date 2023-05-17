using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorPlayer : MonoBehaviour
{
    [SerializeField]
    string AnimationName;
    void Start() => GetComponent<Animator>().Play(AnimationName);
    
}
