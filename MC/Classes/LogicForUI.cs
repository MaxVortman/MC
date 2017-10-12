using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using MC.Abstract_and_Parent_Classes;
using MC.Classes.Drives;
using MC.Classes.Graphics;
using Microsoft.Win32;
using System.Threading.Tasks;

namespace MC.Classes
{

    internal static class LogicForUi
    {


        public static async Task<string> ReadStatisticAsync(object selectedItem)
        {
            var path = (selectedItem as ListSElement).Path;
            Statistics stat = new NonParallelStatistics(path);
            var result = await stat.GetStatisticsAsync();
            stat = new ParallelStatistics(path);
            result += await stat.GetStatisticsAsync();
            return result;
        }


    }
}


