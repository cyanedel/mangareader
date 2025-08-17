namespace mangareader.Controls
{
	partial class PreviewControl
	{
		private FlowLayoutPanel flowLayoutPanel;
		private TableLayoutPanel tableLayoutPanel;
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			flowLayoutPanel = new FlowLayoutPanel() {
				AutoScroll = true,
				Dock = DockStyle.Fill,
				WrapContents = true,
				FlowDirection = FlowDirection.LeftToRight,
				Padding = new Padding(10)
			};

			Controls.Add(flowLayoutPanel);
			//SuspendLayout();
			ResumeLayout(false);

		}

		#endregion
	}
}
