namespace mangareader.Component {
	public static class ThumbnailMaker
	{
		public static Panel Create(Image thumb, string thumbName, EventHandler onThumbClick)
		{
			var panel = new Panel
			{
				Width = 170,
				Height = 240,
				Margin = new Padding(8),
				BorderStyle = BorderStyle.FixedSingle,
			};

			panel.Click += onThumbClick;

			var pictureBox = new PictureBox
			{
				Image = thumb,
				SizeMode = PictureBoxSizeMode.Zoom,
				Location = new Point(5, 5),
				Size = new Size(160, 200)
			};
			pictureBox.Click += onThumbClick;

			var label = new Label
			{
				Text = thumbName,
				Location = new Point(5, 210),
				Width = 160,
				Height = 20,
				TextAlign = ContentAlignment.MiddleCenter
			};
			label.Click += onThumbClick;

			panel.Controls.Add(pictureBox);
			panel.Controls.Add(label);
			return panel;
		}
	}
}