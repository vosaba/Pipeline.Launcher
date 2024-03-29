﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace PipelineLauncher.Extensions.Infrastructure
{
    internal class AsyncAutoResetEvent
    {
        readonly LinkedList<TaskCompletionSource<bool>> _waiters = new LinkedList<TaskCompletionSource<bool>>();

        bool _isSignaled;

        public AsyncAutoResetEvent(bool signaled)
        {
            _isSignaled = signaled;
        }

        public Task<bool> WaitAsync(TimeSpan timeout)
        {
            return WaitAsync(timeout, CancellationToken.None);
        }

        public async Task<bool> WaitAsync(TimeSpan timeout, CancellationToken cancellationToken)
        {
            TaskCompletionSource<bool> tcs;

            lock (_waiters)
            {
                if (_isSignaled)
                {
                    _isSignaled = false;
                    return true;
                }
                else if (timeout == TimeSpan.Zero)
                {
                    return _isSignaled;
                }
                else
                {
                    tcs = new TaskCompletionSource<bool>();
                    _waiters.AddLast(tcs);
                }
            }

            Task winner = await Task.WhenAny(tcs.Task, Task.Delay(timeout, cancellationToken));
            if (winner == tcs.Task)
            {
                // The task was signaled.
                return true;
            }
            else
            {
                // We timed-out; remove our reference to the task.
                // This is an O(n) operation since waiters is a LinkedList<T>.
                lock (_waiters)
                {
                    bool removed = _waiters.Remove(tcs);
                    Debug.Assert(removed);
                    return false;
                }
            }
        }

        public void Set()
        {
            lock (_waiters)
            {
                if (_waiters.Count > 0)
                {
                    // Signal the first task in the waiters list. This must be done on a new
                    // thread to avoid stack-dives and situations where we try to complete the
                    // same result multiple times.
                    TaskCompletionSource<bool> tcs = _waiters.First.Value;
                    Task.Run(() => tcs.SetResult(true));
                    _waiters.RemoveFirst();
                }
                else if (!_isSignaled)
                {
                    // No tasks are pending
                    _isSignaled = true;
                }
            }
        }

        public override string ToString()
        {
            return $"Signaled: {_isSignaled.ToString()}, Waiters: {_waiters.Count.ToString()}";
        }
    }
}
