using System;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Letter.Tcp
{
    partial class TcpSession
    {
        private async Task DoReceive()
        {
            Exception error = null;
            
            try
            {
                await ProcessReceives();
            }
            catch (SocketException ex) when (IsConnectionResetError(ex.SocketErrorCode))
            {
                // This could be ignored if _shutdownReason is already set.
                error = new ConnectionResetException(ex.Message, ex);

                // There's still a small chance that both DoReceive() and DoSend() can log the same connection reset.
                // Both logs will have the same ConnectionId. I don't think it's worthwhile to lock just to avoid this.
                if (!_socketDisposed)
                {
                    _trace.ConnectionReset(this.Id);
                }
            }
            catch (Exception ex)
                when ((ex is SocketException socketEx && IsConnectionAbortError(socketEx.SocketErrorCode)) ||
                       ex is ObjectDisposedException)
            {
                // This exception should always be ignored because _shutdownReason should be set.
                error = ex;

                if (!_socketDisposed)
                {
                    // This is unexpected if the socket hasn't been disposed yet.
                    _trace.ConnectionError(this.Id, error);
                }
            }
            catch (Exception ex)
            {
                // This is unexpected.
                error = ex;
                _trace.ConnectionError(this.Id, error);
            }
            finally
            {
                // If Shutdown() has already bee called, assume that was the reason ProcessReceives() exited.
                Input.Complete(_shutdownReason ?? error);

                FireConnectionClosed();

                await _waitForConnectionClosedTcs.Task;
            }
        }
        
        
        private async Task ProcessReceives()
        {
            // Resolve `input` PipeWriter via the IDuplexPipe interface prior to loop start for performance.
            var input = Input;
            while (true)
            {
                if (_waitForData)
                {
                    // Wait for data before allocating a buffer.
                    await _receiver.WaitForDataAsync();
                }

                // Ensure we have some reasonable amount of buffer space
                var buffer = input.GetMemory(MinAllocBufferSize);

                var bytesReceived = await _receiver.ReceiveAsync(buffer);

                if (bytesReceived == 0)
                {
                    // FIN
                    _trace.ConnectionReadFin(this.Id);
                    break;
                }

                input.Advance(bytesReceived);

                var flushTask = input.FlushAsync();

                var paused = !flushTask.IsCompleted;

                if (paused)
                {
                    _trace.ConnectionPause(this.Id);
                }

                var result = await flushTask;

                if (paused)
                {
                    _trace.ConnectionResume(this.Id);
                }

                if (result.IsCompleted || result.IsCanceled)
                {
                    // Pipe consumer is shut down, do we stop writing
                    break;
                }
            }
        }
        
      
    }
}