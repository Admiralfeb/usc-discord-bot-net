using System;
using System.Threading;
using System.Threading.Tasks;
using UnitedSystemsCooperative.Bot.Utils;
using Xunit;

namespace UnitedSystemsCooperative.Bot.Test.Utils;

public class UtilityMethodsTest
{
    [Fact]
    public async Task PeriodicAsync_Should_Cancel()
    {
        var cancelTokenSource = new CancellationTokenSource();
        cancelTokenSource.Cancel();
        await Assert.ThrowsAsync<OperationCanceledException>(
            () => UtilityMethods.PeriodicAsync(() => Task.Run(() => { }), 
            TimeSpan.FromSeconds(5), 
            cancelTokenSource.Token));
    }

    [Fact]
    public async Task PeriodicAsync_Should_Run_Twice()
    {
        var cancelTokenSource = new CancellationTokenSource();
        cancelTokenSource.CancelAfter(10 * 1000);

        int runs = 0;
        async Task MyAction()
        {
            await Task.Run(() => runs++);
        }

        await Assert.ThrowsAsync<OperationCanceledException>(
            () => UtilityMethods.PeriodicAsync(() => MyAction(), 
                TimeSpan.FromSeconds(5), 
                cancelTokenSource.Token));
        Assert.Equal(2, runs);
    }
}
