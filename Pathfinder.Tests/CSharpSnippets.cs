namespace Pathfinder.Tests {
    internal static class CSharpSnippets {
        internal const string GoodCode =
            @"using System;

              namespace GoodCode {
                  class Program {
                      static void Main(string[] args) {
                          Console.WriteLine(""Hello World!"");
                      }
                  }
              }";

        internal const string BadCode =
            @"using System;

              namespace BadCode {
                  class Program {
                      static void Main(string[] args) {
                          Console.WriteLine(""Hello World!"")
                      }
                  }
              }";

        internal const string ExceptionCode =
            @"using System;

              namespace BadCode {
                  class Program {
                      static void Main(string[] args) {
                          throw new InvalidOperationException();
                      }
                  }
              }";

        internal const string GoodConsoleWriteLineString = @"Console.WriteLine(""Hello World!"");";

        internal const string BadConsoleWriteLineString = @"Console.WriteLine(""Hello World!"")";

        internal const string ExceptionConsoleWriteLineString = @"throw new InvalidOperationException();";
    }
}
