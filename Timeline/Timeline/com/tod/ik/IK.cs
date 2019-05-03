using com.tod.sketch;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace com.tod.ik {

	public class IK {

		public delegate void ConversionResult(int xsteps, int ssteps, int esteps, int wrist);
		public delegate void ConversionComplete(int jobID);

		private static int s_JobID = 0;

		public event ConversionComplete ConversionCompleted;

		private bool m_Active = false;
        private bool m_Paused = false;

        public bool Paused {
            set {
                m_Paused = value;
            }
        }

		public void Stop() {
			m_Active = false;
		}

		private static bool IsProcessRunning(Process process) {
			try { Process.GetProcessById(process.Id); }
			catch (InvalidOperationException) { return false; }
			catch (ArgumentException) { return false; }
			return true;
		}

		public int Convert(List<TP> path, ConversionResult stepsHandler) {

			int id = s_JobID++;

			string filename = string.Format("job{0}.txt", id);
			string filepath = Config.files.ikJobsDir + filename;

			if (SavePath(path, filepath)) {

				Logger.Instance.WriteLog("Save path({0}) to {1}", path.Count, filepath);

				var proc = new Process {
					StartInfo = new ProcessStartInfo {
						FileName = Config.files.ikProcess,
						Arguments = string.Format("\"{0}\"", filepath),
						UseShellExecute = false,
						RedirectStandardOutput = true,
						CreateNoWindow = true
					}
				};

				m_Active = true;
				(new Thread(() => {

					const string flag = "#STEPS";
					proc.Start();
					while (m_Active && !proc.StandardOutput.EndOfStream) {

                        while (m_Paused) {
                            Thread.Sleep(100);
                        }

						string line = proc.StandardOutput.ReadLine();

						if(line.IndexOf(flag) != -1) {
							string[] steps = line.Split(' ');
							int xsteps, ssteps, esteps, wrist;
							if (steps.Length == 5 && int.TryParse(steps[1], out xsteps) && int.TryParse(steps[2], out ssteps) && int.TryParse(steps[3], out esteps) && int.TryParse(steps[4], out wrist))
								stepsHandler.Invoke(xsteps, ssteps, esteps, wrist);
							else
								Logger.Instance.WriteLog("Could not parse {0} entry", flag);
						}
						else {
							Logger.Instance.WriteLog(line);
						}
					}

					try {
						if(IsProcessRunning(proc))
							proc.Close();
					}
					catch(Exception ex) {
						Logger.Instance.ExceptionLog(ex.ToString());
					}

					ConversionCompleted?.Invoke(id);

				})).Start();
			}

			return id;
		}

		private bool SavePath(List<TP> path, string filepath) {

			int pathLength = path.Count;
			List<int> cleanPath = new List<int>(pathLength * 3);

			int index = 0;
			while (index < pathLength && path[index].IsDown == false) index++;

			Add(cleanPath, path[index].x, path[index].y, false);

			float x = 0, y = 0;
			bool penDown = true;
			for (; index < pathLength; index++) {

				TP p = path[index];
				if (p.IsDown) {
					if(penDown == false) {
						penDown = true;
						Add(cleanPath, p.x, p.y, false);
					}
					Add(cleanPath, x = p.x, y = p.y, true);
				}
				else {
					penDown = false;
				}
			}

			Add(cleanPath, x, y, false);

			try {
				using (StreamWriter fs = new StreamWriter(filepath)) {
					int cleanPathLength = cleanPath.Count;
					for(int i = 0; i < cleanPathLength; i += 3) {
						fs.WriteLine(string.Format("{0} {1} {2}", cleanPath[i], cleanPath[i + 1], cleanPath[i + 2]));
					}
				}
			}
			catch (Exception e) {
				Logger.Instance.ExceptionLog(e.ToString());
				return false;
			}

			return true;
		}

		private static void Add(List<int> data, float x, float y, bool down) {
			data.Add((int)x);
			data.Add((int)y);
			data.Add(down ? 0 : 1);
		}

		private static byte[] GetBytes(int[] values) {
			var result = new byte[values.Length * sizeof(int)];
			Buffer.BlockCopy(values, 0, result, 0, result.Length);
			return result;
		}
	}
}
