using Godot;
using System;

public partial class player : Area2D
{
    [Signal]
    public delegate void HitEventHandler(bool t = true);
    [Signal]
    public delegate void PauseEventHandler();

    [Export]
    public int Speed { get; set; } = 400;

    public Vector2 ScreenSize;

    public override void _Ready()
    {
        ScreenSize = GetViewportRect().Size;
        Show();
    }

    public override void _Process(double delta)
    {
        var velocity = Vector2.Zero;

        velocity.Y += Input.GetActionStrength("down") - Input.GetActionStrength("up");
        velocity.X += Input.GetActionStrength("right") - Input.GetActionStrength("left");

        var Sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");

        if (velocity.Length() > 0)
        {
            velocity = velocity.Normalized() * Speed;
            Sprite.Play();
        }
        else
        {
            Sprite.Stop();
        }

        if (Engine.TimeScale != 0)
        {
            if (velocity.X != 0)
            {
                Sprite.Animation = "walk";
                Sprite.FlipH = velocity.X < 0;
            }
            else if (velocity.Y != 0)
            {
                Sprite.Animation = "up";
                Sprite.FlipV = velocity.Y > 0;
            }
        }

        Position += velocity * (float)delta;
        Position = new Vector2(
            x: Mathf.Clamp(Position.X, 0, ScreenSize.X),
            y: Mathf.Clamp(Position.Y, 0, ScreenSize.Y)
        );

        if (Input.IsActionJustPressed("pause"))
        {
            EmitSignal(SignalName.Pause);
        }
    }

    private void OnBodyEntered(Node2D body)
    {
        Delete(true);
    }

    public void ReturnToMenu()
    {
        Delete(false);
        HUD.Click();
        HUD.Music("Stop");
    }

    public void Delete(bool t)
    {
        Hide();
        EmitSignal(SignalName.Hit, t);
        GetNode<CollisionShape2D>("CollisionShape2D").SetDeferred(CollisionShape2D.PropertyName.Disabled, true);
    }
    
    public void Start(Vector2 position)
    {
        Position = position;
        Show();
        GetNode<CollisionShape2D>("CollisionShape2D").Disabled = false;
    }
}