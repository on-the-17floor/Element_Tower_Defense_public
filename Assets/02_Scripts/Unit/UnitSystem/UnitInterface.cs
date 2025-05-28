using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IRun
{
    public void IRun(Transform[] point, Action Action = null);
}

public interface IRecovery
{
    public void IRecovery(Action action = null);
}

public class UnitInterface 
{

}
