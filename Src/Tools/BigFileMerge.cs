namespace bgf;

public class BigFileMerge
{
    private BigFileReader destReader;

    public void Init(string path)
    {
        destReader = new BigFileReader();
        destReader.Init(path);

    }
    public void Merge(FileVO fileVO, BinaryReader reader)
    {
        reader.BaseStream.Position = fileVO.position;
        var srcBigFile = Utils.GetBigFile(reader);
        var srcReader = new BigFileReader();
        srcReader.Init(srcBigFile, reader, fileVO.position);
        Merge(srcReader);
    }

    protected void Merge(BigFileReader srcReader)
    {
        ///合并文件列表;
        var mergeMap = new Dictionary<string, RuntimeMergeFileVO>();
        var dic = destReader.GetMapping();
        foreach (var key in dic.Keys)
        {
            var vo = new RuntimeMergeFileVO();
            vo.fileVO = dic[key];
            vo.reader = destReader;
            mergeMap[key] = vo;
        }
        
        dic = srcReader.GetMapping();
        ///优先用新的
        foreach (var key in dic.Keys)
        {
            var vo = new RuntimeMergeFileVO();
            vo.fileVO = dic[key];
            vo.reader = srcReader;
            mergeMap[key] = vo;
        }


    }
}

class RuntimeMergeFileVO
{
    public FileVO fileVO;
    public BigFileReader reader;

    protected void Init()
    {
        reader.
    }
    public int Read(byte[] buff, int offset, int size)
    {

    }
}