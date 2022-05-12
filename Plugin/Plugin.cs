using BepInEx;
using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace LordAshes
{
    [BepInPlugin(Guid, Name, Version)]
    [BepInDependency(LordAshes.AssetDataPlugin.Guid)]
    [BepInDependency(LordAshes.FileAccessPlugin.Guid)]
    public partial class RingColorPlugin : BaseUnityPlugin
    {
        // Plugin info
        public const string Name = "Ring Color Plug-In";              
        public const string Guid = "org.lordashes.plugins.ringcolor";
        public const string Version = "1.0.0.0";

        // Configuration
        private static Dictionary<string, string> colors = new Dictionary<string, string>();
        private static List<AssetDataPlugin.DatumChange> backlog = new List<AssetDataPlugin.DatumChange>();
        private static bool diagnosticMode = true;
        private static object padlock = new object();

        void Awake()
        {
            UnityEngine.Debug.Log("Ring Color Plugin: "+this.GetType().AssemblyQualifiedName+" Active.");

            colors.Add(Config.Bind("Colors", "Color 01 Name", "Red").Value, Config.Bind("Colors", "Color 01 Value", "FF0000").Value);
            colors.Add(Config.Bind("Colors", "Color 02 Name", "Green").Value, Config.Bind("Colors", "Color 02 Value", "00FF00").Value);
            colors.Add(Config.Bind("Colors", "Color 03 Name", "Blue").Value, Config.Bind("Colors", "Color 03 Value", "0000FF").Value);
            colors.Add(Config.Bind("Colors", "Color 04 Name", "Yellow").Value, Config.Bind("Colors", "Color 04 Value", "FFFF00").Value);
            colors.Add(Config.Bind("Colors", "Color 05 Name", "Cyan").Value, Config.Bind("Colors", "Color 05 Value", "00FFFF").Value);
            colors.Add(Config.Bind("Colors", "Color 06 Name", "Magenta").Value, Config.Bind("Colors", "Color 06 Value", "FF00FF").Value);
            colors.Add(Config.Bind("Colors", "Color 07 Name", "Orange").Value, Config.Bind("Colors", "Color 07 Value", "FFA500").Value);
            colors.Add(Config.Bind("Colors", "Color 08 Name", "Brown").Value, Config.Bind("Colors", "Color 08 Value", "964B00").Value);
            colors.Add(Config.Bind("Colors", "Color 09 Name", "White").Value, Config.Bind("Colors", "Color 09 Value", "FFFFFF").Value);
            colors.Add(Config.Bind("Colors", "Color 10 Name", "Black").Value, Config.Bind("Colors", "Color 10 Value", "000000").Value);

            diagnosticMode = Config.Bind("Diagnostics", "Show Extra Logs", false).Value;

            RadialUI.RadialSubmenu.EnsureMainMenuItem(RingColorPlugin.Guid, RadialUI.RadialSubmenu.MenuType.character, "Ring Color", FileAccessPlugin.Image.LoadSprite("RingColor.png"));

            foreach (KeyValuePair<string, string> entry in colors)
            {
                if (diagnosticMode) { Debug.Log("Ring Color Plugin: Registering Color '" + entry.Key + "' With Value '" + entry.Value + "'"); }
                if (FileAccessPlugin.File.Exists("Ring" + entry.Key + ".png"))
                {
                    RadialUI.RadialSubmenu.CreateSubMenuItem(RingColorPlugin.Guid, entry.Key, FileAccessPlugin.Image.LoadSprite("Ring"+entry.Key+".png"), (c, o, mmi) => Callback(c, o, mmi, entry.Key), true, null);
                }
                else
                {
                    RadialUI.RadialSubmenu.CreateSubMenuItem(RingColorPlugin.Guid, entry.Key, FileAccessPlugin.Image.LoadSprite("RingColor.png"), (c, o, mmi) => Callback(c, o, mmi, entry.Key), true, null);
                }
            }

            AssetDataPlugin.Subscribe(RingColorPlugin.Guid, (request)=> 
            {
                if (request.action != AssetDataPlugin.ChangeAction.remove)
                {
                    if (diagnosticMode) { Debug.Log("Ring Color Plugin: Checking If Ring Color Request Had Already Been Made"); }
                    foreach (AssetDataPlugin.DatumChange backlogEntry in backlog)
                    {
                        if (diagnosticMode) { Debug.Log("Ring Color Plugin: BackLog Entry = "+backlogEntry.source.ToString()+"|"+backlogEntry.value.ToString()+" vs Request = "+request.source.ToString()+"|"+request.value.ToString()); }
                        if ((backlogEntry.source.ToString()==request.source.ToString()) && (backlogEntry.value.ToString() == request.value.ToString())) 
                        {
                            if (diagnosticMode) { Debug.Log("Ring Color Plugin: Ignoring Duplicate Request"); }
                            return; 
                        }
                    }
                    backlog.Add(request);
                    if (diagnosticMode) { Debug.Log("Ring Color Plugin: Placed Ring Color Request Into Backlog (Backlog=" + backlog.Count + ")"); }
                }
            });

            Utility.PostOnMainPage(this.GetType());
        }

        void Update()
        {
            lock(padlock)
            {
                if (backlog.Count > 0)
                {
                    try
                    {
                        if (diagnosticMode) { Debug.Log("Ring Color Plugin: Trying To Switching To Ring Color '" + backlog.ElementAt(0).value + "' For '" + backlog.ElementAt(0).source.ToString() + "' (Backlog=" + backlog.Count + ")"); }
                        CreatureBoardAsset asset;
                        CreaturePresenter.TryGetAsset(new CreatureGuid(backlog.ElementAt(0).source.ToString()), out asset);
                        Color factionColor;
                        ColorUtility.TryParseHtmlString("#" + backlog.ElementAt(0).value, out factionColor);
                        asset.SetFactionColor(factionColor);
                        asset.Deselect();
                        asset.Select();
                        Debug.Log("Ring Color Plugin: Switched Mini '" + backlog.ElementAt(0).source + "' To Ring Color '" + backlog.ElementAt(0).value + "'");
                        backlog.RemoveAt(0);
                    }
                    catch (Exception)
                    {
                        if (diagnosticMode) { Debug.Log("Ring Color Plugin: Mini '" + backlog.ElementAt(0).source.ToString() + "' Not Yet Ready"); }
                    }
                }
            }
        }

        private void Callback(CreatureGuid cid, string key, MapMenuItem mmi, string selection)
        {
            CreatureBoardAsset asset;
            CreaturePresenter.TryGetAsset(new CreatureGuid(RadialUI.RadialUIPlugin.GetLastRadialTargetCreature()), out asset);
            if (asset != null)
            {
                if (diagnosticMode) { Debug.Log("Ring Color Plugin: Requesting Ring Color '" + selection + "' (" + colors[selection] + ")"); }
                AssetDataPlugin.ClearInfo(cid.ToString(), RingColorPlugin.Guid);
                AssetDataPlugin.SetInfo(cid.ToString(), RingColorPlugin.Guid, colors[selection]);
            }
        }
    }
}
