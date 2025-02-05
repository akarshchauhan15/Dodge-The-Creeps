using Godot;
using System;

public partial class ConfigController : Node
{
    public static ConfigFile config =  new ConfigFile();
    static string Path = "res://settings.ini";

    public override void _Ready()
    {
        if (!FileAccess.FileExists(Path))
        {
            config.SetValue("Difficulty", "Speed", 2);
            config.SetValue("Difficulty", "Amount", 2);

            config.SetValue("Sound", "Effects", true);
            config.SetValue("Sound", "Feedback", true);

            config.SetValue("Input", "MouseMode", 0);

            config.Save(Path);
        }
        else
            config.Load(Path);
    }
    public static void SaveSettings(String section, String key, Variant t)
    {
        config.SetValue(section, key, t);
        config.Save(Path);
    }
}
