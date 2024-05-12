using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossDoor : MonoBehaviour
{
    Animator animator;
    float doorAnimationLenght;

    BoxCollider2D bc;

    public float DoorAnimationLenght { get => doorAnimationLenght; }

    private void Awake()
    {
        animator = GetComponent<Animator>();
        bc = GetComponent<BoxCollider2D>();

        animator.Play("BossDoor_Opening");
        doorAnimationLenght = animator.GetCurrentAnimatorClipInfo(0)[0].clip.length;
        animator.Play("BossDoor_Idle");
    }

    public void Open()
    {
        animator.Play("BossDoor_Opening");
        bc.enabled = false;
    }

    public void Close()
    {
        animator.Play("BossDoor_Closing");
        bc.enabled = true;
    }
}
