using com.tod.canvas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.tod {

	class Files {
		public string eyesHC;
		public string facesHC;

		public string video0;

		public string assetsDir;
		public string focusMap;

		public string ikJobsDir;
		public string ikProcess;
	}

	class Time {
		public long startTime = DateTime.Now.Ticks;
		public long ElapsedMillis {
			get { return (long)TimeSpan.FromTicks(DateTime.Now.Ticks - startTime).TotalMilliseconds; }
		}
	}

	class Config {

		public static bool debug = false;
		public static int fps = 30;
		public static Files files = new Files();
		public static Time time = new Time();
		public static Canvas canvas = new Canvas(1000, 1000, 300, 500); // mm
	}
}
