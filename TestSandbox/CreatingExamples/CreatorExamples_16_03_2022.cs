using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestSandbox.PlatformImplementations;

namespace TestSandbox.CreatingExamples
{
    public class CreatorExamples_16_03_2022 : BaseCreatorExamples
    {
        private static readonly IEntityLogger _logger = new LoggerImpementation();

        public void Run()
        {
            _logger.Log("Begin");

            Variables();
            MemberAccessModifiers();//in process
            Field();
            ImperativeLogicOperators();//in process
            ArithmeticOperators();//in process
            LogicConditionalTriggers();//in process
            LogicQueries();//in process
            RepeatStatement();//in process
            WhileStatement();//in process
            ReturnStatement();//in process
            IfElifElseStatement();//in process
            ContinueLoopStatement();//in process
            BreakLoopStatement();//in process

            _logger.Log("End");
        }

        private void Variables()
        {
            var prefix = "Variables";
            var n = 3;

            var text = @"app PeaceKeeper
{
    on Init =>
    {
        'Begin' >> @>log;
        var @a: number = 2;
        @a >> @>log;
        var @b: number;
        @b >> @>log;
        var @c;
        @c >> @>log;
        'End' >> @>log;
    }
}";

            Example(CreateName(prefix, n), text);
        }

        private void MemberAccessModifiers()
        {
            var prefix = "MemberAccessModifiers";

            var n = 1;

            
        }

        private void Field()
        {
            var prefix = "Field";

            var n = 1;

            var text = @"app PeaceKeeper
{
    @b;

    on Init =>
    {
        'Begin' >> @>log;
        @b >> @>log;
        'End' >> @>log;
    }
}";

            Example(CreateName(prefix, n), text);

            n++;

            text = @"app PeaceKeeper
{
    var @b: number = 2;

    on Init =>
    {
        'Begin' >> @>log;
        @b >> @>log;
        'End' >> @>log;
    }
}";

            Example(CreateName(prefix, n), text);
        }

        private void ImperativeLogicOperators()
        {
            var prefix = "ImperativeLogicOperators";

            var n = 1;
        }

        private void ArithmeticOperators()
        {
            var prefix = "ArithmeticOperators";

            var n = 1;
        }

        private void LogicConditionalTriggers()
        {
            var prefix = "LogicConditional_Trigger";

            var n = 3;

        }

        private void LogicQueries()
        {
            var prefix = "LogicQueries";

            var n = 13;


        }

        private void RepeatStatement()
        {
            var prefix = "RepeatStatement";

            var n = 1;
        }

        private void WhileStatement()
        {
            var prefix = "WhileStatement";

            var n = 1;


        }

        private void ReturnStatement()
        {
            var prefix = "ReturnStatement";

            var n = 1;


        }

        private void IfElifElseStatement()
        {
            var prefix = "IfElifElseStatement";

            var n = 1;


        }

        private void ContinueLoopStatement()
        {
            var prefix = "ContinueLoopStatement";

            var n = 1;


        }

        private void BreakLoopStatement()
        {
            var prefix = "BreakLoopStatement";

            var n = 1;


        }

        /*
        var text = @"";

        Example(CreateName(prefix, n), text);

        text = @"";

        Example(CreateName(prefix, n), text);
         */
    }
}
