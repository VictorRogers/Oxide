﻿using System;
using System.IO;
using System.Linq;
using TNet;
using Oxide.Core;
using Oxide.Core.Extensions;
using Oxide.Core.Logging;

namespace Oxide.Game.Nomad
{
    /// <summary>
    /// The extension class that represents this extension
    /// </summary>
    public class NomadExtension : Extension
    {
        /// <summary>
        /// Gets the name of this extension
        /// </summary>
        public override string Name => "Nomad";

        /// <summary>
        /// Gets the version of this extension
        /// </summary>
        public override VersionNumber Version => new VersionNumber(1, 0, 0);

        /// <summary>
        /// Gets the author of this extension
        /// </summary>
        public override string Author => "Oxide Team";

        public override string[] WhitelistAssemblies => new[]
        {
            "mscorlib", "Oxide.Core", "System", "System.Core"
        };
        public override string[] WhitelistNamespaces => new[]
        {
            "System.Collections", "System.Security.Cryptography", "System.Text"
        };

        public static string[] Filter =
        {
        };

        private const string logFileName = "output_log.txt"; // TODO: Add -logFile support
        private TextWriter logWriter;

        /// <summary>
        /// Initializes a new instance of the NomadExtension class
        /// </summary>
        /// <param name="manager"></param>
        public NomadExtension(ExtensionManager manager) : base(manager)
        {
        }

        /// <summary>
        /// Loads this extension
        /// </summary>
        public override void Load()
        {
            // Register our loader
            Manager.RegisterPluginLoader(new NomadPluginLoader());
        }

        /// <summary>
        /// Loads plugin watchers used by this extension
        /// </summary>
        /// <param name="directory"></param>
        public override void LoadPluginWatchers(string directory)
        {
        }

        /// <summary>
        /// Called when all other extensions have been loaded
        /// </summary>
        public override void OnModLoad()
        {
            if (!Interface.Oxide.EnableConsole()) return;

            // TODO: Add console log handling
            if (File.Exists(logFileName)) File.Delete(logFileName);
            var logStream = File.AppendText(logFileName);
            logStream.AutoFlush = true;
            logWriter = TextWriter.Synchronized(logStream);
            Console.SetOut(logWriter);

            Interface.Oxide.ServerConsole.Input += ServerConsoleOnInput;
        }

        internal static void ServerConsole()
        {
            if (Interface.Oxide.ServerConsole == null) return;

            Interface.Oxide.ServerConsole.Title = () => $"{LobbyServerLink.mGameServer.playerCount} | {LobbyServerLink.mGameServer.name}";

            Interface.Oxide.ServerConsole.Status1Left = () => LobbyServerLink.mGameServer.name;
            /*Interface.Oxide.ServerConsole.Status1Right = () =>
            {
                var fps = Main.fpsCount;
                var seconds = TimeSpan.FromSeconds(Main.time);
                var uptime = $"{seconds.TotalHours:00}h{seconds.Minutes:00}m{seconds.Seconds:00}s".TrimStart(' ', 'd', 'h', 'm', 's', '0');
                return string.Concat(fps, "fps, ", uptime);
            };*/

            Interface.Oxide.ServerConsole.Status2Left = () => $"{LobbyServerLink.mGameServer.playerCount}/{LobbyServerLink.mGameServer.playerLimit} players";
            /*Interface.Oxide.ServerConsole.Status2Right = () =>
            {
                var bytesReceived = Utility.FormatBytes(Main.rxData);
                var bytesSent = Utility.FormatBytes(Main.txData);
                return Main.time <= 0 ? "0b/s in, 0b/s out" : string.Concat(bytesReceived, "/s in, ", bytesSent, "/s out");
            };

            Interface.Oxide.ServerConsole.Status3Left = () =>
            {
                var time = DateTime.Today.AddSeconds(Main.mapTime).ToString("h:mm tt").ToLower();
                return string.Concat(" ", time);
            };*/
            Interface.Oxide.ServerConsole.Status3Right = () => $"Oxide {OxideMod.Version} for {NomadCore.CommandLine.GetVariable("clientVersion")}";
            Interface.Oxide.ServerConsole.Status3RightColor = ConsoleColor.Yellow;
        }

        public override void OnShutdown() => logWriter?.Close();

        private static void ServerConsoleOnInput(string input)
        {
            // TODO: Handle console input
        }

        private void HandleLog(string message, string stackTrace, LogType type)
        {
            if (string.IsNullOrEmpty(message) || Filter.Any(message.StartsWith)) return;
            logWriter.WriteLine(message);
            if (!string.IsNullOrEmpty(stackTrace)) logWriter.WriteLine(stackTrace);

            var color = ConsoleColor.Gray;
            if (type == LogType.Warning)
                color = ConsoleColor.Yellow;
            else if (type == LogType.Error)
                color = ConsoleColor.Red;
            Interface.Oxide.ServerConsole.AddMessage(message, color);
        }
    }
}
