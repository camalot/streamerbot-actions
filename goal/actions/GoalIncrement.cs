using System;

public class CPHInline
{
  public bool Execute()
  {
    var goalFile = CPH.GetGlobalVar<string>("gpb_goal_file", true);
    var currentFile = CPH.GetGlobalVar<string>("gpb_current_file", true);
    decimal goal = 0;
    decimal current = 0;
    var goalLines = System.IO.File.ReadAllText(goalFile).Trim().Replace("$", "");
    var currentLines = System.IO.File.ReadAllText(currentFile).Trim().Replace("$", "");
    decimal.TryParse(currentLines, out current);
    decimal.TryParse(goalLines, out goal);

    decimal increase = 0;
    if (args.ContainsKey("input0"))
    {
      decimal.TryParse(args["input0"] as string, out increase);
    }
    else if (args.ContainsKey("amount"))
    {
      decimal.TryParse(args["amount"] as string, out increase);
    }
    else
    {
      return true;
    }
    current += increase;

    CPH.LogDebug($"Goal Increment: {increase.ToString()}");

    System.IO.File.WriteAllText(currentFile, current.ToString("c"));

    CPH.SetArgument("gpbIncrease", increase);
    CPH.SetArgument("gpbIncreaseFormatted", increase.ToString("c"));
    CPH.SetArgument("gpbTotal", current);
    CPH.SetArgument("gpbTotalFormatted", current.ToString("c"));
    CPH.SetArgument("gpbGoal", goal);
    CPH.SetArgument("gpbGoalFormatted", goal.ToString("c"));

    CPH.RunAction("GoalWebSocket");

    return true;
  }
}