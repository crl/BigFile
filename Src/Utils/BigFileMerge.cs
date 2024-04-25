namespace bgf;

public class BigFileMerge
{
    private BigFileReader destReader;
    
    public void Init(string path)
    {
        destReader = new BigFileReader();
        destReader.Init(path);
    }
    internal void Merge(FileVO fileVO, BinaryReader reader)
    {
        reader.BaseStream.Position= fileVO.position;

        var srcBigFile = Utils.GetBigFile(reader);
        var srcReader = new BigFileReader();
        srcReader.Init(srcBigFile, reader, fileVO.position);

        //Merge(srcBigFile);
    }
}