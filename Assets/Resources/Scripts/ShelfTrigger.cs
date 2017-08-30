using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShelfTrigger : MonoBehaviour {
    public MessageManager Messages;

    void OnTriggerEnter2D(Collider2D collider)
    {
        Messages.Trigger("Hanging", collider);
    }
}
