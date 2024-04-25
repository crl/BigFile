using System.Diagnostics;
using System.IO.Compression;
namespace bgf;
/// <summary>
///  合并相关文件为一个大文件,并建立头文件映射磁盘地址
/// </summary>
public class BigFileWriter
{
    private static int MB = 1024 * 1024;
    public static byte[] buffer = new byte[MB];
    private BigFile bigFile;
    private DirVO root;
    private BinaryWriter writer;
    private string _rootPath;

    private string savePath;
    private string saveTempPath;

    private Stopwatch sw;
    public void Init(string path)
    {
        if (File.Exists(path))
        {
            File.Delete(path);
        }

        sw = new Stopwatch();

        bigFile = new BigFile();

        root = new DirVO();
        root.name = "";
        bigFile.dirs.Add(root);

        var fs = new FileStream(path, FileMode.Append);
        writer = new BinaryWriter(fs);
        writer.BaseStream.Position = 0;
        writer.Write((byte)'b');
        writer.Write((byte)'g');
        writer.Write((byte)'f');

        ///占位符，在position=3的地址;
        writer.Write((long)0);
    }

    public void AddDir(string dirStr)
    {
        sw.Start();
        _rootPath = Utils.FormatPath(dirStr);
        _AddDir(_rootPath, root);
        sw.Stop();

        Console.WriteLine($"花费: {sw.ElapsedMilliseconds} name:{_rootPath}");
    }

    public void Close()
    {
        if (writer != null)
        {
            bigFile.headPos = writer.BaseStream.Position;
            bigFile.WriteHead(writer);

            ///偏移回去
            writer.BaseStream.Position = 3;
            writer.Write(bigFile.headPos);

            writer.Flush();
            writer.Close();

            ///不允许它继续写入 破坏文件结构
            writer = null;
        }
    }

    private string FormatPathHash(string path)
    {
        var shortPath = path.Replace(_rootPath, "");
        shortPath = shortPath.Trim('/');
        return shortPath;
    }

    private void _AddDir(string dirStr, DirVO parentDirVO)
    {
        var files = Directory.GetFiles(dirStr, "*.*", SearchOption.TopDirectoryOnly);

        foreach (var file in files)
        {
            var attributes = File.GetAttributes(file);
            if ((attributes & FileAttributes.Hidden) == FileAttributes.Hidden)
            {
                continue;
            }

            var filePath = Utils.FormatPath(file);
            var fileVO = bigFile.GetFileVO(filePath, out var idx);
            if (fileVO != null)
            {
                Console.WriteLine($"存在同名文件:{filePath}");
                continue;
            }

            fileVO = new FileVO();
            fileVO.name = Path.GetFileName(filePath);

            using (FileStream fsReader = new FileStream(filePath, FileMode.Open))
            {
                fileVO.position = writer.BaseStream.Position;
                while (true)
                {
                    //readCount 这个是保存真正读取到的字节数
                    int readCount = fsReader.Read(buffer, 0, buffer.Length);
                    //开始写入读取到缓存内存中的数据到目标文本文件中
                    writer.Write(buffer, 0, readCount);
                    if (readCount < MB)
                    {
                        break; //结束循环
                    }
                }

                ///实际写入大小
                fileVO.size = (int)(writer.BaseStream.Position - fileVO.position);
            }
            writer.Flush();
            bigFile.files.Add(fileVO);

            idx = bigFile.files.Count - 1;
            parentDirVO.files.Add(idx);
        }

        var dirs = Directory.GetDirectories(dirStr, "*", SearchOption.TopDirectoryOnly);
        foreach (var dir in dirs)
        {
            if (dir.StartsWith("."))
            {
                continue;
            }
            var attributes = File.GetAttributes(dir);
            if ((attributes & FileAttributes.Hidden) == FileAttributes.Hidden)
            {
                continue;
            }

            var dirPath = Utils.FormatPath(dir);

            var dirVO = bigFile.GetDirVO(dirPath, out var idx);
            if (dirVO == null)
            {
                dirVO = new DirVO();
                dirVO.name = Path.GetFileName(dirPath);
                bigFile.dirs.Add(dirVO);
                idx = bigFile.dirs.Count - 1;
            }

            parentDirVO.dirs.Add(idx);

            _AddDir(dirPath, dirVO);
        }
    }
}