
using Grpc.Net.Client;
using grpcFileDowloandTransportClient;

var channel = GrpcChannel.ForAddress("https://localhost:7062");
var client = new FileService.FileServiceClient(channel);

string dowloandPath = "C:\\Users\\Burak\\Desktop\\gRPC\\grpcDowloandClient\\fileDowloand";

var fileInfo = new grpcFileDowloandTransportClient.FileInfo
{
    FileName = "2023-05-23 09-41-04",
    FileExtension = ".mkv"
};

FileStream fileStream = null;

var request = client.FileDownload(fileInfo);

CancellationTokenSource cancellationTokenSource = new();

int count = 0;
decimal chunkSize = 0;

while (await request.ResponseStream.MoveNext(cancellationTokenSource.Token))
{
    if (count++ == 0)
    {
        fileStream = new FileStream($"{dowloandPath}\\{request.ResponseStream.Current.Info.FileName}{request.ResponseStream.Current.Info.FileExtension}", FileMode.CreateNew);
        fileStream.SetLength(request.ResponseStream.Current.FileSize);
    }
    var buffer = request.ResponseStream.Current.Buffer.ToByteArray();
    await fileStream.WriteAsync(buffer, 0, request.ResponseStream.Current.ReadedByte);
    Console.WriteLine($"{Math.Round((chunkSize += request.ResponseStream.Current.ReadedByte) * 100 / request.ResponseStream.Current.FileSize)}%");

}

Console.WriteLine("Yüklendi");

await fileStream.DisposeAsync();
fileStream.Close();

