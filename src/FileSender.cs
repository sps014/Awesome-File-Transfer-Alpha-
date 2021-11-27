﻿using System.Net;
using System.Net.Sockets;
using System.Net.Http;

namespace FileShare;

public class FileSender
{
    public IPAddress IpAddress { get; }
    public int Port { get; }

    private HttpClient? _httpClient;

    private const long BufferSize=20480;

    public FileSender(IPAddress address, int port)
    {
        IpAddress = address;
        Port = port;
        InitClient();
    }
    public FileSender(string address, int port)
    {
        IpAddress = IPAddress.Parse(address);
        Port = port;
        InitClient();
    }
    private void InitClient()
    {
        _httpClient=HttpClientFactory.Create();
    }

    public async ValueTask SendAsync(string file,uint start=0,uint length=0)
    {
        if(!File.Exists(file))
            throw new FileNotFoundException(file);

        var actualLength=new FileInfo(file).Length;
        if (actualLength < start + length)
            throw new ArgumentException("Length can't be greater than actual file length");

        await SendBuffer(file, start, length,actualLength);
    }

    private async ValueTask SendBuffer(string file, uint start = 0, uint length = 0,long actualLength=0)
    {
        var buffer=new byte[BufferSize];
        long read = 0;

        using FileStream fs=new FileStream(file,FileMode.Open,FileAccess.Read);

        while (read < length)
        {
            long begin = start + read;
            long count = (begin + BufferSize) > actualLength ? actualLength - begin : BufferSize;
            int stepRead=await fs.ReadAsync(buffer,(int)begin,(int)count);
            read += stepRead;

            //Send The bytes
        }
    }
}
