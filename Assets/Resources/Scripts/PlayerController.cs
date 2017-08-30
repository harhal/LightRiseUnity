using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Character))]
public class PlayerController : MonoBehaviour {

    public MessageManager Messages;

    public KeyCode JumpKey1;
    public KeyCode JumpKey2;
    public KeyCode LeftKey1;
    public KeyCode LeftKey2;
    public KeyCode RightKey1;
    public KeyCode RightKey2;
    public KeyCode UpKey1;
    public KeyCode UpKey2;
    public KeyCode DownKey1;
    public KeyCode DownKey2;
    public KeyCode CrouchKey1;
    public KeyCode CrouchKey2;
    public KeyCode UseKey1;
    public KeyCode UseKey2;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (((Input.GetKey(LeftKey1) || Input.GetKey(LeftKey2)) ^ (Input.GetKey(RightKey1) || Input.GetKey(RightKey2))))
        {
            if (Input.GetKey(LeftKey1) || Input.GetKey(LeftKey2))
                if (transform.localScale.x < 0)
                    Messages.Trigger("Go", this);
                else
                    Messages.Trigger("TurnBack", this);
            if (Input.GetKey(RightKey1) || Input.GetKey(RightKey2))
                if (transform.localScale.x > 0)
                    Messages.Trigger("Go", this);
                else
                    Messages.Trigger("TurnBack", this);
        }
        else
        {
            Messages.Trigger("Stop", this);
        }

        if (((Input.GetKey(UpKey1) || Input.GetKey(UpKey2)) ^ (Input.GetKey(DownKey1) || Input.GetKey(DownKey2))))
        {
            if (Input.GetKeyDown(UpKey1) || Input.GetKeyDown(UpKey2))
                Messages.Trigger("Up", this);
            if (Input.GetKeyDown(DownKey1) || Input.GetKeyDown(DownKey2))
                Messages.Trigger("Down", this);
        }
        else
        {
            Messages.Trigger("VerticalStop", this);
        }

        if (Input.GetKeyDown(JumpKey1) || Input.GetKeyDown(JumpKey2))
            Messages.Trigger("Jump", this);
        if (Input.GetKeyDown(CrouchKey1) || Input.GetKeyDown(CrouchKey2))
            Messages.Trigger("Crouch", this);
        if (Input.GetKeyDown(UseKey1) || Input.GetKeyDown(UseKey2))
            Messages.Trigger("Use", this);
    }
}
