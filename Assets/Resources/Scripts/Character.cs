using DragonBones;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class CharacterAnimation : MonoBehaviour
{
    /// <summary>
    /// Анимация взаимодействия с интерактивными объектами
    /// </summary>
    public abstract void Use();
    /// <summary>
    /// Анимация бездействия
    /// </summary>
    public abstract void Idle();

    /// <summary>
    /// Анимация бездействия с упором в стену
    /// </summary>
    public abstract void IdleAtWall();

    /// <summary>
    /// Анимация движения по земле
    /// </summary>
    public abstract void Run();

    /// <summary>
    /// Анимация прыжка
    /// </summary>
    public abstract void Jump();

    /// <summary>
    /// Анимация падения
    /// </summary>
    public abstract void Fall();

    /// <summary>
    /// Анимация приземления
    /// </summary>
    public abstract void TouchDown();

    /// <summary>
    /// Анимация, используемая, когда персонаж висит на уступе
    /// </summary>
    public abstract void Hanging();

    /// <summary>
    /// Анимация залезания на уступ
    /// </summary>
    public abstract void Climb();

    /// <summary>
    /// Анимация бездействия при скрытном перемещении
    /// </summary>
    public abstract void CrawlIdle();

    /// <summary>
    /// Анимация движения при скрытном перемещении
    /// </summary>
    public abstract void CrawlGo();

    /// <summary>
    /// Анимация перехода в скрытное перемещение
    /// </summary>
    public abstract void StartCrawl();

    /// <summary>
    /// Анимация перехода из скрытного перемещения
    /// </summary>
    public abstract void StopCrawl();

    /// <summary>
    /// Анимация перемещения по лестнице вверх
    /// </summary>
    public abstract void LadderGoUp();

    /// <summary>
    /// Изменение скорости подъёма по лестнице
    /// </summary>
    public abstract void LadderIdle();

    /// <summary>
    /// Анимация перемещения по лестнице вверх
    /// </summary>
    public abstract void LadderGoDown();

    /// <summary>
    /// Анимация поражения
    /// </summary>
    public abstract void Defeat();
}

public interface IInteractiveObject
{
    void Use(Character Caller);

    void HighLightOn();

    void HighLightOff();
}

class CharacterState : State
{
    protected float SpeedFactor = 1;

    protected Character CharacterObject;

    protected MessageManager Messages;

    public CharacterState(Character CharacterObject, MessageManager Messages, Action<State> ChangeState) : base(ChangeState) {
        this.CharacterObject = CharacterObject;
        this.Messages = Messages;
    }

    protected bool CheckForward()
    {
        ContactPoint2D[] points = new ContactPoint2D[32];
        ContactFilter2D filter = new ContactFilter2D()
        {
            useLayerMask = true,
            layerMask = LayerMask.GetMask("LevelGround")
        };
        int PCount = CharacterObject.GetComponent<Collider2D>().GetContacts(points);
        for (int i = 0; i < PCount; i++)
        {
            if (Vector2.Dot(points[i].normal, -new Vector2(CharacterObject.transform.lossyScale.x, 0).normalized) > 1 - Character.horisAngle)
                return false;
        }
        return true;
    }

    protected void Go(object sender)
    {
        int sign = CharacterObject.transform.localScale.x > 0 ? 1 : -1;
        if (CheckForward())
        {
            CharacterObject.GetComponent<Rigidbody2D>().velocity = new Vector2(CharacterObject.GoVelocity * sign, CharacterObject.GetComponent<Rigidbody2D>().velocity.y);
        }
        else Stop(sender);
    }

    protected void Stop(object sender)
    {
        CharacterObject.GetComponent<Rigidbody2D>().velocity = new Vector2(0, CharacterObject.GetComponent<Rigidbody2D>().velocity.y);
    }

    protected void TurnBack(object sender)
    {
        UnityEngine.Transform tf = CharacterObject.transform;
        tf.localScale = new Vector3(-tf.localScale.x, tf.localScale.y, tf.localScale.z);
    }
}

class StandCharacterState : CharacterState
{
    bool _state;
    bool CrawlState
    {
        get
        {
            return _state;
        }
        set
        {
            _state = value;
        }
    }

