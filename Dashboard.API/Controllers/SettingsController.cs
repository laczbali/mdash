using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Dashboard.API.Data;
using Dashboard.API.DTOs;
using Dashboard.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dashboard.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class SettingsController : ControllerBase
    {
        // TODO: ^ Implenet these features
        // ^ Change password
        // _ Set black&white mode
        // _ Change default tab
        // _ Set kiosk mode
        // _ Implement other settings here?

        private readonly ISettingsRepository _settingsRepo;
        private readonly IMapper _mapper;

        public SettingsController(ISettingsRepository settingsRepo, IMapper mapper)
        {
            _mapper = mapper;
            _settingsRepo = settingsRepo;
        }

        // ************
        // GET settings
        // ************

        [HttpGet]
        [Route("getsetting/{settingName}")]
        public async Task<IActionResult> GetSetting(string settingName)
        {
            string clientUsername = User.FindFirst(ClaimTypes.Name).Value;
            var settingsList = await _settingsRepo.GetSettings(clientUsername);

            try
            {
                var setting = settingsList.Single(x => x.Name.ToLower() == settingName.ToLower());
                return Ok(setting);
            }
            catch (InvalidOperationException)
            {
                return NoContent();
            }
        }

        [HttpGet]
        [Route("get/colors")]
        public async Task<IActionResult> GetColorSettings()
        {
            var clientUsername = User.FindFirst(ClaimTypes.Name).Value;
            var userSettings = await _settingsRepo.GetSettings(clientUsername);

            SettingsForQueryDTO colorSettings;
            try
            {
                colorSettings = userSettings.Single(x => x.Name == "General-Colors");
            }
            catch (InvalidOperationException)
            {
                // Type of DEFAULT means the user uses one of the default values (0, 1, 2 ...)
                // Type of CUSTOM means the value is a custom rgb value ('12,34,54'  'x,y,z'  ...)
                Setting defaultColorSettings = new Setting(
                    name: "General-Colors",
                    fields: new List<SettingField>{
                        new SettingField(
                            name: "UIBase",
                            value: "0",
                            type: "DEFAULT"
                        ),
                        new SettingField(
                            name: "TextBase",
                            value: "0",
                            type: "DEFAULT"
                        ),
                        new SettingField(
                            name: "BackgroundBase",
                            value: "0",
                            type: "DEFAULT"
                        )
                    }
                );

                _settingsRepo.AddSetting(clientUsername, defaultColorSettings);
                colorSettings = _mapper.Map<SettingsForQueryDTO>(defaultColorSettings);
            }

            return Ok(colorSettings);
        }

        [HttpGet]
        [Route("get/background")]
        public async Task<IActionResult> GetBackgroundSettings()
        {
            var clientUsername = User.FindFirst(ClaimTypes.Name).Value;
            var userSettings = await _settingsRepo.GetSettings(clientUsername);

            SettingsForQueryDTO backgroundSettings;
            try
            {
                backgroundSettings = userSettings.Single(x => x.Name == "General-Background");
            }
            catch (InvalidOperationException)
            {
                Setting defaultBackgroundSetting = new Setting(
                    name: "General-Background",
                    fields: new List<SettingField>{
                        new SettingField(
                            name: "ImageURL",
                            value: "https://images.unsplash.com/photo-1561363702-e07252da3399?ixlib=rb-1.2.1&ixid=eyJhcHBfaWQiOjEyMDd9&auto=format&fit=crop&w=1350&q=80",
                            type: "url"
                        ),
                        new SettingField(
                            name: "Gradient",
                            value: "0.6",
                            type: "float"
                        )
                    }
                );

                _settingsRepo.AddSetting(clientUsername, defaultBackgroundSetting);
                backgroundSettings = _mapper.Map<SettingsForQueryDTO>(defaultBackgroundSetting);
            }

            return Ok(backgroundSettings);
        }

        [HttpGet]
        [Route("get/torrentapi")]
        public async Task<IActionResult> GetTorrentApiSettings()
        {
            var clientUsername = User.FindFirst(ClaimTypes.Name).Value;
            var userSettings = await _settingsRepo.GetSettings(clientUsername);

            SettingsForQueryDTO torrentApiSettings;
            try
            {
                torrentApiSettings = userSettings.Single(x => x.Name == "Overview-Torrent");
            }
            catch (InvalidOperationException)
            {
                Setting defaultTorrentApiSetting = new Setting(
                    name: "Overview-Torrent",
                    fields: new List<SettingField>{
                        new SettingField(
                            name: "url",
                            value: "http://CHANGEME:8080"
                        ),
                        new SettingField(
                            name: "username",
                            value: "admin"
                        ),
                        new SettingField(
                            name: "password",
                            value: "admin"
                        )
                    }
                );

                _settingsRepo.AddSetting(clientUsername, defaultTorrentApiSetting);
                torrentApiSettings = _mapper.Map<SettingsForQueryDTO>(defaultTorrentApiSetting);
            }

            return Ok(torrentApiSettings);
        }

        // ************
        // SET settings
        // ************

        [HttpPost]
        [Route("set")]
        public IActionResult SetSettings(SettingsForQueryDTO settings)
        {
            var clientUsername = User.FindFirst(ClaimTypes.Name).Value;

            foreach (SettingfieldForQueryDTO field in settings.Fields)
            {
                SettingField newfield = new SettingField(field.Name, field.Value, field.Type);
                _settingsRepo.ChangeSettingField(clientUsername, settings.Name, field.Name, newfield);
            }

            return Ok();
        }
    }
}