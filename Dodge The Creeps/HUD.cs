using Godot;
using System;
using System.Xml.Serialization;

public partial class HUD : CanvasLayer
{
    [Signal]
    public delegate void StartGameEventHandler();
    [Signal] 
    public delegate void ChangeColourEventHandler();
    [Signal]
    public delegate void MenuEventHandler();

    public Timer timer;
    public Label message;
    public static AudioStreamPlayer click;
    public static bool feedback = true;
    public static AudioStreamPlayer music;
    public static bool playMusic = true;
    public override void _Ready()
    {
        timer = GetNode<Timer>("MessageTimer");
        message = GetNode<Label>("Message");
        click = GetNode<AudioStreamPlayer>("Sound/ClickSound");
        music = GetNode<AudioStreamPlayer>("Sound/Music");
        GetNode<Button>("Buttons/StartButton").GrabFocus();
    }
    
    public void ShowMessage(string text)
    {
        message.Text = text;
        message.Show();

        timer.Start();
    }

    async public void ShowGameOver(bool t)
    {
        if (t)
        {
            ShowMessage("Game Over!");
            if (playMusic)
                GetNode<AudioStreamPlayer>("Sound/Gameover").Play();
            await ToSignal(timer, Timer.SignalName.Timeout);
        }
        message.Text = "Dodge the Creeps!";
        message.Show();

        await ToSignal(GetTree().CreateTimer(1.0), SceneTreeTimer.SignalName.Timeout);
        Input.MouseMode = Input.MouseModeEnum.Visible;
        GetNode<Control>("Buttons").Show();
        GetNode<Button>("Buttons/StartButton").GrabFocus();
    }

    public void UpdateScore(int score)
    {
        GetNode<Label>("ScoreLabel").Text = score.ToString();
    }
    private void OnStartButtonPressed()
    {
        GetNode<Control>("Buttons").Hide();
        EmitSignal(SignalName.StartGame);
        Click();
    }

    private void OnChangeColourPressed()
    {
        EmitSignal(SignalName.ChangeColour);
    }
    private void OnQuitButtonPressed()
    {
        GetNode<ConfirmationDialog>("QuitDialog").Popup();
        Click();
    }
    private void OnQuitAccepted()
    {
        Click();
        GetTree().Quit();
    }
    private void OnMessageTimerTimeout()
    {
        message.Hide();
    }
    public void ResumeGame()
    {
        GetTree().Paused = false;
        GetNode<Control>("PauseMenu").Hide();
        if (player.mouseMode)
        {
            Input.MouseMode = Input.MouseModeEnum.ConfinedHidden;
        }
        HUD.Music("resume");
    }
    public void SettingsPressed()
    {
        Click();
        GetNode<Control>("Buttons").Hide();
        message.Hide();
        GetNode<Label>("ScoreLabel").Hide();
        GetNode<Control>("PauseMenu").Hide();
        
        GetNode<Control>("SettingsMenu").Show();
        GetNode<Button>("SettingsMenu/Settings/Exit").GrabFocus();
    }

    public static void Click()
    {
        if (feedback)
            click.Play();
    }

    public static void Music(string mode)
    {
        if (playMusic)
        {
            if (mode == "play")
                music.Play();
            else if (mode == "stop")
                music.Stop();
            else if (mode == "pause")
                music.StreamPaused = true;
            else if (mode == "resume")
                music.StreamPaused = false;
        }
    }
}
