using System;

public class CPHInline
{
  public bool Execute()
  {
    // your main code goes here
    decimal amount = 0;

    if (args.ContainsKey("donationAmount"))
    {
      amount = (decimal)args["donationAmount"];
    }
    else if (args.ContainsKey("tipAmount"))
    {
      amount = (decimal)args["tipAmount"];
    }
    CPH.SetArgument("amount", amount.ToString());
    CPH.LogDebug($"Givemas Increment: {amount.ToString()}");
    CPH.RunAction("GoalIncrement");
    return true;
  }
}
