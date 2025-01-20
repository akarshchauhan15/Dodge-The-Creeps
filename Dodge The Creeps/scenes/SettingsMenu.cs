using Godot;
using System;

public partial class SettingsMenu : Control
{
    public Control buttons;

    public static double[] MobTimerKeys = { 0.8 ,0.66, 0.5, 0.4, 0.31 };
    public static double[] MobSpeedKeys = { 100.0, 150.0 ,200.0, 250.0, 300.0};

    public override void _Ready()
    {
        buttons = GetParent<CanvasLayer>().GetNode<Control>("Buttons");
        HUD.feedback = true;
        GetNode<Button>("Exit").GrabFocus();
    }

    public void Exit()
    {
        HUD.Click();
        Hide();
        buttons.Show();
        GetParent<CanvasLayer>().GetNode<Label>("ScoreLabel").Show();
        GetParent<CanvasLayer>().GetNode<Label>("Message").Show();
        GetParent<CanvasLayer>().GetNode<Button>("Buttons/StartButton").GrabFocus();
    }
    public void Controls()
    {
        var panel = GetNode<Panel>("Panel");
        if (!panel.Visible)
        {
            panel.Visible = true;
            //panel.GetNode<Button>("Return").GrabFocus();
            panel.GetNode<TabContainer>("TabContainer").GetTabBar().GrabFocus();
        }
        else
        {
            panel.Visible = false;
            GetNode<Button>("Exit").GrabFocus();
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
