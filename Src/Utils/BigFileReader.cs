
namespace bgf;

/// <summary>
/// 大文件读取
/// </summary>
public class BigFileReader
{
    protected BinaryReader reader;
    protected BigFile bigFile;
    protected Dictionary<string, FileVO> map = new Dictionary<string, FileVO>();
    protected long streamOffset;
    public void Init(string path)
    {
        var fs = File.OpenRead(path);
        reader = new BinaryReader(fs);
        var v = Utils.GetBigFile(reader);
        if (v != null)
        {
            Init(v, reader, 0);
        }
    }

    public void Init(BigFile bigFile,BinaryReader reader,long offset=0)
    {
        this.bigFile = bigFile;
        this.reader = reader;
        this.streamOffset = offset;

        var root=bigFile.dirs[0];
        Mapping(root);
    }

    private void Mapping(DirVO parent,string parentPath="")
    {
        foreach (var idx in parent.files)
        {
            var vo = this.bigFile.files[idx];
            var path = parentPath + vo.name;
            map[path] = vo;
        }

        foreach (var idx in parent.dirs)
        {
            var vo = this.bigFile.dirs[idx];
            Mapping(vo, parentPath + vo.name + "/");
        }
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