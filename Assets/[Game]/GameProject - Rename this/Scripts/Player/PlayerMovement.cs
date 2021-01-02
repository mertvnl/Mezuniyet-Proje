﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : Singleton<PlayerMovement>
{
    public float speed = 250f;
    public bool canMove = false;
    private Rigidbody rigidbody;
    private Animator animator;
    public Rigidbody Rigidbody
    {
        get
        {
            if (rigidbody == null)
            {
                rigidbody = GetComponent<Rigidbody>();
            }

            return rigidbody;
        }
    }

    public Animator Animator
    {
        get
        {
            if (animator == null)
            {
                animator = GetComponent<Animator>();
            }

            return animator;
        }
    }

    private void OnEnable()
    {
        EventManager.OnLevelStart.AddListener(()=> canMove = true);
        EventManager.OnLevelEnd.AddListener(FinishLevel);
        EventManager.OnLevelFail.AddListener(Die);
        EventManager.OnGameStart.AddListener(InitializePlayer);
    }

    private void OnDisable()
    {
        EventManager.OnLevelStart.RemoveListener(()=> canMove = true);
        EventManager.OnLevelEnd.RemoveListener(FinishLevel);
        EventManager.OnLevelFail.RemoveListener(Die);
        EventManager.OnGameStart.RemoveListener(InitializePlayer);
    }
    
    private void FixedUpdate()
    {
        if (GameManager.Instance.isGameStarted && canMove)
        {
            Vector3 dir = new Vector3(Input.GetAxis("Horizontal"), Rigidbody.velocity.y / 10 , 1);
            Rigidbody.velocity = dir * (speed * Time.fixedDeltaTime);
            Animator.SetTrigger("Run");
        }
    }

    public void Jump()
    {
        StartCoroutine(JumpCo());
    }

    IEnumerator JumpCo()
    {
        GetComponentInChildren<TrailRenderer>().enabled = true;
        canMove = false;
        Animator.SetTrigger("Fly");
        GetComponent<CapsuleCollider>().isTrigger = true;
        Rigidbody.velocity = new Vector3(0,1,1) * TheStick.Instance.transform.localScale.y * 5;
        TheStick.Instance.isJumping = false;
        yield return new WaitForSeconds(1f);
        GetComponent<CapsuleCollider>().isTrigger = false;
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("FinishPlatform"))
        {
            EventManager.OnLevelEnd.Invoke();
        }
    }

    private void InitializePlayer()
    {
        GameManager.Instance.gameData.currentPlayer = this.gameObject;
        GameManager.Instance.gameData.fullDistance =
            (JumpPoint.Instance.transform.position - transform.position).sqrMagnitude;
    }

    private void FinishLevel()
    {
        Animator.SetTrigger("Dance");
        transform.LookAt(Vector3.back);
        GameManager.Instance.isGameStarted = false;
        canMove = false;
    }

    private void Die()
    {
        //TODO die
        Debug.Log("die");
        Animator.SetTrigger("Fail");
        transform.LookAt(Vector3.back);
        GameManager.Instance.isGameStarted = false;
        canMove = false;
    }
}
