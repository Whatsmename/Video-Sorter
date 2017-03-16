using GleamTech.VideoUltimate;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Video_Sorter
{
    public class VideoSorter
    {
        public static object desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        public static List<string> fileType = new List<string>();
        public static List<string> fileList = new List<string>();
        public static bool moveCopyComplete = false;
        public static bool moveCopy;
        public static char driveLetter;
        public static string driveList;
        public static string presetSelect;
        public static string presetCustom;
        public static string currentDrive;
        public static string directoryInput;
        public static Int16 videoResolution;
        public static string placementDirectoryM;
        public static string placementDirectoryS;
        public static string placementDirectoryCustom;
        public static string placementDirectoryDefaultM;
        public static string placementDirectoryDefaultS;
        public static TimeSpan videoDuration;
        public static TimeSpan movieDuration = new TimeSpan(1, 10, 0);
        public static TimeSpan showDuration = new TimeSpan(0, 15, 0);
        public static Queue<string> searchPaths = new Queue<string>();

        static void Main()
        {
            DriveInfo[] drives = DriveInfo.GetDrives();
            Console.WriteLine("These are the drives avalible for scanning");
            foreach (DriveInfo drive in drives)
            {
                Console.WriteLine(drive.Name);
                currentDrive = drive.Name;
                driveList += currentDrive.ToLower() + " ";
            }
            DriveStart:
            Console.Write("Please select the dist to scan for files: ");
            driveLetter = Console.ReadKey().KeyChar;
            Console.WriteLine();
            Console.WriteLine();
            if (driveList.Contains(driveLetter))
            {
                Console.WriteLine("Do you wish to use a preset file type?");
                Console.WriteLine("[P] Preset Video Organisation");
                Console.WriteLine("[C] Custom Video Organisation");
                PresetCustomSelect:
                Console.Write("Please select which you wish to use: [P/C]");
                char presetOrCustom = Console.ReadKey().KeyChar;
                presetCustom = presetOrCustom.ToString();
                Console.WriteLine();
                if (presetCustom == "p")
                {
                    Console.WriteLine("Would you like to sort Movies, Shows or both?");
                    Console.WriteLine("[M] Movies (>70 mins)");
                    Console.WriteLine("[S] TV Shows (<70 mins)");
                    Console.WriteLine("[B] Both Movies and Shows");
                    PresetSelect:
                    Console.Write("Please select which you wish to use: [M/S/B]");
                    char presetSelectChar = Console.ReadKey().KeyChar;
                    presetSelect = presetSelectChar.ToString();
                    Console.WriteLine();
                    if (presetSelect == "m")
                    {
                        placementDirectoryDefaultM = driveLetter + ":\\Sorted Movies\\";
                    }
                    else if (presetSelect == "s")
                    {
                        placementDirectoryDefaultS = driveLetter + ":\\Sorted Shows\\";
                    }
                    else if (presetSelect == "b")
                    {
                        placementDirectoryDefaultM = driveLetter + ":\\Sorted Movies\\";
                        placementDirectoryDefaultS = driveLetter + ":\\Sorted Shows\\";
                    }
                    else
                    {
                        Console.WriteLine("Please select a valid option");
                        goto PresetSelect;
                    }
                    fileType.Add(".mp4");
                    fileType.Add(".flv");
                    fileType.Add(".avi");
                    fileType.Add(".wmv");
                    fileType.Add(".mov");
                    fileType.Add(".m4p");
                    fileType.Add(".mpg");
                    fileType.Add(".mpg2");
                    fileType.Add(".mkv");
                    DirectoryHandler();
                }
                else if (presetCustom == "c")
                {
                    /*Console.WriteLine("What type of file do you want to sort?");
                    TypeSelect:
                    Console.Write("Please write the file type as '.xxx': ");
                    string fileTypeInput = Console.ReadLine().ToLower();
                    if (fileTypeInput.Contains("."))
                    {
                        fileType.Add(fileTypeInput);
                        Console.WriteLine();
                        Console.Write("Do you want to sort by resolution");
                        FileSearch();
                    }
                    else
                    {
                        goto TypeSelect;
                    }*/
                    Console.WriteLine("Please choose a different option, this feature is still a work in progress");
                    goto PresetCustomSelect;
                }
                else if (!presetOrCustom.Equals("p") || !presetOrCustom.Equals("c"))
                {
                    Console.Write("Please enter a valid option: ");
                    goto PresetCustomSelect;
                }
            }
            else if (!driveList.Contains(driveLetter))
            {
                Console.WriteLine("Please enter a valid drive letter");
                goto DriveStart;
            }
            Console.WriteLine("I have transfered all the files I could");
            Console.ReadKey();
        }
        static void DirectoryHandler()
        {
            Console.WriteLine("Where is the directory you want to copy the files to?");
            Console.WriteLine("If left blank a folder will be created at the root of the drive.");
            DirectorySelection:
            directoryInput = Console.ReadLine();
            if (!directoryInput.Contains(":\\"))
            {
                if (presetSelect == "b")
                {
                    if (!Directory.Exists(placementDirectoryDefaultM))
                    {
                        Directory.CreateDirectory(placementDirectoryDefaultM);
                        placementDirectoryM = placementDirectoryDefaultM;
                        if (!Directory.Exists(placementDirectoryDefaultS))
                        {
                            Directory.CreateDirectory(placementDirectoryDefaultS);
                            placementDirectoryS = placementDirectoryDefaultS;
                        }
                    }
                    else if (!Directory.Exists(placementDirectoryDefaultS))
                    {
                        Directory.CreateDirectory(placementDirectoryDefaultS);
                        placementDirectoryS = placementDirectoryDefaultS;
                    }
                    else if (Directory.Exists(placementDirectoryDefaultM) && Directory.Exists(placementDirectoryDefaultS))
                    {
                        placementDirectoryM = placementDirectoryDefaultM;
                        placementDirectoryS = placementDirectoryDefaultS;
                    }
                    VideoHandler();
                }
                else if (presetSelect == "m")
                {
                    if (!Directory.Exists(placementDirectoryDefaultM))
                    {
                        Directory.CreateDirectory(placementDirectoryDefaultM);
                    }
                    placementDirectoryM = placementDirectoryDefaultM;
                    VideoHandler();
                }
                else if (presetSelect == "s")
                {
                    if (!Directory.Exists(placementDirectoryDefaultS))
                    {
                        Directory.CreateDirectory(placementDirectoryDefaultS);
                    }
                    placementDirectoryS = placementDirectoryDefaultS;
                    VideoHandler();
                }
            }
            else if (!Directory.Exists(directoryInput) && directoryInput.Contains(":\\"))
            {
                Directory.CreateDirectory(directoryInput);
                placementDirectoryCustom = directoryInput;
                VideoHandler();
            }
            else
            {
                Console.WriteLine("Please enter a valid path or leave it blank");
                Console.WriteLine();
                goto DirectorySelection;
            }
        }
        static void VideoHandler()
        {
            moveCopyStart:
            Console.WriteLine("Do you wish to Move or Copy these files?");
            string moveOrCopy = Console.ReadLine().ToLower();
            if (moveOrCopy == "m" || moveOrCopy == "move" || moveOrCopy == "c" || moveOrCopy == "copy")
            {
                Console.WriteLine("Searching for and sorting files now...");

                if (moveOrCopy == "move" || moveOrCopy == "m")
                {
                    moveCopy = true;
                }
                else
                {
                    moveCopy = false;
                }
                string searchLocationStart = driveLetter + ":\\";
                searchPaths.Enqueue(searchLocationStart);

                while (searchPaths.Count > 0)
                {
                    var directory = searchPaths.Dequeue();

                    try
                    {
                        fileList = Directory.GetFiles(directory).ToList();

                        foreach (string file in fileList)
                        {
                            foreach (string type in fileType)
                            {
                                moveCopyComplete = false;
                                if (file.Contains(type.ToString()))
                                {
                                    var videoReader = new VideoFrameReader(file);
                                    videoDuration = videoReader.Duration;

                                    /*if (moveCopyComplete == true)
                                    {
                                        break;
                                    }
                                    else*/
                                    if (videoDuration > movieDuration && presetSelect == "m")
                                    {
                                        if (moveCopy == true)
                                        {
                                            if (!File.Exists(placementDirectoryM + Path.GetFileName(file)))
                                            {
                                                Console.WriteLine("Moving {0} to {1}", file, placementDirectoryM);
                                                File.Move(file, placementDirectoryM + Path.GetFileName(file));
                                                moveCopyComplete = true;
                                            }
                                            else if (File.Exists(placementDirectoryM + Path.GetFileName(file)))
                                            {
                                                var existingVideo = new VideoFrameReader(placementDirectoryM + Path.GetFileName(file));
                                                if (existingVideo.Height < videoReader.Height)
                                                {
                                                    Console.WriteLine("Moving {0} to {1}", file, placementDirectoryM);
                                                    File.Delete(placementDirectoryM + Path.GetFileName(file));
                                                    File.Move(file, placementDirectoryM + Path.GetFileName(file));
                                                    moveCopyComplete = true;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (!File.Exists(placementDirectoryM + Path.GetFileName(file)))
                                            {
                                                Console.WriteLine("Moving {0} to {1}", file, placementDirectoryM);
                                                File.Copy(file, placementDirectoryM + Path.GetFileName(file));
                                                moveCopyComplete = true;
                                            }
                                            else if (File.Exists(placementDirectoryM + Path.GetFileName(file)))
                                            {
                                                var existingVideo = new VideoFrameReader(placementDirectoryM + Path.GetFileName(file));
                                                if (existingVideo.Height < videoReader.Height)
                                                {
                                                    Console.WriteLine("Moving {0} to {1}", file, placementDirectoryM);
                                                    File.Delete(placementDirectoryM + Path.GetFileName(file));
                                                    File.Copy(file, placementDirectoryM + Path.GetFileName(file));
                                                    moveCopyComplete = true;
                                                }
                                            }
                                        }
                                    }
                                    else if (videoDuration < movieDuration && videoDuration > showDuration && presetSelect == "s")
                                    {
                                        if (moveCopy == true)
                                        {
                                            if (!File.Exists(placementDirectoryS + Path.GetFileName(file)))
                                            {
                                                Console.WriteLine("Moving {0} to {1}", file, placementDirectoryS);
                                                File.Move(file, placementDirectoryS + Path.GetFileName(file));
                                                moveCopyComplete = true;
                                            }
                                            else if (File.Exists(placementDirectoryM + Path.GetFileName(file)))
                                            {
                                                var existingVideo = new VideoFrameReader(placementDirectoryM + Path.GetFileName(file));
                                                if (existingVideo.Height < videoReader.Height)
                                                {
                                                    Console.WriteLine("Moving {0} to {1}", file, placementDirectoryS);
                                                    File.Delete(placementDirectoryS + Path.GetFileName(file));
                                                    File.Move(file, placementDirectoryS + Path.GetFileName(file));
                                                    moveCopyComplete = true;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (!File.Exists(placementDirectoryS + Path.GetFileName(file)))
                                            {
                                                Console.WriteLine("Copying {0} to {1}", file, placementDirectoryS);
                                                File.Copy(file, placementDirectoryS + Path.GetFileName(file));
                                                moveCopyComplete = true;

                                            }
                                            else if (File.Exists(placementDirectoryM + Path.GetFileName(file)))
                                            {
                                                var existingVideo = new VideoFrameReader(placementDirectoryM + Path.GetFileName(file));
                                                if (existingVideo.Height < videoReader.Height)
                                                {
                                                    Console.WriteLine("Copying {0} to {1}", file, placementDirectoryS);
                                                    File.Delete(placementDirectoryS + Path.GetFileName(file));
                                                    File.Copy(file, placementDirectoryS + Path.GetFileName(file));
                                                    moveCopyComplete = true;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        foreach (var subDirectory in Directory.GetDirectories(directory))
                        {
                            searchPaths.Enqueue(subDirectory);
                        }
                    }
                    catch (System.IO.IOException)
                    {
                        /*Console.WriteLine("IO Exeption encountered");
                        Console.WriteLine("Press any key to continue...");
                        Console.ReadKey();*/
                    }
                    catch (System.UnauthorizedAccessException)
                    {
                        /*Console.WriteLine("Unauthorized Access");
                        Console.WriteLine("Administrator Priveliges required");
                        Console.ReadKey();*/
                    }
                    catch (Exception e)
                    {
                        /*Console.WriteLine("There was a {0}", e);
                        Console.WriteLine("Press any key to continue...");
                        Console.ReadKey();
                        Console.WriteLine("Continuing...");*/
                    }
                }
            }
            else if (moveOrCopy != "m" || moveOrCopy != "move" || moveOrCopy != "c" || moveOrCopy != "copy")
            {
                Console.WriteLine("Please select a valid option");
                goto moveCopyStart;
            }
        }
    }
}


