using System.Diagnostics;

namespace bgf
{
    internal class Program
    {
        static void Main(string[] args)
        {
           

            var writer = new BigFileWriter();
            ///最终 生成的大文件
            var file = "D:/360MoveData/Users/chenronglong/Desktop/bigFile.hpf";
            writer.Init(file);

            ///要打包的目录(不包含目录名,就是底下的文件才会进包)
            var dir = "C:/Users/chenronglong/Pictures/Camera Roll";
            writer.AddDir(dir);

            dir = "C:/Users/chenronglong/Pictures/appleStore";
            writer.AddDir(dir);

            //24G的东西
            dir = "G:/crl";
            writer.AddDir(dir);

            writer.End();


            var exporter = new BigFileExporter();

            exporter.Init(file);

            var dest = "D:/360MoveData/Users/chenronglong/Desktop/test";
            if (Directory.Exists(dest))
            {
                Directory.Delete(dest, true);
            }
            exporter.Export(dest);


            Console.WriteLine("All Complete!");
            Console.ReadLine();
        }
    }
}
