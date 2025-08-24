using System.Diagnostics;
using System.Reflection;

namespace DynamicWallpaperWin
{
	partial class AboutForm : Form
	{
		Assembly assembly = Assembly.GetExecutingAssembly();

		public AboutForm()
		{
			InitializeComponent();
			labelAppName.Text = AssemblyProduct;
			labelAppVersion.Text = AssemblyVersion;
			labelCopyright.Text = AssemblyCopyright;
			linkLabelGH.Text = "Project GitHub page";

			linkLabelGH.LinkClicked += (sender, e) =>
			{
				Process.Start(new ProcessStartInfo
				{
					FileName = "https://github.com/mlogic1/dynamic-wallpaper-win",
					UseShellExecute = true
				});
			};

			linkLabelGH.Links.Add(0, linkLabelGH.Text.Length, "https://github.com/mlogic1/dynamic-wallpaper-win");
		}

		#region Assembly Attribute Accessors

		private string AssemblyTitle
		{
			get
			{
				string? value = assembly.GetCustomAttribute<AssemblyTitleAttribute>()?.Title;
				return (value == null) ? "" : value!;
			}
		}

		private string AssemblyProduct
		{
			get
			{
				string? value = assembly.GetCustomAttribute<AssemblyProductAttribute>()?.Product;
				return (value == null) ? "" : value!;
			}
		}

		private string AssemblyVersion
		{
			get
			{
				string? value = assembly.GetName().Version?.ToString();
				return (value == null) ? "" : value!;
			}
		}

		private string AssemblyCopyright
		{
			get
			{
				string? value = assembly.GetCustomAttribute<AssemblyCopyrightAttribute>()?.Copyright;
				return (value == null) ? "" : value!;
			}
		}
		#endregion

		private void buttonOK_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		private void labelAppName_Click(object sender, EventArgs e)
		{

		}

		private void AboutForm_Load(object sender, EventArgs e)
		{

		}
	}
}
