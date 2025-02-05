using Godot;
using Microsoft.VisualBasic;
using System;
using System.Security.AccessControl;
using System.Security.Cryptography.X509Certificates;

public partial class main : Node2D
{
    [Export]
    public PackedScene MobScene { get; set; }

    private int _score;
    public HUD hud;
    public ConfigFile Config;

    [Export]
    public Control Settings { get; set; }

    public double Speed;

    public override void _Ready()
    {
        Input.MouseMode = Input.MouseModeEnum.Visible;
        GetNode<player>("Player").Hide();

        hud = GetNode<HUD>("HUD");
        SetColor();

        Config = ConfigController.config;

        CheckSettings();
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

    }
    public void NewGame()
    {
        CheckSettings();
        _score = 0;

        var player = GetNode<player>("Player");
        var pos = GetNode<Marker2D>("StartPosition");
        player.Start(pos.Position);

        if (player.mouseMode)
            Input.MouseMode = Input.MouseModeEnum.ConfinedHidden;
            Input.WarpMouse(pos.Position);

        GetNode<Timer>("Timer/StartTimer").Start();
        hud.UpdateScore(_score);
        hud.ShowMessage("Get Ready!");
        GetTree().CallGroup("mobs", Node.MethodName.QueueFree);

        HUD.Music("play");
    }

    public void PauseGame()
    {
        if (GetNode<Timer>("Timer/ScoreTimer").TimeLeft > 0)
        {
            GetTree().Paused = true;
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
        Settings.GetNode<HSlider>("Settings/Difficulty/SpeedSlider").Value = (double)ConfigController.config.GetValue("Difficulty", "Speed");
        Settings.GetNode<HSlider>("Settings/Difficulty/AmountSlider").Value = (double)ConfigController.config.GetValue("Difficulty", "Amount");

        GetNode<Timer>("Timer/MobTimer").WaitTime = SettingsMenu.MobTimerKeys[(int)(Config.GetValue("Difficulty", "Amount"))];
        Speed = SettingsMenu.MobSpeedKeys[(int)(Config.GetValue("Difficulty", "Speed"))];

        player.mouseMode = (bool) Config.GetValue("Input", "MouseMode");

        Settings.GetNode<Button>("Settings/Sound/FeedbackButton").ButtonPressed = (bool)Config.GetValue("Sound", "Feedback");
        Settings.GetNode<Button>("Settings/Sound/MusicButton").ButtonPressed = (bool) Config.GetValue("Sound", "Effects");

        Settings.GetNode<TabContainer>("Controls/TabContainer/Mouse/TabContainer").CurrentTab = (int) Config.GetValue("Input", "MouseMode");

        player.mouseMode = Settings.GetNode<TabContainer>("Controls/TabContainer/Mouse/TabContainer").CurrentTab == 0;
    }
    public void UpdateSettings()
    {
        ConfigController.SaveSettings("Difficulty", "Amount", Settings.GetNode<HSlider>("Settings/Difficulty/AmountSlider").Value);
        ConfigController.SaveSettings("Difficulty", "Speed", Settings.GetNode<HSlider>("Settings/Difficulty/SpeedSlider").Value);
        ConfigController.SaveSettings("Input", "MouseMode", Settings.GetNode<TabContainer>("Controls/TabContainer/Mouse/TabContainer").CurrentTab);
    }
}
