using System.Collections.Generic;

namespace Dashboard.API.Models
{
    public class Setting
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<SettingField> Fields { get; set; }
        public User ParentUser { get; set; }
        public int ParentUserId { get; set; }

        public Setting(string name)
        {
            Name = name;
        }

        public Setting(string name, ICollection<SettingField> fields)
        {
            Name = name;
            Fields = fields;
        }
    }
}