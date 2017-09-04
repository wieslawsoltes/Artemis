﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Artemis.DAL;
using Artemis.Managers;
using Artemis.Modules.Abstract;
using Newtonsoft.Json.Linq;
using PcapDotNet.Core;
using PcapDotNet.Packets;

namespace Artemis.Modules.Games.WoW
{
    public class WoWModel : ModuleModel
    {
        private readonly Regex _rgx;
        private PacketCommunicator _communicator;

        public WoWModel(DeviceManager deviceManager, LuaManager luaManager) : base(deviceManager, luaManager)
        {
            Settings = SettingsProvider.Load<WoWSettings>();
            DataModel = new WoWDataModel();
            ProcessNames.Add("Wow-64");

            _rgx = new Regex("(artemis)\\((.*?)\\)", RegexOptions.Compiled);
        }

        public override string Name => "WoW";
        public override bool IsOverlay => false;
        public override bool IsBoundToProcess => true;


        public override void Enable()
        {
            // Start scanning WoW packets
            // Retrieve the device list from the local machine
            IList<LivePacketDevice> allDevices = LivePacketDevice.AllLocalMachine;

            if (allDevices.Count == 0)
            {
                Logger.Warn("No interfaces found! Can't scan WoW packets.");
                return;
            }

            // Take the selected adapter
            PacketDevice selectedDevice = allDevices.First();

            // Open the device
            _communicator = selectedDevice.Open(65536, PacketDeviceOpenAttributes.Promiscuous, 100);
            Logger.Debug("Listening on " + selectedDevice.Description + " for WoW packets");

            // Compile the filter
            using (var filter = _communicator.CreateFilter("tcp"))
            {
                // Set the filter
                _communicator.SetFilter(filter);
            }

            Task.Run(() => ReceivePackets());
            base.Enable();
        }

        private void ReceivePackets()
        {
            // start the capture
            try
            {
                _communicator.ReceivePackets(0, PacketHandler);
            }
            catch (InvalidOperationException)
            {
                // ignored, happens on shutdown
            }
        }

        private void PacketHandler(Packet packet)
        {
            // Ignore duplicates
            if (packet.Ethernet.IpV4.Udp.SourcePort == 3724)
                return;

            var str = Encoding.Default.GetString(packet.Buffer);
            if (str.ToLower().Contains("artemis"))
            {
                var match = _rgx.Match(str);
                if (match.Groups.Count != 3)
                    return;

                Logger.Debug("[{0}] {1}", packet.Ethernet.IpV4.Udp.SourcePort, match.Groups[2].Value);
                // Get the command and argument
                var parts = match.Groups[2].Value.Split('|');
                HandleGameData(parts[0], parts[1]);
            }
        }

        private void HandleGameData(string command, string data)
        {
            var json = JObject.Parse(data);
            var dataModel = (WoWDataModel) DataModel;
            switch (command)
            {
                case "player":
                    ParsePlayer(json, dataModel);
                    break;
                case "target":
                    ParseTarget(json, dataModel);
                    break;
                case "playerState":
                    ParsePlayerState(json, dataModel);
                    break;
            }
        }

        private void ParsePlayer(JObject json, WoWDataModel dataModel)
        {
            dataModel.Player.ApplyJson(json);
        }

        private void ParseTarget(JObject json, WoWDataModel dataModel)
        {
            dataModel.Target.ApplyJson(json);
        }

        private void ParsePlayerState(JObject json, WoWDataModel dataModel)
        {
            dataModel.Player.ApplyStateJson(json);
        }

        public override void Dispose()
        {
            _communicator.Break();
            _communicator.Dispose();
            base.Dispose();
        }

        public override void Update()
        {
        }
    }
}
