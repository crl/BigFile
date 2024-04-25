namespace bgf;

public class BigFile
{
    /// <summary>
    /// 只要读出0, 递归它的子目录及子文件 就可以创建出目录结构(所以0为根目录)
    /// </summary>
    public List<DirVO> dirs = new List<DirVO>();
    /// <summary>
    /// 包含的所有文件
    /// </summary>
    public List<FileVO> files = new List<FileVO>();

    /// <summary>
    /// 单纯名字列表
    /// </summary>
    public List<string> names = new List<string>();

    public int GetNamesIndex(string name)
    {
        var index = names.IndexOf(name);
        if (index == -1)
        {
            names.Add(name);
            index = names.Count - 1;
        }

        return index;
    }

    public string GetNameBy(int index)
    {
        if (index == -1)
        {
            return string.Empty;
        }
        return names[index];
    }

    public void Write(BinaryWriter writer)
    {
        ///string列表存储空间
        writer.Write((int)0);
        var pos = writer.BaseStream.Position;
        var count = dirs.Count;
        writer.Write7BitEncodedInt(count);
        for (int i = 0; i < count; i++)
        {
            dirs[i].Write(writer,this);
        }
        count = files.Count;
        writer.Write7BitEncodedInt(count);
        for (int i = 0; i < count; i++)
        {
            files[i].Write(writer, this);
        }

        var namesStart = writer.BaseStream.Position;
        count = names.Count;
        writer.Write7BitEncodedInt(count);
        for (int i = 0; i < count; i++)
        {
            writer.Write(names[i]);
        }

        ///移回 字符串
        var endPos = writer.BaseStream.Position;
        var offset =(int) (namesStart - pos);
        writer.BaseStream.Position = pos - 4;
        writer.Write(offset);
        writer.BaseStream.Position = endPos;
    }

    public void Read(BinaryReader reader)
    {
        var offset = reader.ReadInt32();
        var pos = reader.BaseStream.Position;
        ///偏移到string列表存储空间
        reader.BaseStream.Position = pos+offset;
        var count = reader.Read7BitEncodedInt();
        names = new List<string>(count);
        for (int i = 0; i < count; i++)
        {
            var str = reader.ReadString();
            names.Add(str);
        }
        reader.BaseStream.Position = pos;
        ////返回

        count = reader.Read7BitEncodedInt();
        dirs = new List<DirVO>(count);
        for (int i = 0; i < count; i++)
        {
            var dirVO = new DirVO();
            dirVO.Read(reader,this);
            dirs.Add(dirVO);
        }
        count  = reader.Read7BitEncodedInt();
        files = new List<FileVO>(count);
        for (int i = 0; i < count; i++)
        {
            var fileVO = new FileVO();
            fileVO.Read(reader,this);
            files.Add(fileVO);
        }
    }
}