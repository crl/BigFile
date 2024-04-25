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
    public void Init(string path)
    {
        var fs = File.OpenRead(path);
        reader = new BinaryReader(fs);
        bigFile = Utils.GetBigFile(reader);
    }

    public void Close()
    {
        if (reader != null)
        {
            reader.Close();
            reader = null;
        }
    }

    public void Export(string destStr)
    {
        var dest = Utils.FormatPath(destStr);
        if (Directory.Exists(dest) == false)
        {
            Directory.CreateDirectory(dest);
        }
        rootPath = dest;

        if (bigFile.dirs.Count == 0)
        {
            return;
        }

        var rootDir = bigFile.dirs[0];
        Export(rootDir, "");
    }

    /// <summary>
    ///  导出(先导出当前文件，当导当前文件夹)
    /// </summary>
    /// <param name="parentDirVO"></param>
    /// <param name="parentPath"></param>
    private void Export(DirVO parentDirVO, string parentPath)
    {
        foreach (var idx in parentDirVO.files)
        {
            var fileVO = bigFile.files[idx];
            var filePath = string.Format("{0}/{1}{2}", rootPath, parentPath, fileVO.name);
            if (File.Exists(filePath) && Utils.IsBigFile(filePath))
            {
                ///todo 合并操作
                var merge = new BigFileMerge();
                merge.Init(filePath);
                merge.Merge(fileVO, reader);
                continue;
            }
            Export(fileVO, filePath);
        }

        foreach (var idx in parentDirVO.dirs)
        {
            var dirVO = bigFile.dirs[idx];
            var dir = string.Format("{0}/{1}{2}", rootPath, parentPath, dirVO.name);
            if (Directory.Exists(dir) == false)
            {
                Directory.CreateDirectory(dir);
            }

            Export(dirVO, parentPath + dirVO.name + "/");
        }
    }

    /// <summary>
    ///  覆盖文件
    /// </summary>
    /// <param name="fileVO"></param>
    /// <param name="filePath"></param>
    protected void Export(FileVO fileVO, string filePath)
    {
        var fsWriter = new FileStream(filePath, FileMode.CreateNew);
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
        fsWriter.Close();
    }
}