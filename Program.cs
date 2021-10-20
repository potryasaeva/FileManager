using System;
using System.IO;
using System.Configuration;

namespace FileManager
{
    class Program
    {
        static void Main(string[] args)
        {
            int lines = Convert.ToInt32(ConfigurationManager.AppSettings.Get("linesOnPage")); //получаем из конфига количество строк на странице с деревом каталогов

            string workDir = @"C:\";
            string filename = "catalog.txt";
            GetPathTree(workDir, filename);

            var linesInFile = File.ReadAllLines(filename);
            var pages = linesInFile.Length / lines;

            var page = 1; //счет начинаю именно с 1, потому что pages - это целая часть от деления массива на строки. То есть это не массив и там нет 0 элемента.
            PrintSelectedPage(page, pages, lines, linesInFile);

            //Console.TreatControlCAsInput = true;
            ConsoleKey key; 
            do
            {

                key = Console.ReadKey(true).Key;
                if (key == ConsoleKey.RightArrow & page < pages)
                {
                    page ++;
                    PrintSelectedPage(page, pages, lines, linesInFile);
                }
                else if (key == ConsoleKey.LeftArrow & page != 1)
                {
                    page --;
                    PrintSelectedPage(page, pages, lines, linesInFile);
                }
                else
                {
                    PrintSelectedPage(page, pages, lines, linesInFile);
                }

            } while (key != ConsoleKey.Escape);

            static void PrintSelectedPage(int page, int pages, int lines, string[] linesInFile) {
                Console.Clear();

                if (page == 0)
                {
                    Console.WriteLine("Check your code. You use page number 0 somwhere");
                }
                if (page < pages + 1)
                {
                    for (int i = lines * (page - 1); i < lines * page; i++)
                    {
                        Console.WriteLine(linesInFile[i]);
                    }
                    Console.WriteLine("--------------------------------------------------------------------------------");
                }
                else 
                {
                    Console.WriteLine($"Check your code. You use page number more that {pages}");
                }
            }

            static void GetPathTree(string directory, string filename)
            {
                var catalogElement = Directory.GetFileSystemEntries(directory);

                File.WriteAllText(filename, directory); // записываем в файл строку и перенос строки
                File.AppendAllText(filename, Environment.NewLine);

                foreach (var i in catalogElement)
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


            //Вывод информации о выбранном эллементе



            //командная строка



        }
    }
}
