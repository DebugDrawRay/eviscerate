using UnityEngine;
using System.Collections;
using InControl;

public class PlayerActions : PlayerActionSet
{
    public PlayerAction attack;
    public PlayerAction target;
    public PlayerAction right;
    public PlayerAction left;
    public PlayerAction up;
    public PlayerAction down;
    public PlayerTwoAxisAction move;

    public PlayerActions()
    {
        attack = CreatePlayerAction("Attack");
        target = CreatePlayerAction("Target");
        right = CreatePlayerAction("Move Right");
        left = CreatePlayerAction("Move Left");
        up = CreatePlayerAction("Move Up");
        down = CreatePlayerAction("Move Down");
        move = CreateTwoAxisPlayerAction(left, right, down, up);
    }
}
