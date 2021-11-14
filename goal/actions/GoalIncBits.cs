using System;

public class CPHInline
{
  public bool Execute()
  {
    var increase = 0.01;

    if (args.ContainsKey("bits"))
    {
      int count = (int)args["bits"];
      increase = increase * count;
    }
    CPH.SetArgument("amount", increase.ToString());
    CPH.LogDebug($"Givemas Increment: {increase.ToString()}");
    CPH.RunAction("GoalIncrement");
    return true;
  }
}
