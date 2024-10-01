using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace osu_FolderLinker
{
    class Program
    {
        [STAThread]
        static void Main()
        {
            Console.Write("Do you want to link osu! folders (Songs, Replays, etc.)? (y/n): ");
            if (Console.ReadLine()?.ToLower().Trim() == "y")
            {
                string osuFolder = PickFolder();
                if (!string.IsNullOrEmpty(osuFolder) && Directory.Exists(osuFolder) && osuFolder != AppDomain.CurrentDomain.BaseDirectory)
                {
                    Console.WriteLine("Linking starting...");
                    foreach (var folder in new[] { "Songs", "Replays", "Skins", "Screenshots" })
                    {
                        string sourcePath = Path.Combine(osuFolder, folder);
                        string targetPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, folder);
                        if (Directory.Exists(sourcePath))
                        {
                            if (Directory.Exists(targetPath))
                            {
                                Console.Write($"The folder '{folder}' already exists in the current directory. Override? (y/n): ");
                                if (Console.ReadLine()?.ToLower().Trim() != "y")
                                {
                                    Console.WriteLine($"Skipping linking for the '{folder}' folder.");
                                    continue;
                                }
                            }

                            try
                            {
                                Process.Start(new ProcessStartInfo("cmd.exe", $"/C mklink /J \"{targetPath}\" \"{sourcePath}\"") { CreateNoWindow = true, UseShellExecute = false }).WaitForExit();
                                Console.WriteLine($"Linked the {folder} folder!");
                            }
                            catch (Exception ex) { Console.WriteLine($"Error linking {folder}: {ex.Message}"); }
                        }
                        else Console.WriteLine($"Folder '{folder}' does not exist.");
                    }

                    Console.WriteLine("Finished linking folders!");
                    Console.Write("Do you want to link osu!.db and osu!.{0}.cfg? (y/n): ", Environment.UserName);
                    if (Console.ReadLine()?.ToLower().Trim() == "y")
                    {
                        foreach (var file in new[] { $"osu!.{Environment.UserName}.cfg", "osu!.db" })
                        {
                            string source = Path.Combine(osuFolder, file);
                            string destination = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, file);
                            if (File.Exists(source))
                            {
                                try
                                {
                                    File.Copy(source, destination, true);
                                    Console.WriteLine($"Linked {file}!");
                                }
                                catch (Exception ex) { Console.WriteLine($"Failed to link {file}: {ex.Message}"); }
                            }
                            else Console.WriteLine($"File '{file}' does not exist in the osu! folder.");
                        }
                    }
                    Console.WriteLine("Press any key to exit...");
                    Console.ReadKey();
                }
                else
                    Console.WriteLine("Skipping linking. Please ensure you selected a valid osu! folder."); 
            }
        }

        static string PickFolder()
        {
            using (var dialog = new OpenFileDialog { ValidateNames = false, CheckFileExists = false, CheckPathExists = true, FileName = "Select Folder" })
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    string path = Path.GetDirectoryName(dialog.FileName);
                    if (Directory.Exists(path)) return path;
                    MessageBox.Show("Please select a valid folder, not a file.", "Invalid Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                return null;
            }
        }

    }
}
