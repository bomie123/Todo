using TodoApp.Models.DatabaseModels;

namespace TodoApp.Models.DataModels
{
    class SettingsRecord : BaseDataModel
    {
        public DefaultLighting DefaultLighting { get; set; }
    }

    public enum DefaultLighting
    {
        Light,
        Dark,
        SystemDefault
    }
}
