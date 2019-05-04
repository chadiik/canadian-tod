using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.tod.stream {

	public delegate void Connection();
	public delegate void StreamState();

	public interface Streamer {

		event Connection ConnectionEstablished; 
		event Connection ConnectionFailed;
		event Connection ConnectionClosed;

		event StreamState CalibrationStarted;
		event StreamState CalibrationCompleted;
		event StreamState StreamStarted;
		event StreamState StreamCompleted;
		event StreamState StreamPaused;

		void Open();
		void Pause();
        void Resume();
        void Close();
		void Calibrate();
		void Stream(int xsteps, int ssteps, int esteps, int wrist);
	}
}
