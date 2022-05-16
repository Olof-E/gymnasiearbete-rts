using System;
using UnityEngine;
public interface ISelectable
{
    Vector3 selectablePosition { get; set; }
    bool selected { get; set; }
    bool isOrderable { get; set; }
    Renderer boundsRenderer { get; set; }
    BoxCollider selectionCollider { get; set; }
    SpriteRenderer selectedSprite { get; set; }
}
