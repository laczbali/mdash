using System.Collections.Generic;
using Dashboard.API.Models;

namespace Dashboard.API.DTOs
{
    public class SettingsForQueryDTO
    {
        public string Name { get; set; }
        public ICollection<SettingfieldForQueryDTO> Fields { get; set; }
    }
}