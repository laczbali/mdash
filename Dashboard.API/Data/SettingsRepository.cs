using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Dashboard.API.DTOs;
using Dashboard.API.Models;
using Microsoft.EntityFrameworkCore;

namespace Dashboard.API.Data
{
    public class SettingsRepository : ISettingsRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        public SettingsRepository(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async void AddSetting(string username, Setting setting)
        {
            var users = _context.Users;
            var userId = users.FirstOrDefault(u => u.Username == username).Id;

            setting.ParentUserId = userId;
            await _context.Settings.AddAsync(setting);

            foreach (SettingField field in setting.Fields)
            {
                field.ParentSettingId = setting.Id;
                await _context.SettingFields.AddAsync(field);
            }

            _context.SaveChanges();
        }

        public async void AddSettingField(string username, string settingName, SettingField newField)
        {
            var user = await _context.Users
                .Include(x => x.UserSettings)
                .FirstOrDefaultAsync(u => u.Username == username);
            var setting = user.UserSettings.Single(x => x.Name == settingName);

            newField.ParentSettingId = setting.Id;
            await _context.SettingFields.AddAsync(newField);
            _context.SaveChanges();
        }

        public async void ChangeSettingField(string username, string settingName, string fieldName, SettingField newValue)
        {
            var user = await _context.Users
                .Include(x => x.UserSettings)
                    .ThenInclude(x => x.Fields)
                .FirstOrDefaultAsync(u => u.Username == username);
            var settings = user.UserSettings;
            
            var setting = settings.Single(x => x.Name == settingName);
            var field = setting.Fields.Single(x => x.Name == fieldName);

            field.Value = newValue.Value;
            field.Type = newValue.Type;
            _context.SaveChanges();
        }

        public async Task<ICollection<SettingsForQueryDTO>> GetSettings(string username)
        {
            var user = await _context.Users
                .Include(x => x.UserSettings)
                    .ThenInclude(x => x.Fields)
                .FirstOrDefaultAsync(u => u.Username == username);
            var settings = user.UserSettings;

            var settingsToReturn = _mapper.Map<ICollection<SettingsForQueryDTO>>(settings);
            return settingsToReturn;
        }

        public async void RemoveSetting(string username, string settingName)
        {
            var user = await _context.Users
                .Include(x => x.UserSettings)
                .FirstOrDefaultAsync(u => u.Username == username);
            var settings = user.UserSettings;
            var setting = settings.Single(x => x.Name == settingName);

            _context.Remove(setting);
            _context.SaveChanges();
        }

        public async void RemoveSettingField(string username, string settingName, string fieldName)
        {
            var user = await _context.Users
                .Include(x => x.UserSettings)
                    .ThenInclude(x => x.Fields)
                .FirstOrDefaultAsync(u => u.Username == username);
            var settings = user.UserSettings;
            var setting = settings.Single(x => x.Name == settingName);
            var field = setting.Fields.Single(x => x.Name == fieldName);

            _context.Remove(field);
            _context.SaveChanges();
        }
    }
}