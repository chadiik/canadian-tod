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
        public static bool sprayLine = false;
        public static bool stream = false;

		static Config() {
			Load();
		}

		public static void Load() {
			try {
				System.IO.StreamReader file = new System.IO.StreamReader("config/config.txt");
				string content = file.ReadToEnd();
				string[] entries = content.Split('\n', '\r');
				foreach(string entry in entries) {
					string[] key_value = entry.Split('=');
					if(key_value.Length == 2) {
						string key = key_value[0];
						string value = key_value[1];

						switch (key) {
							case "debug":
								bool debug;
								if (bool.TryParse(value, out debug)) Config.debug = debug;
								break;

                            case "sample":
                                files.video0 = "config/" + value;
                                break;

                            case "linestyle":
                                sprayLine = value == "spray";
                                break;

                            case "stream":
                                bool stream;
                                if (bool.TryParse(value, out stream)) Config.stream = stream;
                                break;

                        }
					}
				}
			}
			catch(Exception ex) {
				Logger.Instance.ExceptionLog(ex.ToString());
			}
		}
	}
}
