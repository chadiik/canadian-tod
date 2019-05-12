using com.tod.canvas;
using Version = com.tod.sketch.Sketch.Version;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Security;

namespace com.tod {

	public class JSON {

		public static string Serialize(object obj) {
			return JsonConvert.SerializeObject(obj);
		}

		public static bool Save(string json, string name) {

			try {
				System.IO.File.WriteAllText(string.Format("config/{0}.json", name), json);
			}
			catch (SecurityException error) {
				Console.WriteLine(
@"Error writing file to {0}
\n\terror:{1}
\n\tcontent:{2}",
				name, error.Message, json);
				return false;
			}

			return true;
		}

		public static bool Save(object obj, string name) {
			Save(Serialize(obj), name);
			return true;
		}

		public static bool Load<T>(string name, out T result) {

			result = default(T);
			string path = string.Format("config/{0}.json", name);
			if (!System.IO.File.Exists(path))
				return false;

			try {
				string json = System.IO.File.ReadAllText(path);
				result = JsonConvert.DeserializeObject<T>(json);
				return true;
			}
			catch (Exception ex) {
				Logger.Instance.ExceptionLog(ex.ToString());
			}

			return false;
		}
	}

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
        public static bool sprayLine = false;
        public static bool stream = false;
        public static int cell = -1;
        public static string wallConfig = "wall";
        public static int xOffset = 0, yOffset;
        public static double xScale = 1, yScale = 1;
        public static Version version = Version.Legacy;

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

							case "version":
								string v = value?.ToLower();
								switch (v) {
									case "legacy":
										version = Version.Legacy;
										break;
									case "zig":
									case "zigzag":
										version = Version.Zigzag;
										break;
									case "hatch":
										version = Version.Hatch;
										break;
								}
								break;

                            case "cell":
                                int cell;
                                if (int.TryParse(value, out cell)) Config.cell = cell;
                                break;

                            case "wallConfig":
                                Config.wallConfig = value;
                                break;

                            case "xOffset":
                                int xOffset;
                                if (int.TryParse(value, out xOffset)) Config.xOffset = xOffset;
                                break;

                            case "yOffset":
                                int yOffset;
                                if (int.TryParse(value, out yOffset)) Config.yOffset = yOffset;
                                break;

                            case "xScale":
                                double xScale;
                                if (double.TryParse(value, out xScale)) Config.xScale = xScale;
                                break;

                            case "yScale":
                                double yScale;
                                if (double.TryParse(value, out yScale)) Config.yScale = yScale;
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
