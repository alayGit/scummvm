using ManagedCommon.Enums;
using ManagedCommon.Interfaces;
using Microsoft.VisualStudio.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagedCommon
{
    public class BufferAndProcess<T>
    {
        public bool Stopped { get; set; }


        private Task _processTask;
        private Func<T,Task> _processCallback;
        private AsyncQueue<T> _processQueue;

		public BufferAndProcess(Func<T, Task> processCallback,  IConfigurationStore<Enum> configurationStore)
        {
            Stopped = false;
            _processCallback = processCallback;
            _processQueue = new AsyncQueue<T>();

            _processTask = Task.Run(async () =>
            {
                while (!Stopped)
                {
                    if (!_processQueue.IsEmpty)
                    {
                        T bufferedData = await _processQueue.DequeueAsync();

                        //if (typeof(T).GetType() != typeof(byte[]))
                        //{
                            await _processCallback(bufferedData);
                        //}
                    }
                    else
                    {
                        await Task.Delay(configurationStore.GetValue<int>(ScummHubSettings.BufferAndProcessSleepTime));
                    }
                }
            });
        }

        public void Enqueue(T data)
        {
            _processQueue.Enqueue(data);
        }

        public async Task Stop()
        {
            if(Stopped)
            {
                throw new Exception("Already stopped");
            }

            Stopped = true;

            await _processTask;
        }
    }
}