    public StandCharacterState(Character CharacterObject, MessageManager Messages, Action<State> ChangeState) : base(CharacterObject, Messages, ChangeState) { }

    void Crouch(object sender)
    {
        CrawlState = !CrawlState;
        if (CrawlState)
            CharacterObject.Animation.StartCrawl();
        else
            CharacterObject.Animation.StopCrawl();
    }

    new void Go(object sender)
    {
        int sign = CharacterObject.transform.localScale.x > 0 ? 1 : -1;
        if (CrawlState)
        {
            if (CheckForward())
            {
                CharacterObject.Animation.CrawlGo();
                CharacterObject.GetComponent<Rigidbody2D>().velocity = new Vector2(CharacterObject.GoVelocity * sign * CharacterObject.CrawlVelocityFactor, CharacterObject.GetComponent<Rigidbody2D>().velocity.y);
            }
            else
            {
                CharacterObject.Animation.CrawlIdle();
                CharacterObject.Animation.IdleAtWall();
                Stop(sender);
            }
        }
        else
        {
            if (CheckForward())
            {
                CharacterObject.Animation.Run();
                CharacterObject.GetComponent<Rigidbody2D>().velocity = new Vector2(CharacterObject.GoVelocity * sign, CharacterObject.GetComponent<Rigidbody2D>().velocity.y);
            }
            else
            {
                CharacterObject.Animation.IdleAtWall();
                Stop(sender);
            }
        }
    }

    new void Stop(object sender)
    {
        if (CrawlState)
        {
            CharacterObject.Animation.CrawlIdle();
        }
        else
        {
            CharacterObject.Animation.Idle();
        }
        base.Stop(sender);
    }

    new void TurnBack(object sender)
    {
        if (CrawlState)
        {
            CharacterObject.Animation.CrawlGo();
        }
        else
        {
            CharacterObject.Animation.Run();
        }
        base.TurnBack(sender);
    }

    void Fall(object sender)
    {
        ChangeState(CharacterObject.FallCharacterState);
    }

    void Jump(object sender)
    {
        CharacterObject.Animation.Jump();
        CharacterObject.GetComponent<Rigidbody2D>().velocity = new Vector3(CharacterObject.GetComponent<Rigidbody2D>().velocity.x, CharacterObject.JumpVelocity, 0);
    }

    void Use(object sender)
    {
        var Interaction = CharacterObject.GetNearestInteraction();
        if (Interaction != null)
        {
            CharacterObject.Animation.Use();
            (Interaction.GetComponent<IInteractiveObject>()).Use(CharacterObject);
        }
    }

    public override void Start()
    {
        CrawlState = false;
        Messages.Add("Go", Go);
        Messages.Add("Stop", Stop);
        Messages.Add("Use", Use);
        Messages.Add("Jump", Jump);
        Messages.Add("Crouch", Crouch);
        Messages.Add("TurnBack", TurnBack);
        Messages.Add("MissDown", Fall);
    }

    public override void End()
    {
        Messages.Remove("Go", Go);
        Messages.Remove("Stop", Stop);
        Messages.Remove("Use", Use);
        Messages.Remove("Jump", Jump);
        Messages.Remove("Crouch", Crouch);
        Messages.Remove("TurnBack", TurnBack);
        Messages.Remove("MissDown", Fall);
    }
}

class FallCharacterState : CharacterState
{
    public FallCharacterState(Character CharacterObject, MessageManager Messages, Action<State> ChangeState) : base(CharacterObject, Messages, ChangeState) {
        SpeedFactor = 0.1f;
    }

    void TouchDown(object sender)
    {
        CharacterObject.Animation.TouchDown();
        ChangeState(CharacterObject.StandCharacterState);
    }

    void Hanging(object sender)
    {
        Collider2D secPoint = sender as Collider2D;
        if (secPoint != null)
        {
            CharacterObject.transform.position = secPoint.transform.position - (CharacterObject.HangingPoint.transform.position - CharacterObject.transform.position);
            CharacterObject.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
            CharacterObject.HangingCharacterState.SafeRange = secPoint.transform.lossyScale.y + CharacterObject.HangingPoint.transform.lossyScale.y;
            ChangeState(CharacterObject.HangingCharacterState);
        }
    }

