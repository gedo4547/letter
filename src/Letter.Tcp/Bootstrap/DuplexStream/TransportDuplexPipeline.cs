// using System.IO.Pipelines;
//
// namespace Letter
// {
//     class TransportDuplexPipeline : IDuplexPipe
//     {
//         public TransportDuplexPipeline(PipeReader reader, PipeWriter writer)
//         {
//             Input = reader;
//             Output = writer;
//         }
//
//         public PipeReader Input { get; }
//         public PipeWriter Output { get; }
//
//
//         public static DuplexPipePair CreatePair(in TransportOptions options)
//         {
//             var inputOption = new PipeOptions(
//                options.memoryPool,
//                PipeScheduler.ThreadPool,
//                options.scheduler,
//                options.memoryPool.MaxBufferSize,
//                options.memoryPool.MaxBufferSize / 2,
//                useSynchronizationContext: false);
//
//             var outputOption = new PipeOptions(
//                 options.memoryPool,
//                 options.scheduler,
//                 PipeScheduler.ThreadPool,
//                 options.memoryPool.MaxBufferSize,
//                 options.memoryPool.MaxBufferSize / 2,
//                 useSynchronizationContext: false);
//
//             return CreatePair(inputOption, outputOption);
//         }
//
//         public static DuplexPipePair CreatePair(PipeOptions inputOptions, PipeOptions outputOptions)
//         {
//             var input = new Pipe(inputOptions);
//             var output = new Pipe(outputOptions);
//             
//             var transportToApplication = new TransportDuplexPipeline(output.Reader, input.Writer);
//             var applicationToTransport = new TransportDuplexPipeline(input.Reader, output.Writer);
//
//             return new DuplexPipePair(applicationToTransport, transportToApplication, input, output);
//         }
//
//         // This class exists to work around issues with value tuple on .NET Framework
//         public readonly struct DuplexPipePair
//         {
//             public IDuplexPipe Transport { get; }
//             public IDuplexPipe Application { get; }
//
//             public Pipe InputPipe { get; }
//             
//             public Pipe OutputPipe { get; }
//
//             public DuplexPipePair(IDuplexPipe transport, IDuplexPipe application, Pipe inputPipe, Pipe outputPipe)
//             {
//                 Transport = transport;
//                 Application = application;
//
//                 InputPipe = inputPipe;
//                 OutputPipe = outputPipe;
//             }
//         }
//     }
// }
