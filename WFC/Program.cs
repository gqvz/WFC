
using WFC;

var window = new WFCWindow(1024, 1024);
if (Environment.GetEnvironmentVariable("IMAGE_ONLY") == "true")
	window.IsVisible = false;
window.Run();