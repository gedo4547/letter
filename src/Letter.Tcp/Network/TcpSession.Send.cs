using System;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Letter.Tcp
{
    partial class TcpSession
    {
        private async Task DoSend()
        {
            Exception shutdownReason = null;
            Exception unexpectedError = null;

            try
            {
                await ProcessSends();
            }
            catch (SocketException ex) when (IsConnectionResetError(ex.SocketErrorCode))
            {
                shutdownReason = new ConnectionResetException(ex.Message, ex);
                _trace.ConnectionReset(this.Id);
            }
            catch (Exception ex)
                when ((ex is SocketException socketEx && IsConnectionAbortError(socketEx.SocketErrorCode)) ||
                      ex is ObjectDisposedException)
            {
                // This should always be ignored since Shutdown() must have already been called by Abort().
                shutdownReason = ex;
            }
            catch (Exception ex)
            {
                shutdownReason = ex;
                unexpectedError = ex;
                _trace.ConnectionError(this.Id, unexpectedError);
            }
            finally
            {
                Shutdown(shutdownReason);

                // Complete the output after disposing the socket
                Output.Complete(unexpectedError);

                // Cancel any pending flushes so that the input loop is un-paused
                Input.CancelPendingFlush();
            }
        }
        
        private async Task ProcessSends()
        {
            // Resolve `output` PipeReader via the IDuplexPipe interface prior to loop start for performance.
            var output = Output;
            while (true)
            {
                var result = await output.ReadAsync();

                if (result.IsCanceled)
                {
                    break;
                }

                var buffer = result.Buffer;

                var end = buffer.End;
                var isCompleted = result.IsCompleted;
                if (!buffer.IsEmpty)
                {
                    await _sender.SendAsync(buffer);
                }

                output.AdvanceTo(end);

                if (isCompleted)
                {
                    break;
                }
            }
        }
    }
}