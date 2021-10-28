using System;
using System.IO;
using System.Configuration;
using System.Collections.Generic;

namespace FileManager
{
    class Program
    {
        static void Main(string[] args)
        {

            int lines = Convert.ToInt32(ConfigurationManager.AppSettings.Get("linesOnPage")); //получаем из конфига количество строк на странице с деревом каталогов.
            
            // отображаем дерево каталогов для диска С, при первом запуске
            string workDir = @"C:\";
            string filename = "catalog.txt";
            var linesInFile = File.ReadAllLines(filename);
            var pages = linesInFile.Length / lines;
            List<string> commands = new ();

            commands.Add(workDir);
            WritePathTreeToTheFile(workDir, filename);
            PrintSelectedPageOfTreeCatalog(1, pages, lines, linesInFile);
            PaginationOfCataloAndCommandHistory(pages, linesInFile, 1, lines);
            ExecuteCommandFromCommandLine(workDir, "catalog1.txt", lines, commands);


            static void PaginationOfCataloAndCommandHistory(int pages, string[] linesInFile, int page, int lines)
            {
                ConsoleKey key;
                do
                {
                    key = Console.ReadKey(true).Key;

                    if (key == ConsoleKey.RightArrow & page < pages)
                    {
                        page++;
                        PrintSelectedPageOfTreeCatalog(page, pages, lines, linesInFile);
                    }
                    else if (key == ConsoleKey.LeftArrow & page != 1)
                    {
                        page--;
                        PrintSelectedPageOfTreeCatalog(page, pages, lines, linesInFile);
                    }
                    else if (key == ConsoleKey.Enter)
                    {
                        break;
                    }
                    else
                    {
                        PrintSelectedPageOfTreeCatalog(page, pages, lines, linesInFile);
                    }

                } while (key != ConsoleKey.Escape);
                if (key == ConsoleKey.Escape)
                {
                    Environment.Exit(0);
                }
            }


            static void ExecuteCommandFromCommandLine(string workDir, string filename, int lines, List<string> commands)
            {
                File.Delete(filename); //удаляем файл если он существует от предыдущих запросов
                do
                {
                    Console.WriteLine("enter command");
                    string command = Console.ReadLine();
                    commands.Add(command);
                    string[] commandLine = command.Split(' ');
                    if (commandLine.Length == 1)
                    {
                        Console.WriteLine("Please enter correct command and Path");
                    }
                    else
                    {
                        switch (commandLine[0].ToLower())
                        {
                            case "cd":
                                try
                                {
                                    WritePathTreeToTheFile(commandLine[1], filename);
                                    var linesInFile1 = File.ReadAllLines(filename);
                                    var pages = linesInFile1.Length / lines;
                                    var page = 1;
                                    PrintSelectedPageOfTreeCatalog(page, pages, lines, linesInFile1);
                                    PaginationOfCataloAndCommandHistory(pages, linesInFile1, page, lines);
                                    ExecuteCommandFromCommandLine(workDir, filename, lines, commands);
                                }
                                catch (Exception)
                                {
                                    continue;
                                }
                                break;
                            case "add":
                                try
                                {
                                    File.Create(commandLine[1]);
                                    Console.WriteLine($"File {commandLine[1]} is created");
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex.Message);
                                    continue;
                                }
                                break;
                            case "rm":
                                try
                                {
                                    File.Delete(commandLine[1]);
                                    Console.WriteLine($"File {commandLine[1]} is deleted");
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex.Message);
                                    continue;
                                }
                                break;
                            case "i":
                                try
                                {
                                    Console.WriteLine($"File attribute - {File.GetAttributes(commandLine[1])}; Creation time - {File.GetCreationTime(commandLine[1])}; Last access time - {File.GetLastAccessTime(commandLine[1])}"); ;
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex.Message);
                                    continue;
                                }
                                break;
                            case "idir":
                                try
                                {
                                    Console.WriteLine($"Directory root - {Directory.GetDirectoryRoot(commandLine[1])}; Creation time - {Directory.GetCreationTime(commandLine[1])}; Last access time - {Directory.GetLastAccessTime(commandLine[1])}"); ;
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex.Message);
                                    continue;
                                }

                                break;
                            case "mkdir":
                                try
                                {
                                    Directory.CreateDirectory(commandLine[1]);
                                    Console.WriteLine($"Directory {commandLine[1]} is created");
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex.Message);
                                    continue;
                                }
                                break;
                            case "rmdir":
                                try
                                {
                                    Directory.Delete(commandLine[1]);
                                    Console.WriteLine($"File {commandLine[1]} is deleted");
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex.Message);
                                    continue;
                                }
                                break;
                            default:
                                Console.WriteLine($"Couldn't fined command {commandLine[0]}. Please check that you use one of this: cd, add, rm, inf, mkdir, rmdir, infdir.");
                                break;
                        }
                    }
                } while (!File.Exists(filename));
            }


            static void PrintSelectedPageOfTreeCatalog(int page, int pages, int lines, string[] linesInFile)
            {
                Console.Clear();
                ConsoleRules();
                if (pages == 0)
                {
                    foreach (var item in linesInFile)
                    {
                        Console.WriteLine(item);
                    }
                }
                else if (page < pages + 1)
                {
                    for (int i = lines * (page - 1); i < lines * page; i++)
                    {
                        Console.WriteLine(linesInFile[i]);
                    }
                }
                else
                {
                    Console.WriteLine($"Check your code. You use page number more that {pages}");
                }
                Console.WriteLine("-----------------------------------------------------");
            }

            static void WritePathTreeToTheFile(string directory, string filename)  //Данный метод создает при необходимости файл и записывает в него дерево подкаталогов и файлов.
            {
                try
                {
                    var catalogElement = Directory.GetFileSystemEntries(directory);  //Получаем список подкатологов и файлов в директории
                    File.WriteAllText(filename, directory);    // записываем в файл строку c введенной директорией и перенос строки
                    File.AppendAllText(filename, Environment.NewLine);


                    foreach (var i in catalogElement) //записываем в файл строки для каждого подкаталога и файла 
                    {
                        try
                        {
                            var elementsInPath = Directory.GetFileSystemEntries(i);
                            File.AppendAllLines(filename, new[] { "  |  ", "  " + i.Replace("C:\\", "|--") });
                            foreach (var j in elementsInPath)
                            {
                                try
                                {
                                    File.AppendAllLines(filename, new[] { "  " + j.Replace((i + "\\"), "|  |--") });
                                }
                                catch (Exception)
                                {
                                    continue;
                                }
                            }
                        }
                        catch (Exception)
                        {
                            continue;
                        }
                    }
                }
                catch (DirectoryNotFoundException ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            static void ConsoleRules()
            {
                Console.WriteLine("------------------------------------------------------------------------");
                Console.WriteLine("|  KEY              |  ACTION                                          |");
                Console.WriteLine("------------------------------------------------------------------------");
                Console.WriteLine("|LEFT/RIGHT ARROW   | listing catalog to the left.                     |");
                Console.WriteLine("|UP/DOWN ARROW      | command history.Works only on active command line|");
                Console.WriteLine("|ENTER              | enter in command line.                           |");
                Console.WriteLine("|ESC                | exit.                                            |");
                Console.WriteLine("------------------------------------------------------------------------");

            }
        }
    }
}
