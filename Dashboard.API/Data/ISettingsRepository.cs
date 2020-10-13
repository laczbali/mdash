using System.Collections.Generic;
using System.Threading.Tasks;
using Dashboard.API.DTOs;
using Dashboard.API.Models;

namespace Dashboard.API.Data
{
    public interface ISettingsRepository
    {
        /// <summary>
        /// Adds a new Setting under 'User.Usersettings'. Only set the 'Name' and optionally the 'Fields'.
        /// </summary>
        /// <param name="username">The username of the User, which it needs to add the Setting to.</param>
        /// <param name="setting">The Setting to add. Only set the 'Name' and optionally the 'Fields'.</param>
        void AddSetting(string username, Setting setting);

        /// <summary>
        /// Add a new SettingField under 'User.Usersettings.Fields'. Only set the 'Name' and optionall the 'Value' and 'Type'.
        /// </summary>
        /// <param name="username">The username of the User, which it needs to add the field to.</param>
        /// <param name="settingName">The Name of the Setting, which it needs to add the field to.</param>
        /// <param name="newField">The field to add. Only set the 'Name' and optionall the 'Value' and 'Type'.</param>
        void AddSettingField(string username, string settingName, SettingField newField);

        /// <summary>
        /// Changes the 'Value' and 'Type' of a SettingField under 'User.Usersettings.Fields'.
        /// </summary>
        /// <param name="username">The username of the User, where the field is.</param>
        /// <param name="settingName">The Name of the setting, where the field is.</param>
        /// <param name="fieldName">The name of the field to change.</param>
        /// <param name="newValue">The new value for the field.</param>
        void ChangeSettingField(string username, string settingName, string fieldName, SettingField newValue);

        /// <summary>
        /// Gets all Settings and Setting fields for a User.
        /// </summary>
        /// <param name="username">The user, whose settings to query.</param>
        /// <returns></returns>
        Task<ICollection<SettingsForQueryDTO>> GetSettings(string username);

        /// <summary>
        /// Removes a Setting and all of its fields.
        /// </summary>
        /// <param name="username">The username, where the setting is.</param>
        /// <param name="settingName">The name of the setting to remove.</param>
        void RemoveSetting(string username, string settingName);

        /// <summary>
        /// Removes a SettingField
        /// </summary>
        /// <param name="username">The username, where the field is</param>
        /// <param name="settingName">The setting, where the field is</param>
        /// <param name="fieldName">The name of the field to remove</param>
        void RemoveSettingField(string username, string settingName, string fieldName);
    }
}