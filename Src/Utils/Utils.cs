namespace bgf;

public static class Utils
{
    public static string FormatPath(string path)
    {
        return path.Replace("\\", "/");
    }

    /// <summary>
    /// 查询是否含有一个文件夹
    /// </summary>
    /// <param name="bigFile"></param>
    /// <param name="dirPath"></param>
    /// <param name="idx"></param>
    /// <returns></returns>
    public static DirVO GetDirVO(this BigFile bigFile,string dirPath, out int idx)
    {
        var len = bigFile.dirs.Count;
        for (int i = 0; i < len; i++)
        {
            var vo = bigFile.dirs[i];
            if (vo.name == dirPath)
            {
                idx = i;
                return vo;
            }
        }
        idx = -1;
        return null;
    }

    /// <summary>
    /// 查询是否含有一个文件
    /// </summary>
    /// <param name="bigFile"></param>
    /// <param name="path"></param>
    /// <param name="idx"></param>
    /// <returns></returns>
    public static FileVO GetFileVO(this BigFile bigFile,string path, out int idx)
    {
        var len = bigFile.files.Count;
        for (int i = 0; i < len; i++)
        {
            var vo = bigFile.files[i];
            if (vo.name == path)
            {
                idx = i;
                return vo;
            }
        }

        idx = -1;
        return null;
    }

    /// <summary>
    /// 是否为bigFile印记
    /// </summary>
    /// <param name="first"></param>
    /// <param name="second"></param>
    /// <param name="third"></param>
    /// <returns></returns>
    public static bool IsBigFileSign(byte first, byte second, byte third)
    {
        if (first == 'b' && second == 'g' && third == 'f')
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// 从一个二进制流中读取BigFile;
    /// </summary>
    /// <param name="reader"></param>
    /// <returns></returns>
    public static BigFile GetBigFile(BinaryReader reader)
    {
        var b = reader.ReadByte();
        var g = reader.ReadByte();
        var f = reader.ReadByte();

        if (Utils.IsBigFileSign(b, g, f) == false)
        {
            Console.WriteLine("非bgf格式文件");
            return null;
        }
        ///文件结构
        var pos = reader.ReadInt64();
        reader.BaseStream.Position = pos;

        var bigFile = new BigFile();
        bigFile.Read(reader);
        return bigFile;
    }


    public static bool IsBigFile(string filePath)
    {
        var reader = new BinaryReader(new FileStream(filePath, FileMode.Open));
        var first = reader.ReadByte();
        var second = reader.ReadByte();
        var third = reader.ReadByte();
        reader.Close();

        return IsBigFileSign(first, second, third);
    }
}