using System.Collections;
using UnityEngine;

public class ExplosionHandler : MonoBehaviour
{
    private Animator animator;
    void Start()
    {
        animator = GetComponent<Animator>();

        float animationLength = animator.GetCurrentAnimatorStateInfo(0).length;

        Destroy(gameObject, animationLength);
    }
}
