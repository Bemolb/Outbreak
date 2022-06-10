using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimatorService : IAnimatorService
{
    private Animator _animator;
    private Player _player;
    private System.Random _random;

    public PlayerAnimatorService(Player player)
    {
        _animator = player.GetComponent<Animator>();
        _player = player;
        _random = new System.Random();
    }

    public void PlayAttackAnim()
    {
        throw new System.NotImplementedException();
    }

    public void PlayDeathAnim()
    {
        int animNum = _random.Next(0, 3);
        _animator.SetInteger("deathAnim", animNum);
        _animator.SetTrigger("death");
    }

    public void PlayMoveAnim()
    {
        _animator.SetBool("isMove", true);
        float currSpeed = (float)Math.Round(_player.Agent.velocity.magnitude, 2);
        if (currSpeed > _player.MoveSpeed)
            _animator.SetBool("isRun", true);
        else
            _animator.SetBool("isRun", false);
        _animator.speed = currSpeed / _player.Agent.speed;
    }

    public void StopAttackAnim()
    {
        throw new System.NotImplementedException();
    }

    public void StopMoveAnim()
    {
        _animator.SetBool("isMove", false);
        _animator.SetBool("isRun", false);
    }
}
