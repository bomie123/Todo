using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
