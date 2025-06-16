using System.Linq;
using System.Threading.Tasks;
using MHLab.Patch.Core.Client.IO;

namespace MHLab.Patch.Core.Client.Advanced.IO
{
	public class SmartDownloadMetrics : IDownloadMetrics
	{
		private readonly Task[] _tasks;

		public int RunningThreads
		{
			get
			{
				if (_tasks != null)
				{
					return _tasks.Count((Task t) => t != null && t.Status == TaskStatus.Running);
				}
				return 1;
			}
		}

		public SmartDownloadMetrics(Task[] tasks)
		{
			_tasks = tasks;
		}
	}
}
