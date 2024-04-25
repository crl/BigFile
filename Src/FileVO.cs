namespace bgf;

public class FileVO
{
    public string path;
    public long position;
    public int size;

    public void Write(BinaryWriter writer)
    {
        writer.Write(path);

        writer.Write7BitEncodedInt64(position);
        writer.Write7BitEncodedInt(size);
    }

    public void Read(BinaryReader reader)
    {
        path = reader.ReadString();
        position = reader.Read7BitEncodedInt64();
        size = reader.Read7BitEncodedInt();
    }
}

public class DirVO
{
    public string path;

    public List<int> dirs=new List<int>();
    public List<int> files=new List<int>();


    public void Write(BinaryWriter writer)
    {
        writer.Write(path);

        var len = dirs.Count;
        writer.Write7BitEncodedInt(len);
        for (int i = 0; i < len; i++)
        {
            writer.Write7BitEncodedInt(dirs[i]);
        }

        len = files.Count;
        writer.Write7BitEncodedInt(len);
        for (int i = 0; i < len; i++)
        {
            writer.Write7BitEncodedInt(files[i]);
        }
    }

    public void Read(BinaryReader reader)
    {
        path = reader.ReadString();

        var len = reader.Read7BitEncodedInt();
        dirs = new List<int>(len);
        for (int i = 0; i < len; i++)
        {
            dirs.Add(reader.Read7BitEncodedInt());
        }

        len = reader.Read7BitEncodedInt();
        files = new List<int>(len);
        for (int i = 0; i < len; i++)
        {
            files.Add(reader.Read7BitEncodedInt());
        }
    }
}