using System.Threading.Tasks;
using MC.Source.Entries;
using MC.Source.Statistics;

namespace MC.Source
{

    internal static class LogicForUi
    {


        public static async Task<string> ReadStatisticAsync(object selectedItem)
        {
            var path = (selectedItem as Entity).FullPath;
            Statistics.Statistics stat = new NonParallelStatistics(path);
            var result = await stat.GetStatisticsAsync();
            stat = new ParallelStatistics(path);
            result += await stat.GetStatisticsAsync();
            return result;
        }


    }
}


