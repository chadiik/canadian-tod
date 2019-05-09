using Emgu.CV;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.tod.vision {

	interface ISource {
		event ImageEvent SourceUpdated;
		bool IsActive { get; set; }
		int Fps { get; set; }
		int Width { get; }
		int Height { get; }
		void Start();
		void Pause();
		void Stop();
	}
}
