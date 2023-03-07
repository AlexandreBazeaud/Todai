using System.Timers;
using BlazorWebAssymblyWeb3.Client.Components.Global;
using Timer = System.Timers.Timer;

namespace BlazorWebAssymblyWeb3.Client.Services;

public class AlertMessageService
{
    public AlertMessage AlertMessage;
    private Timer timer;
    
    public enum States
    {
        Success,
        Error,
        Info,
        Event
    }

    private const int timerTime = 5000;
    
    public AlertMessageService()
    {
        timer = new Timer(timerTime);
        timer.Elapsed += HideAlertMessage;
        timer.AutoReset = false;
    }

    public void ShowAlertMessage(States pState, string pMessage)
    {
        if (pMessage is null)
            pMessage = "";
        AlertMessage.SetState(pState,pMessage);
#if DEBUG
        Console.WriteLine($"{Enum.GetName(typeof(States), pState)} => {pMessage}");
#endif
        if(timer.Enabled)
            timer.Stop();
        
        timer.Start();
    }

    public void HideAlertMessage(object? pO, ElapsedEventArgs pArg)
    {
        AlertMessage.Hide();
    }
}