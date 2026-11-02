// A stand-in for IdeaStatiCa.ConnectionRestApi.exe, used by the ConnectionApiServiceRunner tests to
// reproduce the ways a real service fails to come up. It never opens a listener, so the runner's
// heartbeat never succeeds - which is the point.
//
// The behaviour is chosen by the FAKE_SERVICE_MODE environment variable:
//   exit  (default) - write to both streams and exit with FAKE_SERVICE_EXIT_CODE (default 150)
//   hang            - write to both streams and stay alive without serving anything

string mode = Environment.GetEnvironmentVariable("FAKE_SERVICE_MODE") ?? "exit";

Console.Out.WriteLine("fake service stdout: args = " + string.Join(" ", args));
Console.Error.WriteLine("fake service stderr: refusing to start");
Console.Out.Flush();
Console.Error.Flush();

if (mode == "hang")
{
	Thread.Sleep(Timeout.Infinite);
	return 0;
}

return int.TryParse(Environment.GetEnvironmentVariable("FAKE_SERVICE_EXIT_CODE"), out int code) ? code : 150;
