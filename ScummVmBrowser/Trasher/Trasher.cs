using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using System.Threading;
using Microsoft.VisualStudio.Threading;
using static Microsoft.VisualStudio.Threading.AsyncReaderWriterLock;
using System.Threading.Tasks;
using ManagedCommon.Interfaces;

namespace Trasher
{
    public class Trasher : ITrasher
    {
        private ITrashable _trashable;
        private System.Timers.Timer _timer;
        private Action _trashRoutine;
        SemaphoreSlim _syncLock;
        private int _readerCount;
        

        public bool IsTrashed { get; private set; }

        public Trasher(ITrashable trashable, int trashAfterMinutes, Action trashRoutine)
        {
            _trashable = trashable;

            _timer = new System.Timers.Timer(new TimeSpan(0, trashAfterMinutes, 0).TotalMilliseconds);
            _timer.Elapsed += TrashTimerCallback;

            _syncLock = new SemaphoreSlim(1);

            _trashRoutine = trashRoutine;

            _readerCount = 0;

            IsTrashed = false;

            _timer.Start();
        }

        public void ScheduleTrashing()
        {
            Task.Factory.StartNew(() => TrasherTimerCallback());
        }


        private async Task TrasherTimerCallback()
        {
            if (!IsTrashed)
            {
                try
                {
                    await _syncLock.WaitAsync();

                    if (!IsTrashed)
                    {
                        _timer.Stop();

                        foreach (Func<Task> a in _trashable.WhenTrashedSubscribers)
                        {
                            await a();
                        }

                        _trashRoutine();

                        IsTrashed = true;
                    }
                }
                finally
                {
                    _syncLock.Release();
                }
            }
        }

        public async Task PauseTrashCountDown()
        {
            if (!IsTrashed)
            {
                try
                {
                    await _syncLock.WaitAsync();
                    _readerCount++;

                    if (!IsTrashed && _readerCount == 1)
                    {
                        _timer.Stop();
                    }

                }
                finally
                {
                    _syncLock.Release();
                }
            }
        }

        public async Task ResumeTrashCountDown()
        {
            if (!IsTrashed)
            {
                try
                {
                    await _syncLock.WaitAsync();
                    _readerCount--;

                    if (!IsTrashed && _readerCount == 0)
                    {
                        _timer.Start();
                    }

                }
                finally
                {
                    _syncLock.Release();
                }
            }
        }

        private async void TrashTimerCallback(object sender, ElapsedEventArgs e)
        {
            try
            {
                await TrasherTimerCallback();
            }
            catch (Exception ex)
            {
                //ToDo: Log
            }
        }

        public void Dispose()
        {
            _timer.Dispose();
            _syncLock.Dispose();
        }
    }
}