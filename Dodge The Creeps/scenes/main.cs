using Godot;
using System;
using System.Security.AccessControl;
using System.Security.Cryptography.X509Certificates;

public partial class main : Node2D
{
    [Export]
    public PackedScene MobScene { get; set; }

    private int _score;
    public HUD hud;

    [Export]
    public Control Settings { get; set; }

    public double Speed;

    public override void _Ready()
    {
        Input.MouseMode = Input.MouseModeEnum.Visible;
        GetNode<player>("Player").Hide();
        hud = GetNode<HUD>("HUD");
        SetColor();
    }

    public void SetColor()
    {                                                                                                              
        var rect = GetNode<ColorRect>("Background");                                                                                        
        rect.Color = Color.Color8(((byte)GD.RandRange(45, 160)), ((byte)GD.RandRange(45, 160)), ((byte)GD.RandRange(45, 160)));
    }
    public void GameOver(bool t)
    {
        GetNode<Timer>("Timer/MobTimer").Stop();
        GetNode<Timer>("Timer/ScoreTimer").Stop();

        hud.ShowGameOver(t);
        HUD.Music("stop");
        if (HUD.playMusic)
            hud.GetNode<AudioStreamPlayer>("Sound/Gameover").Play();
    }
    public void NewGame()
    {
        _score = 0;

        var player = GetNode<player>("Player");
        var pos = GetNode<Marker2D>("StartPosition");
        player.Start(pos.Position);

        Input.MouseMode = Input.MouseModeEnum.Captured;

        GetNode<Timer>("Timer/StartTimer").Start();
        hud.UpdateScore(_score);
        hud.ShowMessage("Get Ready!");
        GetTree().CallGroup("mobs", Node.MethodName.QueueFree);

        HUD.Music("play");
        CheckSettings();
    }

    public void PauseGame()
    {
        if (GetNode<Timer>("Timer/ScoreTimer").TimeLeft > 0 && Engine.TimeScale != 0)
        {
            Engine.TimeScale = 0;
            GetNode<Control>("HUD/PauseMenu").Show();
            GetNode<Button>("HUD/PauseMenu/ResumeButton").GrabFocus();
            Input.MouseMode = Input.MouseModeEnum.Visible;
            HUD.Music("pause");
        }
    }

    private void ScoreTimerTimeout()
    {
        _score++;
        hud.UpdateScore(_score);
    }

    private void StartTimerTimeout()
    {
        GetNode<Timer>("Timer/MobTimer").Start();
        GetNode<Timer>("Timer/ScoreTimer").Start();
    }

    private void MobTimerTimeout()
    {
        Mob mob = MobScene.Instantiate<Mob>();

        var location = GetNode<PathFollow2D>("MobPath/MobSpawnLocation");
        location.ProgressRatio = GD.Randf();

        float direction = location.Rotation + Mathf.Pi / 2;
        mob.Position = location.Position;

        direction += (float)GD.RandRange(-Mathf.Pi / 4, Mathf.Pi / 4);
        mob.Rotation = direction;

        var velocity = new Vector2((float)GD.RandRange(Speed - 30, Speed + 30), 0);
        mob.LinearVelocity = velocity.Rotated(direction);

        AddChild(mob);
    }

    public void CheckSettings()
    {
        GetNode<Timer>("Timer/MobTimer").WaitTime = SettingsMenu.MobTimerKeys[(int)Settings.GetNode<HSlider>("Difficulty/AmountSlider").Value];
        Speed = SettingsMenu.MobSpeedKeys[(int)Settings.GetNode<HSlider>("Difficulty/SpeedSlider").Value];
    }
}