    void Use(object sender)
    {
        var Interaction = CharacterObject.GetNearestInteraction();
        if (Interaction != null)
        {
            CharacterObject.Animation.Use();
            (Interaction.GetComponent<IInteractiveObject>()).Use(CharacterObject);
        }
    }

    public override void Start()
    {
        CharacterObject.Animation.Fall();
        Messages.Add("Go", Go);
        Messages.Add("Hanging", Hanging);
        Messages.Add("TouchDown", TouchDown);
        Messages.Add("TurnBack", TurnBack);
        Messages.Add("Use", Use);
    }

    public override void End()
    {
        Messages.Remove("Go", Go);
        Messages.Remove("Hanging", Hanging);
        Messages.Remove("TouchDown", TouchDown);
        Messages.Remove("TurnBack", TurnBack);
        Messages.Remove("Use", Use);
    }
}

class HangingCharacterState : CharacterState
{

    public float SafeRange { get; set; }

    public HangingCharacterState(Character CharacterObject, MessageManager Messages, Action<State> ChangeState) : base(CharacterObject, Messages, ChangeState) { }

    void Up(object sender)
    {
        ChangeState(CharacterObject.ClimbCharacterState);
    }

    void Down(object sender)
    {
        CharacterObject.transform.position -= new Vector3(0, SafeRange);
        ChangeState(CharacterObject.FallCharacterState);
    }

    new void TurnBack(object sender)
    {
        base.TurnBack(sender);
        ChangeState(CharacterObject.FallCharacterState);
    }

    public override void Start()
    {
        CharacterObject.Animation.Hanging();
        CharacterObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
        Messages.Add("Up", Up);
        Messages.Add("Down", Down);
        Messages.Add("TurnBack", TurnBack);
    }

    public override void End()
    {
        CharacterObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
        Messages.Remove("Up", Up);
        Messages.Remove("Down", Down);
        Messages.Remove("TurnBack", TurnBack);
    }
}

class ClimbCharacterState : CharacterState
{
    Vector3 StartPosition;
    float Progress;

    public ClimbCharacterState(Character CharacterObject, MessageManager Messages, Action<State> ChangeState) : base(CharacterObject, Messages, ChangeState) { }

    public override void Start()
    {
        CharacterObject.Animation.Climb();
        Progress = 0;
        StartPosition = CharacterObject.transform.position;
        CharacterObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
    }

    public override void Update()
    {
        Progress += Time.deltaTime;
        if (Progress > CharacterObject.ClimbDuration)
        {
            ChangeState(CharacterObject.StandCharacterState);
            return;
        }
        Vector2 Offset = CharacterObject.ClimbUpPath.GetPointAt(Progress / CharacterObject.ClimbDuration) - CharacterObject.transform.position;
        CharacterObject.transform.position = StartPosition + (Vector3)Offset;
        if (Progress == CharacterObject.ClimbDuration)
        {
            ChangeState(CharacterObject.StandCharacterState);
        }
    }

    public override void End()
    {
        CharacterObject.Animation.Idle();
        CharacterObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
    }
}

class KinematicState : CharacterState
{
    public KinematicState(Character CharacterObject, MessageManager Messages, Action<State> ChangeState) : base(CharacterObject, Messages, ChangeState) { }

    public void EndKinematic(object sender)
    {
        ChangeState(CharacterObject.StandCharacterState);
    }

    public override void Start()
    {
        Messages.Add("EndKinematic", EndKinematic);
    }

    public override void End()
    {
        Messages.Remove("EndKinematic", EndKinematic);
    }
}

class LadderBreakState : CharacterState
{
    float Progress;
    float NextVelocity;

    public LadderBreakState(Character CharacterObject, MessageManager Messages, Action<State> ChangeState) : base(CharacterObject, Messages, ChangeState) { }

    private void Up(object invoker)
    {
        NextVelocity = CharacterObject.LadderVelocity;
    }

    private void VerticalStop(object invoker)
    {
        NextVelocity = 0;
    }

    private void Down(object invoker)
    {
        NextVelocity = -CharacterObject.LadderVelocity;
    }

    public override void Start()
    {
        CharacterObject.Animation.LadderGoDown();
        Progress = 0;
        NextVelocity = -CharacterObject.LadderVelocity;
        CharacterObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
        Messages.Add("Up", Up);
        Messages.Add("VerticalStop", VerticalStop);
        Messages.Add("Down", Down);
    }

