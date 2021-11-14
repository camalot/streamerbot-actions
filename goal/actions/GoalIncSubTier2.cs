using System;

public class CPHInline
{
  public bool Execute()
  {
    var increase = 5;
    if (args.ContainsKey("gifts"))
    {
      int count = (int)args["gifts"];
      increase = increase * count;
    }
    CPH.SetArgument("amount", increase.ToString());
    CPH.RunAction("GoalIncrement");
    return true;
  }
}
