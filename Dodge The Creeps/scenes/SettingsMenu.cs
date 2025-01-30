using Godot;
using System;

public partial class SettingsMenu : Control
{
    public Control buttons;

    public static double[] MobTimerKeys = { 0.8 ,0.66, 0.5, 0.4, 0.31 };
    public static double[] MobSpeedKeys = { 100.0, 150.0, 200.0, 250.0, 300.0 };

    [Export]
    public Timer timer;

    public override void _Ready()
    {
        buttons = GetParent<CanvasLayer>().GetNode<Control>("Buttons");

        GetNode<Panel>("Settings").Show();
        GetNode<Panel>("Controls").Hide();

        HUD.feedback = true;

        GetNode<Button>("Settings/Exit").GrabFocus();
    }

    public void Exit()
    {
        HUD.Click();
        Hide();

        if (GetTree().Paused)
        {
            GetNode<Control>("../PauseMenu").Show();
            GetNode<Label>("../ScoreLabel").Show();
        }

        else
        {
            buttons.Show();
            GetParent<CanvasLayer>().GetNode<Label>("ScoreLabel").Show();
            GetParent<CanvasLayer>().GetNode<Label>("Message").Show();
            GetParent<CanvasLayer>().GetNode<Button>("Buttons/StartButton").GrabFocus();
        }
    }
    public void Controls()
    {
        var panel = GetNode<Panel>("Controls");
        var set = GetNode<Panel>("Settings");
        if (!panel.Visible)
        {
            panel.Visible = true;
            set.Visible = false;
            panel.GetNode<TabContainer>("TabContainer").GetTabBar().GrabFocus();
        }
        else
        {
            panel.Visible = false;
            set.Visible = true;
            GetNode<Button>("Settings/Exit").GrabFocus();
        }
        HUD.Click();
    }
    public void Feedback(bool t)
    {
        HUD.feedback = t;
        HUD.Click();
    }

    public void Music(bool t)
    {
        HUD.playMusic = t;
        HUD.Click();
    }
}
