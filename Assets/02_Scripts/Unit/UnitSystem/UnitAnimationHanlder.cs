using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UnitAnimationHanlder 
{
    private Dictionary<EnemyAnimationType, Action> typeToAction;
    private Animator animator;

    public UnitAnimationHanlder(Animator ani) 
    {
        this.animator = ani;
        InitDictionary();
    }

    public void ChangeAnimation(EnemyAnimationType type) 
    {
        Action action;
        if(typeToAction.TryGetValue(type, out action)) 
        {
            action?.Invoke();
        }
    }

    private void InitDictionary() 
    { 
        typeToAction = new Dictionary<EnemyAnimationType, Action>() 
        {
            { EnemyAnimationType.Run , () =>
                {
                    animator.SetBool("Run" , true);
                }
            }, 
            { EnemyAnimationType.Die , () =>
                {
                    animator.SetBool("Die" , true);
                }
            }
        };
    }
}
