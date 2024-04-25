
namespace bgf;

/// <summary>
/// 大文件读取
/// </summary>
public class BigFileReader
{
    private BinaryReader reader;
    private BigFile bigFile;
    private Dictionary<string, FileVO> map = new Dictionary<string, FileVO>();
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

        foreach (var file in bigFile.files)
        {
            map[file.path] = file;
        }

        return true;
    }

    public BigFile GetBigFile()
    {
        return bigFile;
    }

    public FileVO Get(string filePath)
    {
        map.TryGetValue(filePath, out var fileVO);
        return fileVO;
    }

    public void Close()
    {
        if (reader != null)
        {
            reader.Close();
            reader = null;
        }

        map.Clear();
        bigFile = null;
    }
}