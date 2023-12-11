#region License
/*
Copyright 2022-2023 Dmitrii Evdokimov
Open source software

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/
#endregion

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Diev.Extensions.QueueService;

public sealed class QueuedHostedService : BackgroundService
{
    private readonly IBackgroundTaskQueue _taskQueue;
    private readonly ILogger<QueuedHostedService> _logger;

    public QueuedHostedService(
        IBackgroundTaskQueue taskQueue,
        ILogger<QueuedHostedService> logger) =>
        (_taskQueue, _logger) = (taskQueue, logger);

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation(
            $"{nameof(QueuedHostedService)} is running.{Environment.NewLine}" +
            $"{Environment.NewLine}Tap W to add a work item to the " +
            $"background queue.{Environment.NewLine}");

        return ProcessTaskQueueAsync(stoppingToken);
    }

    /// <summary>
    /// The ProcessTaskQueueAsync method returns a Task in ExecuteAsync.
    /// Background tasks in the queue are dequeued and executed in ProcessTaskQueueAsync.
    /// Work items are awaited before the service stops in StopAsync.
    /// </summary>
    /// <param name="stoppingToken"></param>
    /// <returns></returns>
    private async Task ProcessTaskQueueAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                Func<CancellationToken, ValueTask>? workItem =
                    await _taskQueue.DequeueAsync(stoppingToken);

                await workItem(stoppingToken);
            }
            catch (OperationCanceledException)
            {
                // Prevent throwing if stoppingToken was signaled
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred executing task work item.");
            }
        }
    }

    public override async Task StopAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation(
            $"{nameof(QueuedHostedService)} is stopping.");

        await base.StopAsync(stoppingToken);
    }
}
