
using System;
using UnityEngine;

namespace LittelSword.InputSystem
{
    public interface IInputEvents
    {
        event Action<Vector2> OnMove;
        event Action OnAttack;
    }
}

