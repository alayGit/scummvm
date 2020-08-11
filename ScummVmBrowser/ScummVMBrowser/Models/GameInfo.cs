using ScummVMBrowser.Clients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Trasher;
using Microsoft.VisualStudio.Threading;
using System.Threading.Tasks;
using ScummVMBrowser.StaticData;
using ManagedCommon.Interfaces;
using ManagedCommon.Enums;
using ManagedCommon.Interfaces.Rpc;

namespace ScummVMBrowser.Models
{
    public class GameInfo : IGameInfo
    {
        private IScummVMHubClient _client;
        private bool _isDisposed;
        private Trasher.Trasher _trasher;
        private List<Func<Task>> _whenTrashedSubscribers;
        public IEnumerable<Func<Task>> WhenTrashedSubscribers => _whenTrashedSubscribers.AsReadOnly();
                                             
        public GameInfo(IScummVMHubClient client, IConfigurationStore<System.Enum> configureStore)
        {
            _disposalLock = new AsyncReaderWriterLock();
            _isDisposed = false;

            _client = client;
            _client.OnQuit += () =>
            {
                _trasher.ScheduleTrashing();
                return Task.CompletedTask;
            };

            _whenTrashedSubscribers = new List<Func<Task>>();
            _trasher = new Trasher.Trasher(this, configureStore.GetValue<int>(ManagedCommon.Enums.Trasher.TrashTimeMin), () =>
            {
                Dispose();
            });

            SubscribeToTrashEvent(_client.BeforeDispose);
        }

        private AsyncReaderWriterLock _disposalLock;

        public async Task<IAntiDisposalLock<IScummVMHubClient>> GetClient()
        {
            IAntiDisposalLock<IScummVMHubClient> result = new AntiDisposalLock<IScummVMHubClient>(null, null); //Return a wrapper around nothing in the case that we never get the lock, that way it can still be wrapped around usings
            AsyncReaderWriterLock.Releaser? releaser = null;
            bool isReaderLockHeld = false;

            if (!_isDisposed)
            {
                try
                {
                    releaser = await _disposalLock.ReadLockAsync();
                    isReaderLockHeld = _disposalLock.IsReadLockHeld;
                    if (isReaderLockHeld)
                    {
                        await _trasher.PauseTrashCountDown();
                        result = new AntiDisposalLock<IScummVMHubClient>(_client, releaser);
                    }

                }
                catch (ObjectDisposedException)
                {

                }
                finally
                {
                    if (releaser != null && !isReaderLockHeld)
                    {
                        releaser.Value.Dispose(); //Dispose if the lock was never granted due to disposal starting
                    }
                    else if (isReaderLockHeld)
                    {
                        await _trasher.ResumeTrashCountDown();
                    }
                }
            }
            return result;
        }

        public async void Dispose()
        {
            using (await _disposalLock.WriteLockAsync())
            {
                _client.Dispose();
                _disposalLock.Complete();
                _disposalLock.Dispose();
                _trasher.Dispose();
                _isDisposed = true;
            }
        }

        public void SubscribeToTrashEvent(Func<Task> onTrashed)
        {
            _whenTrashedSubscribers.Add(onTrashed);
        }
    }
}