
using System.IO.Compression;

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


    public void Write(BinaryWriter writer)
    {
        var count = dirs.Count;
        writer.Write7BitEncodedInt(count);
        for (int i = 0; i < count; i++)
        {
            dirs[i].Write(writer);
        }
        count = files.Count;
        writer.Write7BitEncodedInt(count);
        for (int i = 0; i < count; i++)
        {
            files[i].Write(writer);
        }
    }

    public void Read(BinaryReader reader)
    {
        var count = reader.Read7BitEncodedInt();
        dirs = new List<DirVO>(count);
        for (int i = 0; i < count; i++)
        {
            var dirVO = new DirVO();
            dirVO.Read(reader);
            dirs.Add(dirVO);
        }
        count  = reader.Read7BitEncodedInt();
        files = new List<FileVO>(count);
        for (int i = 0; i < count; i++)
        {
            var fileVO = new FileVO();
            fileVO.Read(reader);
            files.Add(fileVO);
        }
    }
}