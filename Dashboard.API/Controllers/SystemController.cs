using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Dashboard.API.DTOs;
using Dashboard.API.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Dashboard.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class SystemController : ControllerBase
    {
        private readonly IConfiguration _config;
        public SystemController(IConfiguration config)
        {
            _config = config;
        }

        [HttpGet]
        [Route("status")]
        public IActionResult GetSystemStatus()
        {
            var server = new ServerStatusDTO();


            try { server.drives = GetDriveStatus(); }
            catch (Exception e) { server.errors += "Drives: " + e.Message + ' '; }

            try { server.vpn = GetVpnStatus(); }
            catch (Exception e) { server.errors += "VPN: " + e.Message + ' '; }

            try { server.processes = GetProcessStats(); }
            catch (Exception e) { server.errors += "Processes: " + e.Message + ' '; }

            try { server.resources = GetServerResources(); }
            catch (Exception e) { server.errors += "Resources: " + e.Message + ' '; }


            return Ok(server);
        }

        private List<ServerProcessDTO> GetProcessStats()
        {
            var processesToQuery = _config.GetSection("AppSettings:ProcessesToQuery").GetChildren();

            List<ServerProcessDTO> processesToReturn = new List<ServerProcessDTO>();

            foreach (var process in processesToQuery)
            {
                string displayName = "";
                bool isRunning = false;

                var processFields = process.GetChildren();
                foreach (var field in processFields)
                {
                    if (field.Key == "displayName") { displayName = field.Value; }
                    if (field.Key == "serviceName")
                    {
                        var command = "powershell \"Get-Process -Name \'" + field.Value + "\' -ErrorAction Ignore | Select -ExpandProperty Id\"";
                        var pids = ScriptRunner.RunCommand(command);
                        isRunning = !String.IsNullOrWhiteSpace(pids);
                    }
                }

                processesToReturn.Add(new ServerProcessDTO { name = displayName, running = isRunning });
            }

            return processesToReturn;
        }

        private ServerResourceDTO GetServerResources()
        {
            // Read result from file (will read previous)
            float ramUsedBytes = 0;
            try
            {
                var ramUsedString = System.IO.File.ReadAllLines("./cache/resources/ramUsed.txt")[0];
                ramUsedBytes = float.Parse(ramUsedString);
            }
            catch (Exception) { }

            float ramAllBytes = 0;
            try
            {
                var ramAllString = ScriptRunner.RunCommand("powershell \"(Get-CimInstance Win32_PhysicalMemory | Measure-Object -Property capacity -Sum).sum\"");
                ramAllBytes = float.Parse(ramAllString);
            }
            catch (Exception) { }

            float cpuUsed = 0;
            try
            {
                var cpuUsedString = System.IO.File.ReadAllLines("./cache/resources/cpuUsed.txt")[0];
                cpuUsed = float.Parse(cpuUsedString);
            }
            catch (Exception) { }

            // Start measurement, save result to file, don't wait for exit
            var sampleSize = _config.GetSection("AppSettings:ResourceQueries:SampleSize").Value;
            var ramQuery = _config.GetSection("AppSettings:ResourceQueries:RAM").Value;
            var cpuQuery = _config.GetSection("AppSettings:ResourceQueries:CPU").Value;

            Task.Run(() =>
            {
                var ramUsedCommand = "Get-Counter -Counter \'" + ramQuery + "\' -SampleInterval 1 -MaxSamples " + sampleSize;
                ramUsedCommand += " | ForEach-Object {$_.CounterSamples[0].CookedValue}";
                ramUsedCommand += " | Measure-Object -Average";
                ramUsedCommand += " | Select -ExpandProperty Average";
                ramUsedCommand = "powershell \"" + ramUsedCommand + "\"";
                var ramCommandResult = ScriptRunner.RunCommand(ramUsedCommand);
                System.IO.File.WriteAllText("./cache/resources/ramUsed.txt", ramCommandResult);
            });

            Task.Run(() =>
            {
                var cpuUsedCommand = "Get-Counter -Counter \'" + cpuQuery + "\' -SampleInterval 1 -MaxSamples " + sampleSize;
                cpuUsedCommand += " | ForEach-Object {$_.CounterSamples[0].CookedValue}";
                cpuUsedCommand += " | Measure-Object -Average";
                cpuUsedCommand += " | Select -ExpandProperty Average";
                cpuUsedCommand = "powershell \"" + cpuUsedCommand + "\"";
                var cpuCommandResult = ScriptRunner.RunCommand(cpuUsedCommand);
                System.IO.File.WriteAllText("./cache/resources/cpuUsed.txt", cpuCommandResult);
            });

            return new ServerResourceDTO
            {
                memoryUsedGb = ramUsedBytes / 1073741824,
                memoryAllGb = ramAllBytes / 1073741824,
                processorUsedPercent = cpuUsed
            };
        }

        private List<DriveDTO> GetDriveStatus()
        {
            // Run command to get drive info
            string commandResult = "";
            try
            {
                commandResult = ScriptRunner.RunCommand("wmic logicaldisk get caption, volumename, size, freespace /format:csv");
            }
            catch (Exception e)
            {
                throw new Exception("Failed to run command. Details: " + e.Message);
            }

            var driveArray = StringDigest.ParseCSVTo2DArray(commandResult, ",", "\r\r\n");

            // Parse info and fill result object
            List<DriveDTO> driveList = new List<DriveDTO>();
            for (int i = 1; i < driveArray.GetLength(0); i++)
            {
                var drive = new DriveDTO();
                drive.letter = driveArray[i, 1];
                drive.name = driveArray[i, 4];

                if (String.IsNullOrWhiteSpace(driveArray[i, 3]) || String.IsNullOrWhiteSpace(driveArray[i, 2]))
                {
                    // Missing drive size, skip
                    continue;
                }

                drive.sizeGbytes = float.Parse(driveArray[i, 3]) / 1073741824;
                drive.freeGbytes = float.Parse(driveArray[i, 2]) / 1073741824;

                driveList.Add(drive);
            }

            return driveList;
        }

        private VpnDTO GetVpnStatus()
        {
            // Run command to get VPN info
            var logLocation = _config.GetSection("AppSettings:VpnLogLocation").Value;
            string commandResult = "";
            try
            {
                commandResult = ScriptRunner.RunCommand("more \"" + logLocation + "\"");
            }
            catch (Exception e)
            {
                throw new Exception("Failed to run command. Details: " + e.Message);
            }

            // Parse update time
            var updatedString = Regex.Match(commandResult, "Updated,(.*)")
                .Groups[1].Value;
            var updatedDetails = Regex.Match(updatedString, "\\w+\\s(\\w+)\\s(\\d+)\\s\\d+:\\d+:\\d+\\s(\\d+)").Groups;
            updatedString = updatedDetails[3].Value + "-" + updatedDetails[1].Value + "-" + updatedDetails[2].Value;

            // Parse client list
            var clientList = new List<VpnClientDTO>();
            float sumTraffic = 0;
            var allRows = commandResult.Split("\r\n");
            var firstClientIndex = Array.FindIndex(allRows, s => s.Contains("Common Name")) + 1;
            var lastClientIndex = Array.FindIndex(allRows, s => s.Contains("ROUTING TABLE"));
            for (int i = firstClientIndex; i < lastClientIndex; i++)
            {
                var newClient = new VpnClientDTO();
                var clientStringArray = allRows[i].Split(',');

                newClient.name = clientStringArray[0];
                newClient.trafficMbytes = (float.Parse(clientStringArray[2]) + float.Parse(clientStringArray[3])) / 1048576;
                sumTraffic += newClient.trafficMbytes;
                var connectedDetails = Regex.Match(clientStringArray[4], "\\w+\\s(\\w+)\\s(\\d+)\\s(\\d+):(\\d+):\\d+\\s(\\d+)").Groups;
                var connectedString = connectedDetails[5].Value + "-" + connectedDetails[1].Value + "-" + connectedDetails[2].Value
                    + " " + connectedDetails[3].Value + ":" + connectedDetails[4].Value;
                newClient.connectedSince = DateTime.Parse(connectedString);

                clientList.Add(newClient);
            }

            // Fill result object
            var vpnInfo = new VpnDTO();
            vpnInfo.updatedTime = DateTime.Parse(updatedString);
            vpnInfo.clientList = clientList;
            vpnInfo.sumTrafficMbytes = sumTraffic;

            return vpnInfo;
        }
    }
}