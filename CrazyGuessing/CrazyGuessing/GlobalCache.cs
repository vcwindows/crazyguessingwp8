using System.Collections.Generic;
using CrazyGuessing.StatusMachine;

namespace CrazyGuessing
{
    public static class GlobalCache
    {
        public static List<string> GuessingList = new List<string>();
        public static StatusEnum CurrentStatus = StatusEnum.CustomCode;
    }
}
