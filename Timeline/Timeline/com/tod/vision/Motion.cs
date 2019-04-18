using Emgu.CV;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.tod.vision {
	class Motion {

		public event ImageEvent MovementDetected;
		private MotionHistory m_History;

		public Motion() {

			m_History = new MotionHistory(
				1.0, //in second, the duration of motion history you wants to keep
				0.05, //in second, maxDelta for cvCalcMotionGradient
				0.5); //in second, minDelta for cvCalcMotionGradient
		}

		public void Process(Mat image) {
			MovementDetected?.Invoke(image);
		}
	}
}
