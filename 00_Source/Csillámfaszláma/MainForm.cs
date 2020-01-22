/*
 * Created by SharpDevelop.
 * User: adam.katona
 * Date: 2017.06.11.
 * Time: 8:48
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Csillámfaszláma
{
	/// <summary>
	/// Description of MainForm.
	/// </summary>
	public partial class MainForm : Form
	{
		private LLamaController controller;
		
		public MainForm()
		{
			try
			{
				InitializeComponent();
				CheckForIllegalCrossThreadCalls = false;
				controller = new LLamaController();
				controller.Form = this;
				controller.init();
				
				tbLuminance.Value = LLamaController.LUMINANCE;
				tbSpeed.Value = LLamaController.ANGLE_STEP;
				tbShift.Value = LLamaController.ANGLE_SHIFT;
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}
		
		/// <summary>
		/// Set color of a given panel.
		/// </summary>
		/// <param name="stripNo"></param>
		/// <param name="r"></param>
		/// <param name="g"></param>
		/// <param name="b"></param>
		public void setColor(short stripNo, byte[] rgb)
		{
			Panel panel;
			switch (stripNo)
			{
					case 0: panel = panel1; break;
					case 1: panel = panel2; break;
					case 2: panel = panel3; break;
					case 3: panel = panel4; break;
					case 4: panel = panel5; break;
					case 5: panel = panel6; break;
					case 6: panel = panel7; break;
					default: return;
			}
			
			panel.BackColor = Color.FromArgb(255, rgb[0], rgb[1], rgb[2]);
			
			if (stripNo == 0)
			{
				label1.Text = Convert.ToString(rgb[0]);
				label2.Text = Convert.ToString(rgb[1]);
				label3.Text = Convert.ToString(rgb[2]);
			}
		}
		
		/// <summary>
		/// Form is closed.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void MainFormFormClosed(object sender, FormClosedEventArgs e)
		{
			try
			{
				controller.shutdown();
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}

		
		void CheckBox1CheckedChanged(object sender, EventArgs e)
		{
			controller.IsCyclingEnabled = checkBox1.Checked;
		}
		
		void TrackBar1ValueChanged(object sender, EventArgs e)
		{
			controller.valueChanged(0, trackBar1.Value);
			label1.Text = Convert.ToString(trackBar1.Value);
		}

		void TrackBar2ValueChanged(object sender, EventArgs e)
		{
			controller.valueChanged(1, trackBar2.Value);
			label2.Text = Convert.ToString(trackBar2.Value);	
		}
		
		void TrackBar3ValueChanged(object sender, EventArgs e)
		{
			controller.valueChanged(2, trackBar3.Value);
			label3.Text = Convert.ToString(trackBar3.Value);		
		}
				
		void TbSpeedValueChanged(object sender, EventArgs e)
		{
			controller.AngleStep = tbSpeed.Value;
		}
		
		void TbShiftValueChanged(object sender, EventArgs e)
		{
			controller.AngleShift = tbShift.Value;
		}
		
		void TbLuminanceValueChanged(object sender, EventArgs e)
		{
			controller.Luminance = tbLuminance.Value;
		}
	}
}
