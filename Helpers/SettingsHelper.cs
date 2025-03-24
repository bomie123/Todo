﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TodoApp.Models.DataModels;

namespace TodoApp.Helpers
{
    class SettingsHelper
    {
        public static readonly int SettingsId = 1;

        public static SettingsRecord DefaultSettingsRecord = new SettingsRecord()
        {
            Active = true,
            StartWithDarkMode = false
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
