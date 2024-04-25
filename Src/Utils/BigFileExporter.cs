namespace bgf;

/// <summary>
///  解压大文件
/// </summary>
public class BigFileExporter
{
    private static int MB = 1024 * 1024;
    public static byte[] buffer = new byte[MB];

    private BigFile bigFile;
    private BinaryReader reader;

    private string rootPath;
    public bool Init(string path)
    {
        var fs = File.OpenRead(path);
        reader = new BinaryReader(fs);

        var b = reader.ReadByte();
        var g = reader.ReadByte();
        var f = reader.ReadByte();

        if (b != 'b' || g != 'g' || f != 'f')
        {
            Console.WriteLine("非bgf格式文件");
            return false;
        }

        ///文件结构
        var pos = reader.ReadInt64();
        reader.BaseStream.Position = pos;

        bigFile = new BigFile();
        bigFile.Read(reader);
        if (bigFile.dirs.Count == 0)
        {
            return false;
        }

        return true;
    }

    public void Export(string destStr)
    {
        var dest = Utils.FormatPath(destStr);
        if (Directory.Exists(dest)==false)
        {
            Directory.CreateDirectory(dest);
        }
        rootPath = dest;

        if (bigFile.dirs.Count == 0)
        {
            return;
        }

        var rootDir = bigFile.dirs[0];
        Export(rootDir);
    }

    private void Export(DirVO parentDirVO)
    {
        foreach (var idx in parentDirVO.files)
        {
            var fileVO = bigFile.files[idx];
            var filePath = rootPath + "/" + fileVO.path;

            using (FileStream fsWriter = new FileStream(filePath, FileMode.CreateNew))
            {
                reader.BaseStream.Position = fileVO.position;
                var totalCount = 0;
                while (true)
                {
                    //readCount 这个是保存真正读取到的字节数
                    int readCount = reader.Read(buffer, 0, buffer.Length);
                    totalCount += readCount;
                    if (totalCount > fileVO.size)
                    {
                        ///多读了,要回掉多读的部份
                        var passCount = totalCount - fileVO.size;
                        readCount -= passCount;
                    }

                    //开始写入读取到缓存内存中的数据到目标文本文件中
                    fsWriter.Write(buffer, 0, readCount);
                    if (totalCount >= fileVO.size || readCount < MB)
                    {
                        break; //结束循环
                    }
                }
            }
        }

        foreach (var idx in parentDirVO.dirs)
        {
            var dirVO = bigFile.dirs[idx];
            var dir = rootPath + "/" + dirVO.path;
            if (Directory.Exists(dir) == false)
            {
                Directory.CreateDirectory(dir);
            }

            Export(dirVO);
        }
    }
}