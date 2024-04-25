﻿namespace bgf;

public static class Utils
{
    public static string FormatPath(string path)
    {
        return path.Replace("\\", "/");
    }

    public static DirVO GetDirVO(this BigFile bigFile,string dirPath, out int idx)
    {
        var len = bigFile.dirs.Count;
        for (int i = 0; i < len; i++)
        {
            var vo = bigFile.dirs[i];
            if (vo.path == dirPath)
            {
                idx = i;
                return vo;
            }
        }
        idx = -1;
        return null;
    }

    public static FileVO GetFileVO(this BigFile bigFile,string path, out int idx)
    {
        var len = bigFile.files.Count;
        for (int i = 0; i < len; i++)
        {
            var vo = bigFile.files[i];
            if (vo.path == path)
            {
                idx = i;
                return vo;
            }
        }

        idx = -1;
        return null;
    }
}