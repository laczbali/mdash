namespace Dashboard.API.Models
{
    public class SettingField
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public string Type { get; set; }
        public Setting ParentSetting { get; set; }
        public int ParentSettingId { get; set; }

        public SettingField(string name)
        {
            Name = name;
        }
        public SettingField(string name, string value)
        {
            Name = name;
            Value = value;
        }

        public SettingField(string name, string value, string type)
        {
            Name = name;
            Value = value;
            Type = type;
        }
    }
}