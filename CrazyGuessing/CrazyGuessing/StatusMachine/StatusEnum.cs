using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrazyGuessing.StatusMachine
{
    public enum StatusEnum
    {
        CustomCode = 0,
        FrontPage = 1,
        PreparePage = 2,
        CountBackwards = 3,
        GuessWords = 4,
        ResultPage = 5,
        ViewInfo = 6,
        ViewRule = 7,
    }
}
