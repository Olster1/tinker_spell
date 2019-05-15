using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHitBox
{
    void wasHit(int damage, string type, EnemyType enemyType, Vector2 position);
}