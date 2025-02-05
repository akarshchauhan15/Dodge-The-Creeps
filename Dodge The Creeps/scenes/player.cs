using Godot;
using System;

public partial class player : Area2D
{
    [Signal]
    public delegate void HitEventHandler(bool t = true);

    [Export]
    public int Speed { get; set; } = 400;
    public static bool mouseMode = true;
    public bool keyInput = false;

    public Vector2 ScreenSize;
    public Vector2 velocity;
    public Vector2 position;

    public AnimatedSprite2D Sprite;

    public override void _Ready()
    {
        Sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        ScreenSize = GetViewportRect().Size;
        Show();
    }

    public override void _Process(double delta)
    {
        if (!mouseMode && !keyInput)
        {
            Vector2 mousePosition = GetGlobalMousePosition();
            position = mousePosition;

            Position = Position.MoveToward(mousePosition, Speed * (float)delta);

            Animate(mousePosition);
        }
        else if (mouseMode && !keyInput)
        {
            Vector2 motion = GetGlobalMousePosition();
            Animate(motion);
            Position = motion;
            position = motion;
        }

        velocity = Vector2.Zero;

        velocity.Y += Input.GetActionStrength("down") - Input.GetActionStrength("up");
        velocity.X += Input.GetActionStrength("right") - Input.GetActionStrength("left");

        if (velocity != Vector2.Zero)
        {
            if (!keyInput)
            {
                keyInput = true;
                if (!mouseMode)
                    Input.MouseMode = Input.MouseModeEnum.Hidden;
            }
        }

        if (keyInput)
        {
            if (velocity != Vector2.Zero)
            {
                velocity = velocity.Normalized() * Speed;
                Sprite.Play();
            }
            else
            {
                Sprite.Stop();
            }

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

            Position += velocity * (float)delta;
        }

        if (GetGlobalMousePosition() != position)
        {
            if (keyInput)
            {
                keyInput = false;
                Input.WarpMouse(Position);
                if (!mouseMode)
                    Input.MouseMode = Input.MouseModeEnum.Visible;
            }
        }

        Position = new Vector2(
            x: Mathf.Clamp(Position.X, 0, ScreenSize.X),
            y: Mathf.Clamp(Position.Y, 0, ScreenSize.Y)
        );
    }

    public void Animate(Vector2 motion)
    {
        if (Position.Y > motion.Y)
            Sprite.FlipV = false;
        else if (Position.Y < motion.Y)
            Sprite.FlipV = true;

        if (Position.X < motion.X)
            Sprite.FlipH = false;
        else if (Position.X > motion.X)
            Sprite.FlipH = true;

        if (Position != motion)
        {
            double angle = (Math.Abs(Math.Abs(GetAngleTo(motion) * 180 / Math.PI) - 90));

            if (angle <= 40)
                Sprite.Play("up");

            else if (angle >= 50)
                Sprite.Play("walk");
        }

        else
            Sprite.Stop();
    }

    private void OnBodyEntered(Node2D body)
    {
        Delete(true);
    }

    public void ReturnToMenu()
    {
        Delete(false);
        GetNode<Control>("../HUD/PauseMenu").Hide();
        GetTree().Paused = false;
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
    public void WrapMouse()
    {
        if (!keyInput)
        {
            Input.WarpMouse(Position);
        }
    }
}