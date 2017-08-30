using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class FollowCamera : MonoBehaviour {

    public GameObject FollowObject;
    public float MaxOffset;
    public float OffsetFactor;
    public float MinOffset;
    public List<GameObject> ParalaxList;
    public List<Vector2> ParalaxFactor;
    public float CursorLeadFactor;

    List<ParalaxScreen> paralaxList;

    public class ParalaxScreen
    {
        GameObject Screen;
        Vector2 Factor;
        Vector3 startPos;

        public ParalaxScreen(GameObject screen, Vector2 factor)
        {
            Screen = screen;
            Factor = factor;
            startPos = Screen.transform.localPosition;
        }

        public void SetFactor(Vector3 factor)
        {
            Factor = factor;
        }

        public void MoveParalax(Vector2 offset)
        {
            Screen.transform.localPosition = startPos - new Vector3(offset.x * Factor.x, offset.y * Factor.y, 0);
        }
    }

    // Use this for initialization
    void Start ()
    {
        paralaxList = new List<ParalaxScreen>();
        for (int i = 0; i < ParalaxList.Count; i++)
            paralaxList.Add(new ParalaxScreen(ParalaxList[i], i < ParalaxFactor.Count ? ParalaxFactor[i] : Vector2.zero));
	}

    // Update is called once per frame
    void MoveCam()
    {
        Vector2 destination = (FollowObject.transform.position + (this.GetComponent<Camera>().ScreenToWorldPoint(Input.mousePosition) - FollowObject.transform.position) * CursorLeadFactor);
        Vector2 offset = (Vector2)transform.position - destination;
        if (offset.magnitude < MinOffset)
        {
            transform.position = new Vector3(destination.x, destination.y, transform.position.z);
            return;
        }
        offset *= OffsetFactor;
        if (offset.magnitude > MaxOffset)
            offset = offset.normalized * MaxOffset;
        transform.position -= (Vector3)offset;
    }

    void MoveParalax()
    {
        for (int i = 0; i < paralaxList.Count; i ++)
        {
            paralaxList[i].SetFactor(ParalaxFactor[i]);
            paralaxList[i].MoveParalax(transform.position);
        }
    }

    void Update ()
    {
        MoveCam();
        MoveParalax();
    }
}
