using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Task1
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            //Паралельное исполнение 00:00:03.5344792
            //Последовательное исполнение 00:00:07.3859013
            Console.WriteLine("Введите эталонное строковое значение:");
            string stringToFind = Console.ReadLine();
            List<string> listFilesWithFindedString = new List<string>();
            ParallelFindFilesWithString(ref stringToFind);
            FindFilesWithString(ref stringToFind, ref listFilesWithFindedString);

            if (listFilesWithFindedString.Count == 0)
            {
                Console.WriteLine("Таких файлов с этим текстом нет");
            }
            else
            {
                ReplaceTextInFiles(ref stringToFind, ref listFilesWithFindedString);
            }
        }
        static void FindFilesWithString(ref string stringToFind, ref List<string> listFilesWithFindedString)
        {
            string Path = @"C:\Users\Admin\Documents\Visual Studio 2015\";
            string[] files = Directory.GetFiles(Path, "*.*", SearchOption.AllDirectories);
            string line;
            Stopwatch timer = new Stopwatch();
            timer.Start();
            for (int i = 0; i < files.Length; i++)
            {
                bool duplicate = false;
                StreamReader fileReader = new StreamReader(files[i]);

                while ((line = fileReader.ReadLine()) != null)
                {
                    if (line == stringToFind)
                    {
                        if (duplicate == false)
                        {
                            listFilesWithFindedString.Add(files[i]);
                            duplicate = true;
                        }

                    }
                }
                fileReader.Close();
            }
            timer.Stop();
            Console.WriteLine("Последовательное исполнение {0}", timer.Elapsed);

        }
        static object locker = new object();

        private static void ParallelFindFilesWithString(ref string stringToFind)
        {
            // string Path = @"C:\Users\Admin\Documents\Visual Studio 2015\Projects\HW_IO 2.09.15\Task2\bin\Debug";
            string Path = @"C:\Users\Admin\Documents\Visual Studio 2015\";
            string[] files = Directory.GetFiles(Path, "*.*", SearchOption.AllDirectories);
            string line;

            List<string> listFilesWithFindedString2 = new List<string>();
            string strToFind = stringToFind;
            Stopwatch Timer = new Stopwatch();
            Timer.Start();
            Parallel.For(0, files.Length, i =>
            {

                bool duplicate = false;
                StreamReader fileReader = new StreamReader(files[i]);

                while ((line = fileReader.ReadLine()) != null)
                {
                    if (line == strToFind)
                    {
                        lock (locker)
                        {
                            if (duplicate == false)
                            {
                                listFilesWithFindedString2.Add(files[i]);
                                duplicate = true;
                            }
                        }
                    }
                }
                fileReader.Close();
            });
            Timer.Stop();
            Console.WriteLine("Паралельное исполнение {0}", Timer.Elapsed);
        }

        static void ReplaceTextInFiles(ref string stringToFind, ref List<string> listFilesWithFindedString)
        {
            Console.WriteLine($"Искомое значение {stringToFind} было найдено в таких файлах:");
            foreach (var lst in listFilesWithFindedString)
            {
                Console.WriteLine(lst);
            }

            Console.WriteLine($"Введите текст на который Вы хотите заменить {stringToFind}:");
            string replaceText = Console.ReadLine();
            foreach (var fileList in listFilesWithFindedString)
            {
                StreamReader fileReader = new StreamReader(fileList);
                string content = fileReader.ReadToEnd();
                fileReader.Close();

                content = Regex.Replace(content, stringToFind, replaceText);

                StreamWriter writer = new StreamWriter(fileList);
                writer.Write(content);
                writer.Close();
                fileReader.Close();
            }

        }
    }
}
