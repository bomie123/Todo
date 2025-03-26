using TodoApp.Models.DataModels;

namespace TodoApp.Helpers
{
    class SettingsHelper
    {
        public static readonly int SettingsId = 1;

        public static SettingsRecord DefaultSettingsRecord = new SettingsRecord()
        {
            Active = true,
            DefaultLighting = DefaultLighting.SystemDefault
        };
        public static SettingsRecord GetSettingsRecord()
        {
            var settings = DatabaseHelper.SelectData<SettingsRecord>(SettingsId);
            settings ??= DefaultSettingsRecord;
            return settings;
        }

        public static void UpdateSettings(SettingsRecord settingsRecord) => DatabaseHelper.UpsertData(settingsRecord);
    }
}
