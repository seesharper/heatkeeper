// #r "nuget:System.Diagnostics.Process, 4.3.0"
// using System.Diagnostics;
// using System.Threading;
// using System.Text.RegularExpressions;

// public static class Command2
// {
//     public static CommandResult2 Capture(string commandPath, string arguments, string workingDirectory = null)
//     {
//         var startInformation = CreateProcessStartInfo(commandPath, arguments, workingDirectory, false, false);
//         var process = CreateProcess(startInformation);
//         process.Start();
//         var standardOut = process.StandardOutput.ReadToEnd();
//         var standardError = process.StandardError.ReadToEnd();
//         process.WaitForExit();
//         return new CommandResult2(process.ExitCode, standardOut, standardError);
//     }

//     public static async Task<int> ExecuteAsync(string commandPath, string arguments, string workingDirectory = null, int success = 0)
//     {
//         var startInformation = CreateProcessStartInfo(commandPath, arguments, workingDirectory, false, false);
//         var process = CreateProcess(startInformation);

//         process.OutputDataReceived += (o, a) => WriteStandardOut(a);
//         process.ErrorDataReceived += (o, a) => WriteStandardError(a);

//         void WriteStandardOut(DataReceivedEventArgs args)
//         {
//             if (args.Data != null)
//             {
//                 Out.WriteLine(args.Data);
//             }
//         }

//         void WriteStandardError(DataReceivedEventArgs args)
//         {
//             if (args.Data != null)
//             {
//                 Out.WriteLine(args.Data);
//             }
//         }

//         await StartProcessAsync(process);



//         if (process.ExitCode != success)
//         {
//             throw new InvalidOperationException($"The command {commandPath} {arguments} failed.");
//         }

//         return process.ExitCode;
//     }
//     private static ProcessStartInfo CreateProcessStartInfo(string commandPath, string arguments, string workingDirectory, bool echoStandardOut, bool echoStandardError)
//     {
//         var startInformation = new ProcessStartInfo($"{commandPath}");
//         startInformation.CreateNoWindow = true;
//         startInformation.Arguments = arguments;
//         startInformation.RedirectStandardOutput = !echoStandardOut;
//         startInformation.RedirectStandardError = !echoStandardError;
//         startInformation.UseShellExecute = false;
//         startInformation.WorkingDirectory = workingDirectory ?? Environment.CurrentDirectory;
//         return startInformation;
//     }

//     private static void RunAndWait(Process process)
//     {
//         process.Start();
//         process.WaitForExit();
//     }
//     private static Process CreateProcess(ProcessStartInfo startInformation)
//     {
//         var process = new Process();
//         process.EnableRaisingEvents = true;
//         process.StartInfo = startInformation;
//         return process;
//     }

//     private static Task<int> StartProcessAsync(Process process)
//     {
//         var tcs = new TaskCompletionSource<int>();
//         process.Exited += (o, s) => tcs.SetResult(process.ExitCode);
//         process.Start();
//         process.BeginOutputReadLine();
//         process.BeginErrorReadLine();
//         return tcs.Task;
//     }
// }

// public class CommandResult2
// {
//     public CommandResult2(int exitCode, string standardOut, string standardError)
//     {
//         ExitCode = exitCode;
//         StandardOut = standardOut;
//         StandardError = standardError;
//     }
//     public string StandardOut { get; }
//     public string StandardError { get; }
//     public int ExitCode { get; }

//     public CommandResult2 Dump()
//     {
//         Out.Write(StandardOut);
//         Error.Write(StandardError);
//         return this;
//     }

//     public CommandResult2 EnsureSuccessfulExitCode(int success = 0)
//     {
//         if (ExitCode != success)
//         {
//             throw new InvalidOperationException(StandardError);
//         }
//         return this;
//     }
// }