    public override void Update()
    {
        Progress += Time.deltaTime;
        CharacterObject.transform.position -= new Vector3(0, CharacterObject.LadderVelocity * Time.deltaTime, 0);
        if (Progress > CharacterObject.ClimbDuration)
        {
            ChangeState(CharacterObject.LadderState);
            CharacterObject.LadderState.Velocity = NextVelocity;
        }
    }

    public override void End()
    {
        CharacterObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
        Messages.Remove("Up", Up);
        Messages.Remove("VerticalStop", VerticalStop);
        Messages.Remove("Down", Down);
    }
}

class LadderState : CharacterState
{
    public float Velocity;
    Ladder Ladder;

    public LadderState(Character CharacterObject, MessageManager Messages, Action<State> ChangeState) : base(CharacterObject, Messages, ChangeState) { }

    public void Up(object sender)
    {
        Velocity = CharacterObject.LadderVelocity;
        CharacterObject.Animation.LadderGoUp();
    }

    public void Down(object sender)
    {
        Velocity = -CharacterObject.LadderVelocity;
        CharacterObject.Animation.LadderGoDown();
    }

    public new void Stop(object sender)
    {
        Velocity = 0;
        CharacterObject.Animation.LadderIdle();
    }

    public void TouchDown(object sender)
    {
        ChangeState(CharacterObject.StandCharacterState);
    }

    public void Let(object sender)
    {
        ChangeState(CharacterObject.FallCharacterState);
    }

    public override void Start()
    {
        CharacterObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
        Ladder = CharacterObject.GetNearestInteraction().GetComponent<Ladder>();
        Messages.Add("Up", Up);
        Messages.Add("VerticalStop", Stop);
        Messages.Add("Down", Down);
        Messages.Add("TouchDown", TouchDown);
        Messages.Add("Use", Let);
        Messages.Add("Jump", Let);
        //Messages.Add("Go", Let);
        //Messages.Add("TurnBack", Let);
    }

    public override void Update()
    {
        CharacterObject.transform.position += new Vector3(0, Velocity * Time.deltaTime);
        if (!CharacterObject.GetComponent<Collider2D>().IsTouching(Ladder.GetComponent<Collider2D>()))
            ChangeState(CharacterObject.FallCharacterState);
    }

    public override void End()
    {
        CharacterObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
        Velocity = 0;
        Messages.Remove("Up", Up);
        Messages.Remove("VerticalStop", Stop);
        Messages.Remove("Down", Down);
        Messages.Remove("TouchDown", TouchDown);
        Messages.Remove("Use", Let);
        Messages.Remove("Jump", Let);
        //Messages.Remove("Go", Let);
        //Messages.Remove("TurnBack", Let);
    }
}

public class Character : StateManager
{
    public float GoVelocity;
    public float JumpVelocity;
    public float LadderVelocity;
    public MonoBehaviour HangingPoint;
    public BezierCurve ClimbUpPath;
    public float ClimbDuration;
    public float LadderBreakDuration;
    public float InteractRadius;
    public CharacterAnimation Animation;
    public float CrawlVelocityFactor;
    public int ErrorFloorFrames;

    public const float horisAngle = 0.1f;

    internal StandCharacterState StandCharacterState;
    internal FallCharacterState FallCharacterState;
    internal HangingCharacterState HangingCharacterState;
    internal KinematicState KinematicState;
    internal ClimbCharacterState ClimbCharacterState;
    internal LadderBreakState LadderBreakState;
    internal LadderState LadderState;

    int ErrorFloorFrameCounter;

    public MessageManager Messages;

    public Vector2 Forward
    {
        get { return transform.lossyScale.x > 0 ? Vector2.right : Vector2.left; }
    }

