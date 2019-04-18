using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace simpleIK_sharp
{
    class Program
    {

        static void Main(string[] args)
        {

			if(args.Length > 0) {

				Console.WriteLine("Hello '{0}'!", args[0]);
				Read_File(args[0]);
				string stepsFilepath = args[0] + ".steps";
			}
			else {

				// The code provided will print ‘Hello World’ to the console.
				// Press Ctrl+F5 (or go to Debug > Start Without Debugging) to run your app.

				Console.WriteLine("Hello World!");
				// Console.WriteLine(Directory.GetCurrentDirectory());
				Read_File("job.txt");
				Console.ReadKey();

				// Go to http://aka.ms/dotnet-get-started-console to continue learning how to build a console app! 
			}
		}

        public static void Read_File(string data_folder)
        {
            double l1 = 1365;
            double l2 = 1365;

            double ssteprange = 5522;
            double esteprange = 8033;
            double xsteprange = 3000;
            double xdist = 770;

            double step_angle = 1.8;
            int micro_step = 1;

            double angular_res = step_angle / micro_step;

            double yoffset = 160;
            double xoffset = 540;

            double smin = 33.8;
            double emin = -37.5 - smin;
            double smax = 77.4;
            double emax = 65.6 - smax;
            double t1min = smin * Math.PI / 180;
            double t1max = smax * Math.PI / 180;
            double t2min = emin * Math.PI / 180;
            double t2max = emax * Math.PI / 180;

            double xstp_mm = xsteprange * 1.0 / 770;
            double sstp_rad = ssteprange * 1.0 / (t1max - t1min);
            double estp_rad = esteprange * 1.0 / (t2max - t2min);

            int theta1_size = Convert.ToInt32(Math.Floor((t1max - t1min) / (angular_res * Math.PI / 180)) + 1);
            double[] theta1 = new double[theta1_size];

            int ctr1 = 0;
            for (double theta1_val = t1min; theta1_val < t1max; theta1_val += angular_res * Math.PI / 180) {
                theta1[ctr1++] = theta1_val;
            }

            int theta2_size = Convert.ToInt32(Math.Floor((t2max - t2min) / (angular_res * Math.PI / 180)) + 1);
            double[] theta2 = new double[theta2_size];

            int ctr2 = 0;
            for (double theta2_val = t2min; theta2_val < t2max; theta2_val += angular_res * Math.PI / 180)
            {
                theta2[ctr2++] = theta2_val;
            }

            double[] t1shellmin_add = Enumerable.Repeat(t1max, theta2_size).ToArray();
            double[] t1shellmin = theta1.Concat(t1shellmin_add).ToArray();
            
            double[] t2shellmin_pre = Enumerable.Repeat(t2min, theta1_size).ToArray();
            double[] t2shellmin = t2shellmin_pre.Concat(theta2).ToArray();
        
            double[] t1shellmax_add = Enumerable.Repeat(t1min, theta2_size).ToArray();
            double[] t1shellmax = theta1.Concat(t1shellmax_add).ToArray();
            
            double[] t2shellmax_pre = Enumerable.Repeat(t2max, theta1_size).ToArray();
            double[] t2shellmax = t2shellmax_pre.Concat(theta2).ToArray();

            double[] Xmin = new double[t1shellmin.Length];
            double[] Xmin_ind_vec = new double[t1shellmin.Length];
            double[] Ymin = new double[t1shellmin.Length];
            double[] Ymin_ind_vec = new double[t1shellmin.Length];
            double[] Xmax = new double[t1shellmax.Length];
            double[] Xmax_ind_vec = new double[t1shellmax.Length];
            double[] Ymax = new double[t1shellmax.Length];
            double[] Ymax_ind_vec = new double[t1shellmax.Length];

            for (int i = 0; i < t1shellmin.Length; i++)
            {
                Xmin[i] = l1 * Math.Cos(t1shellmin[i]) + l2 * Math.Cos(t1shellmin[i] + t2shellmin[i]);
                Xmin_ind_vec[i] = i + 1;
                Ymin[i] = l1 * Math.Sin(t1shellmin[i]) + l2 * Math.Sin(t1shellmin[i] + t2shellmin[i]);
                Ymin_ind_vec[i] = i + 1;
                //Console.Write(Xmin[i]);
                //Console.Write(" ");
                //Console.Write(Ymin[i]);
                //Console.Write("\n");
            }

            for (int i = 0; i < t1shellmax.Length; i++)
            {
                Xmax[i] = l1 * Math.Cos(t1shellmax[i]) + l2 * Math.Cos(t1shellmax[i] + t2shellmax[i]);
                Xmax_ind_vec[i] = i + 1;
                Ymax[i] = l1 * Math.Sin(t1shellmax[i]) + l2 * Math.Sin(t1shellmax[i] + t2shellmax[i]);
                Ymax_ind_vec[i] = i + 1;
                //Console.Write(Xmax[i]);
                //Console.Write(" ");
                //Console.Write(Ymax[i]);
                //Console.Write("\n");
            }

            Array.Sort(Ymin.ToArray(), Xmin);
            Array.Sort(Ymax.ToArray(), Xmax);
            Array.Sort(Ymin);
            Array.Sort(Ymax);

            string line;
            int xCoord = -1;
            int yCoord = -1;
            int wrist_status = -1;

            System.IO.StreamReader file = new System.IO.StreamReader(@data_folder);
            while ((line = file.ReadLine()) != null)
            {
                var line_val = line.Split(' ', '\t');
                xCoord = Int32.Parse(line_val[0]);
                int x = xCoord;
                
                yCoord = Int32.Parse(line_val[1]);
                int y = yCoord;

                wrist_status = Int32.Parse(line_val[2]);

                double Xtest = x - xoffset;
                double Ytest = y - yoffset;

                double yminind = 0;
                double ymaxind = 0;

                yminind = int_vector(Ymin, Ymin_ind_vec, Ytest);
                ymaxind = int_vector(Ymax, Ymax_ind_vec, Ytest);

                double xminval = 0;
                double xmaxval = 0;

                xminval = int_vector(Xmin_ind_vec, Xmin, yminind);
                xmaxval = int_vector(Xmax_ind_vec, Xmax, ymaxind);

                double xcenter = (xminval + xmaxval) / 2;
                double xTod = Xtest - xcenter;

                double l3 = Math.Sqrt(Math.Pow(xcenter, 2) + Math.Pow(Ytest, 2));

                double aY = Math.Atan2(Convert.ToDouble(Ytest), xcenter);
                
                double aA = Math.Acos((Math.Pow(l2, 2) - Math.Pow(l1, 2) - Math.Pow(l3, 2)) / (-2.0 * l1 * l3));
                

                double theta1_current = aA + aY;
                double theta1_actual = theta1_current - smin * Math.PI / 180;
               
                double aB = Math.Acos((Math.Pow(l3, 2) - Math.Pow(l1, 2) - Math.Pow(l2, 2)) / (-2 * l1 * l2));

                double theta2_current = aB - Math.PI;
                double theta2_actual = theta2_current - emin * Math.PI / 180;
                
                Int64 xsteps = Convert.ToInt64(Math.Round(xTod * xstp_mm));
                
                Int64 ssteps = Convert.ToInt64(Math.Round(theta1_actual * sstp_rad));
                
                Int64 esteps = Convert.ToInt64(Math.Round(theta2_actual * estp_rad));
                //Console.WriteLine("xcoord: {0}, ycoord: {1}, xtest: {2}, ytest: {3}", xCoord, yCoord, Xtest, Ytest);
                //Console.WriteLine("yminind: {0}, ymaxind: {1}, xminval: {2}, xmaxval: {3}", yminind, ymaxind, xminval, xmaxval);

                // Console.WriteLine("Xsteps: {0}, Ssteps: {1}, Esteps: {2}", xsteps, ssteps, esteps);
                Console.WriteLine("#STEPS {0} {1} {2} {3}", xsteps, ssteps, esteps, wrist_status);
				// Console.ReadLine();

			}

            Console.Write("Done\n");
        }

        static public double linear(double x, double x0, double x1, double y0, double y1)
        {
            if ((x1 - x0) == 0)
            {
                return (y0 + y1) / 2;
            }
            return y0 + (x - x0) * (y1 - y0) / (x1 - x0);
        }

        static public double int_vector(double []X, double []Y, double x)
        {
            //Console.WriteLine("Xval for interp: {0}", x);
            int i = 0;
            for(int j = 1; j < Y.Length; j++)
            {
                if((x - X[j-1])*(x-X[j])<0)
                {
                    i = j;
                    break;
                }
            }
            //Console.WriteLine("Matched index: {0}", i);
            double y = Y[i - 1] + (x - X[i - 1]) * (Y[i] - Y[i - 1]) / (X[i] - X[i - 1]);
            //Console.WriteLine("Yval for interp: {0}", y); 
            return y;
        }

        static public double int_vector2(double[] X, double[] Y, double x)
        {
            Console.WriteLine("Xval for interp: {0}", x);
            /*for (int j = 0; j < Y.Length; j++)
            {
                Console.WriteLine("i: {0}, Xarray: {1}, Yarray: {2}", j, X[j], Y[j]);
            }*/
            int i = Array.FindIndex(X, k => x <= k);
            Console.WriteLine("Matched index: {0}", i);
            double y = Y[i - 1] + (x - X[i - 1]) * (Y[i] - Y[i - 1]) / (X[i] - X[i - 1]);
            Console.WriteLine("Yval for interp: {0}", y);
            return y;
        }
    }
}
;