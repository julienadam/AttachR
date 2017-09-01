# AttachR

[![AttachR build status on AppVeyor](https://ci.appveyor.com/api/projects/status/n5avheg2xxyqoldr?svg=true)](https://ci.appveyor.com/project/julienadam/attachr/history)

AttachR is a simple tool that launches executables and automatically attaches them to a running instance of visual studio that is currently open on a specific solution.

***

### Creating a profile

1. First, enter the full path of the .sln file in the main window (or browse for it)
2. Use the `+` button to add a process
  1. Enter the path to the .exe
  2. Optionally specify a working directory
  3. Set the command line arguments
  4. Choose the debugging engine to use in VS, default is Managed 4.5/4.0
  5. Press `OK`
3. Add all the executables you need, use the edit or delete buttons if needed
4. Save the profile (File / Save as...)

### Usage

1. Open the profile with either file / open, or use the recent menu
2. Make sure Visual Studio is running and the corresponding solution is open, or click the open solution button to open it
3. To debug a single process or stop it, use the play / stop buttons next to it
4. To debug multiple processes or stop them, make sure they’re selected (checkbox on the left) and press the play / stop buttons on the bottom
5. There are also global hotkeys for starting / stopping all selected processes. To start / stop press `CTRL-ALT-PLAY` / `CTRL-ALT-STOP` if you have a multimedia keyboard, or use `WIN-F5` / `WIN-SHIFT-F5` if you don’t (these hotkeys are hardcoded for now)

### F.A.Q.

>Attaching doesn't work !

Make sure you are running with the same UAC privileges as the visual instance you want to attach to. Run AttachR as elevated if trying to attach to an elevated VS.

>The stop command actually kills my processes

That is the expected behavior as it mimics what is done by Visual Studio when you stop debugging a program that you started via VS. If you need to shut your application down cleanly, close them manually for now.
