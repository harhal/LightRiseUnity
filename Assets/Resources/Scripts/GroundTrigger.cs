using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundTrigger : MonoBehaviour {

    public List<PhysicsMaterial2D> GroundMaterial;
    public MessageManager Messages;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

    }

    void OnTriggerEnter2D(Collider2D other)
    {
        /*if (other.gameObject.layer == 8)
            Messages.Trigger("TouchDown", this);*/
    }

    void OnTriggerStay2D(Collider2D other)
    {
        int a = other.gameObject.layer;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        /*Collider2D collider = GetComponent<Collider2D>();
        ContactFilter2D filter = new ContactFilter2D();
        Collider2D[] list = new Collider2D[5];
        int res = collider.OverlapCollider(filter.NoFilter(), list);
        if (res <= 1)
            Messages.Trigger("MissDown", this);*/
        if (!GetComponent<Collider2D>().IsTouchingLayers(Convert.ToInt32("100000000", 2)))
            Messages.Trigger("MissDown", this);
    }
}
