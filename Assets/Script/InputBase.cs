using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InputBase : MonoBehaviour
{
    public abstract bool Jump();

    public abstract int MoveLeft();

    public abstract int MoveRight();
}
