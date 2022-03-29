using System;
using UnityEngine;
public interface ISelectable
{
    bool selected { get; set; }
    bool isOrderable { get; set; }
    BoxCollider selectionCollider { get; set; }
    SpriteRenderer selectedSprite { get; set; }
}