    public Collider2D GetNearestInteraction()
    {
        Collider2D[] list = Physics2D.OverlapCircleAll(transform.position, InteractRadius, LayerMask.GetMask("LevelLadder", "LevelInteraction"));
        Collider2D Interaction = null;
        foreach (var item in list)
        {
            var Obstacles = Physics2D.LinecastAll(transform.position, item.bounds.ClosestPoint(transform.position), LayerMask.GetMask("LevelGround"));
            float Cos = Vector2.Dot((item.transform.position - transform.position).normalized, Forward);
            float Distance = Vector2.Distance(item.transform.position, transform.position);
            float MinDistance = Interaction == null ? float.MaxValue : Vector2.Distance(Interaction.transform.position, transform.position);
            if (Obstacles.Length == 0 && Cos > -0.1f && item.GetComponent<IInteractiveObject>()!= null && Distance < MinDistance)
                Interaction = item;
        }
        return Interaction;
    }

    public void UseLadder(Ladder Ladder)
    {
        var a = Ladder.GetComponent<Collider2D>().bounds;
        var b = GetComponent<Collider2D>().bounds;
        var c = a.min.y - b.max.y;
        if (CurrentState is FallCharacterState)
        {
            transform.position = new Vector3(Ladder.transform.position.x, transform.position.y, transform.position.z);
            ChangeState(LadderState);
        }
        else if (Math.Abs(Ladder.GetComponent<Collider2D>().bounds.max.y - GetComponent<Collider2D>().bounds.min.y) < 2)
        {
            transform.position = new Vector3(Ladder.transform.position.x, transform.position.y, transform.position.z);
            ChangeState(LadderBreakState);
        }
        else
        {
            transform.position = new Vector3(Ladder.transform.position.x, transform.position.y + LadderVelocity * 0.1f, transform.position.z);
            ChangeState(LadderState);
        }
    }

    // Use this for initialization
    protected virtual void Start()
    {
        StandCharacterState = new StandCharacterState(this, Messages, ChangeState);
        FallCharacterState = new FallCharacterState(this, Messages, ChangeState);
        HangingCharacterState = new HangingCharacterState(this, Messages, ChangeState);
        KinematicState = new KinematicState(this, Messages, ChangeState);
        ClimbCharacterState = new ClimbCharacterState(this, Messages, ChangeState);
        LadderBreakState = new LadderBreakState(this, Messages, ChangeState);
        LadderState = new LadderState(this, Messages, ChangeState);
        //ChangeState(StandCharacterState);
        ChangeState(FallCharacterState);
        AcceptState();
    }

    void UpdateGround()
    {
        ContactPoint2D[] points = new ContactPoint2D[32];
        ContactFilter2D filter = new ContactFilter2D()
        {
            useLayerMask = true,
            layerMask = LayerMask.GetMask("LevelGround", "LevelLadder")
        };
        int PCount = GetComponent<Collider2D>().GetContacts(points);
        for (int i = 0; i < PCount; i++)
        {
            if (Vector2.Dot(points[i].normal, -Physics2D.gravity.normalized) > horisAngle && Vector2.Distance(points[i].point, transform.position) > transform.lossyScale.y * 0.45f)
            {
                Messages.Trigger("TouchDown", this);
                ErrorFloorFrameCounter = ErrorFloorFrames;
                return;
            }
        }
        if (ErrorFloorFrameCounter > 0)
            ErrorFloorFrameCounter--;
        else
            Messages.Trigger("MissDown", this);
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        UpdateGround();
        CurrentState.Update();
        AcceptState();
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        /*ContactPoint2D[] points = collision.contacts;
        foreach (ContactPoint2D point in points)
        {
            if (point.collider.gameObject.layer == LayerMask.NameToLayer("LevelGround") || 
                point.collider.gameObject.layer == LayerMask.NameToLayer("LevelLadder"))
            {
                if (Vector2.Dot(point.normal, -Physics2D.gravity.normalized) > horisAngle && Vector2.Distance(point.point, transform.position) > transform.lossyScale.y * 0.45f)
                {
                    Messages.Trigger("TouchDown", this);
                    return;
                }
            }
        }
        Messages.Trigger("MissDown", this);*/
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        /*ContactPoint2D[] points = collision.contacts;
        foreach (ContactPoint2D point in points)
        {
            if (point.collider.gameObject.layer == LayerMask.NameToLayer("LevelGround") ||
                point.collider.gameObject.layer == LayerMask.NameToLayer("LevelLadder"))
            {
                if (Vector2.Dot(point.normal, -Physics2D.gravity.normalized) > horisAngle)
                {
                    return;
                }
            }
        }
        Messages.Trigger("MissDown", this);*/
    }
}
