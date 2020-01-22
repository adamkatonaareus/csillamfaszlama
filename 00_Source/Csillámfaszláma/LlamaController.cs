/*
 * Created by SharpDevelop.
 * User: adam.katona
 * Date: 2017.06.11.
 * Time: 8:59
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Threading;
using System.Windows.Forms;
namespace Csillámfaszláma
{
	/// <summary>
	/// The controller
	/// </summary>
	public class LLamaController
	{
		private static readonly int STRIP_COUNT = 7;
		private static readonly short FIRST_CHANNEL_NO = 0;
		public static readonly int LUMINANCE = 255;
		public static readonly int ANGLE_STEP = 5;
		public static readonly int ANGLE_SHIFT = 20;
		//private static readonly int SPEED = 1;
		private static readonly int COMMAND_DELAY = 10;
		
		private static readonly double[] lights =
		{
		  0,   0,   0,   0,   0,   1,   1,   2, 
		  2,   3,   4,   5,   6,   7,   8,   9, 
		 11,  12,  13,  15,  17,  18,  20,  22, 
		 24,  26,  28,  30,  32,  35,  37,  39, 
		 42,  44,  47,  49,  52,  55,  58,  60, 
		 63,  66,  69,  72,  75,  78,  81,  85, 
		 88,  91,  94,  97, 101, 104, 107, 111, 
		114, 117, 121, 124, 127, 131, 134, 137, 
		141, 144, 147, 150, 154, 157, 160, 163, 
		167, 170, 173, 176, 179, 182, 185, 188, 
		191, 194, 197, 200, 202, 205, 208, 210, 
		213, 215, 217, 220, 222, 224, 226, 229, 
		231, 232, 234, 236, 238, 239, 241, 242, 
		244, 245, 246, 248, 249, 250, 251, 251, 
		252, 253, 253, 254, 254, 255, 255, 255, 
		255, 255, 255, 255, 254, 254, 253, 253, 
		252, 251, 251, 250, 249, 248, 246, 245, 
		244, 242, 241, 239, 238, 236, 234, 232, 
		231, 229, 226, 224, 222, 220, 217, 215, 
		213, 210, 208, 205, 202, 200, 197, 194, 
		191, 188, 185, 182, 179, 176, 173, 170, 
		167, 163, 160, 157, 154, 150, 147, 144, 
		141, 137, 134, 131, 127, 124, 121, 117, 
		114, 111, 107, 104, 101,  97,  94,  91, 
		 88,  85,  81,  78,  75,  72,  69,  66, 
		 63,  60,  58,  55,  52,  49,  47,  44, 
		 42,  39,  37,  35,  32,  30,  28,  26, 
		 24,  22,  20,  18,  17,  15,  13,  12, 
		 11,   9,   8,   7,   6,   5,   4,   3, 
		  2,   2,   1,   1,   0,   0,   0,   0, 
		  0,   0,   0,   0,   0,   0,   0,   0, 
		  0,   0,   0,   0,   0,   0,   0,   0, 
		  0,   0,   0,   0,   0,   0,   0,   0, 
		  0,   0,   0,   0,   0,   0,   0,   0, 
		  0,   0,   0,   0,   0,   0,   0,   0, 
		  0,   0,   0,   0,   0,   0,   0,   0, 
		  0,   0,   0,   0,   0,   0,   0,   0, 
		  0,   0,   0,   0,   0,   0,   0,   0, 
		  0,   0,   0,   0,   0,   0,   0,   0, 
		  0,   0,   0,   0,   0,   0,   0,   0, 
		  0,   0,   0,   0,   0,   0,   0,   0, 
		  0,   0,   0,   0,   0,   0,   0,   0, 
		  0,   0,   0,   0,   0,   0,   0,   0, 
		  0,   0,   0,   0,   0,   0,   0,   0, 
		  0,   0,   0,   0,   0,   0,   0,   0
		};
		
		private int angle;
		private Thread thread;
		private uDMX udmx;
	
		
		/// <summary>
		/// The display form.
		/// </summary>
		public MainForm Form
		{
			get; set;
		}

		/// <summary>
		/// Enable/disable cycling
		/// </summary>
		public bool IsCyclingEnabled
		{
			get; set;
		}
		
		/// <summary>
		/// Angle step
		/// </summary>
		public int AngleStep
		{
			get; set;
		}

		/// <summary>
		/// Angle shift
		/// </summary>
		public int AngleShift
		{
			get; set;
		}
		
		public int Luminance
		{
			get; set;
		}
		
		/// <summary>
		/// Init llama.
		/// </summary>
		public void init()
		{
			angle = 0;
			IsCyclingEnabled = true;
			Luminance = LUMINANCE;
			AngleShift = ANGLE_SHIFT;
			AngleStep = ANGLE_STEP;
			
			//--- Init DMX
			udmx = new uDMX();
			
			if (!udmx.IsOpen)
			{
				MessageBox.Show("DMX controller not found!");
				return;
			}
			else
			{
		    	for (short i=0; i<512; i++)
		    	{
			    	udmx.SetSingleChannel (i, 0);
		    	}
			}
			
			//--- Init worker thread
			thread = new Thread(new ThreadStart(this.work));
			thread.Start();
		}
		
		/// <summary>
		/// Shut down llama.
		/// </summary>
		public void shutdown()
		{
			if (thread != null)
			{
				thread.Interrupt();
				thread.Join();
			}
			
			if (udmx != null)
			{
				udmx.Dispose();
			}
		}
		
		/// <summary>
		/// The thread worker process.
		/// </summary>
		public void work()
		{
			try
			{
				while(true)
				{
					if (!IsCyclingEnabled)
					{
						Thread.Sleep(COMMAND_DELAY);
						continue;
					}
					
					for(short i=0; i<STRIP_COUNT; i++)
					{
						setColor(i, angle + (i * AngleShift));
						Thread.Sleep(COMMAND_DELAY);
					}
					
					angle += AngleStep;
					if (angle >= 360)
					{
						angle = 0;
					}
					
					//Thread.Sleep(SPEED);
				}
			}
			catch (ThreadInterruptedException)
			{
				//--- Just exit
			}
			catch (ThreadAbortException)
			{
				//--- Just exit
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.ToString());
			}
		}
		
		private void setColor(short stripNo, int angle)
		{
			if (angle >= 360)
			{
				angle = angle - 360 * (angle/360);
			}
			
			double luminance = (double)Luminance / 255;
			
			byte[] rgb = { 
				(byte)(lights[(angle+120)%360] * luminance),
				(byte)(lights[angle] * luminance),
				(byte)(lights[(angle+240)%360] * luminance) };
			
			Form.setColor(stripNo, rgb);
			
			//BUGBUG: not working udmx.SetChannelRange((short)(FIRST_CHANNEL_NO + stripNo * 3), rgb);
//			if (stripNo == 0)
//			{
//				Console.WriteLine(angle + " " + lights[angle] + " " + rgb[0] + " " + rgb[1] + " " + rgb[2] + " ");
//			}
			
			//FIX KA: Strip color order is: RBG...
			udmx.SetSingleChannel((short)(FIRST_CHANNEL_NO + stripNo * 3), rgb[0]);
			udmx.SetSingleChannel((short)(FIRST_CHANNEL_NO + stripNo * 3 + 2), rgb[1]);
			udmx.SetSingleChannel((short)(FIRST_CHANNEL_NO + stripNo * 3 + 1), rgb[2]);
		}
		
		/// <summary>
		/// Manually changed color values.
		/// </summary>
		/// <param name="channel"></param>
		/// <param name="value"></param>
		public void valueChanged(short channel, int value)
		{
			for(short i=0; i<STRIP_COUNT; i++)
			{
				udmx.SetSingleChannel((short)(FIRST_CHANNEL_NO + i * 3 + channel), (byte)value);
			}
		}
	}
}

