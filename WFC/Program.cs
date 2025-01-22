
using WFC;

var window = new WFCWindow(4096, 4096);
if (Environment.GetEnvironmentVariable("IMAGE_ONLY") == "true")
	window.IsVisible = false;
window.Run();