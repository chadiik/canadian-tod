using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.tod.stream {
	class TODCmd {

		public static string PENUP = "0", PENDOWN = "1";

		public static string Stream(int xSteps, int shoulderSteps, int elbowSteps, bool penup) {
			string cmd = string.Format("q{0}_{1}_{2}_{3}", Format(xSteps), Format(shoulderSteps), Format(elbowSteps), penup ? PENUP : PENDOWN);

			return cmd;
		}

		public static string Stop(bool penup) {
			string cmd = string.Format("q{0}_{1}_{2}_{3}", Format(0), Format(0), Format(0), penup ? PENUP : PENDOWN);

			return cmd;
		}

		public static string Move(int xSteps, int shoulderSteps, int elbowSteps, bool penup) {
			string cmd = string.Format("m{0}_{1}_{2}_{3}", Format(xSteps), Format(shoulderSteps), Format(elbowSteps), penup ? PENUP : PENDOWN);

			return cmd;
		}

		public static string Format(int n, int stringLength = 5) {
			string str = n.ToString();
			if (str.Length > stringLength) str = "";
			while (str.Length < stringLength) {
				str = "0" + str;
			}
			return str;
		}

		public static string Calibrate() {
			return "c";
		}
	}
}
