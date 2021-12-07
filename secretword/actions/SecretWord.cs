using System;
using System.Net;
using System.Text.RegularExpressions;

public class CPHInline {
  public void Init() {
    SetSecretWord();
  }

  public bool Execute() {
    var input = args["rawInput"] as string;
    if (string.IsNullOrWhiteSpace(input)) {
      return true;
    }

    var secretWord = CPH.GetGlobalVar<string>("SW_SECRETWORD");
    if (string.IsNullOrWhiteSpace(secretWord)) {
      SetSecretWord();
    }

    var matchPattern = new Regex($@"(^|\s){secretWord}($|\s)", RegexOptions.Singleline | RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace | RegexOptions.IgnoreCase);
    var match = matchPattern.Match(input);
    if (match.Success) {
      CPH.SetArgument("foundWord", match.Value.Trim());
      CPH.RunAction("FoundSecretWord");
      SetSecretWord();
    }

    return true;
  }

  private void SetSecretWord() {
    var url = "https://gist.githubusercontent.com/camalot/412bd9feaa3b209a5ffad838fde55dc6/raw/ce74b1f078f9f3a35decedea86a1be1cb2e0817c/secretwords.txt";
    var rng = new Random();
    using (var wc = new WebClient()) {
      var lines = wc.DownloadString(url).Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
      //var lines = new string[] { "test", "money", "everything"};
      var lineIndex = rng.Next(lines.Length);
      var secretWord = lines[lineIndex].Trim();
      CPH.LogDebug($@"[SECRETWORD] The secret word is ""{secretWord}""");
      CPH.SetGlobalVar("SW_SECRETWORD", secretWord, true);
    }
  }
}