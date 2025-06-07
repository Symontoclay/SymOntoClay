using Newtonsoft.Json.Linq;
using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using SymOntoClay.Core.Internal.Parsing.Internal;
using SymOntoClay.Monitor.Common;
using SymOntoClay.Monitor.NLog;
using static System.Net.Mime.MediaTypeNames;

namespace TestSandbox.Handlers
{
    public class ParseNameHandler
    {
        public ParseNameHandler() 
        {
        }

        private static readonly IMonitorLogger _logger = new MonitorLoggerNLogImpementation();

        public void Run()
        {
            _logger.Info("5BF042DE-688A-48D8-8ECD-92233AF6D477", "Begin");

            //Case24();
            //Case23();
            //Case22();
            //Case21();
            //Case20();
            //Case19();
            //Case18();
            //Case17();
            //Case16();
            //Case15();
            //Case14();
            //Case13();
            //Case12();
            //Case12_a();
            //Case11();
            //Case10();
            //Case9();
            //Case8();
            //Case8_a();
            //Case7();
            //Case6();
            //Case5();
            //Case5_a();
            Case4();
            //Case3();
            //Case3_a();
            //Case2();
            //Case2_a();
            //Case2_b();
            //Case1();

            _logger.Info("75A8C4DA-0D97-496F-AF78-258D4B8A5043", "End");
        }

        private void Case24()
        {
            _logger.Info("BC058057-DC3A-4F43-BCC6-DBD3AF930EF8", $"Begin {nameof(Case24)}");

            var str = "__ctor";

            _logger.Info("865F6911-5A70-4122-BDF6-9CE938D42B68", $"str = '{str}'");

            var parserContext = new InternalParserCoreContext(str, _logger, LexerMode.StrongIdentifier);

            var parser = new StrongIdentifierValueParser(parserContext);
            parser.Run();

            var result = parser.Result;

            _logger.Info("ABA4A563-E829-4B5B-9C57-655D90E84F90", $"result = {result}");
            _logger.Info("4A2C87EA-2A9F-4264-A294-A48460089EA0", $"result.ToHumanizedLabel() = {result.ToHumanizedLabel()}");

            var debugHelperOptions = DebugHelperOptions.FromHumanizedOptions();
            debugHelperOptions.ShowPrefixesForConceptLikeIdentifier = false;

            _logger.Info("DA5D5799-221E-4739-8639-08BDA51FECC5", $"result.ToHumanizedLabel(debugHelperOptions) = {result.ToHumanizedLabel(debugHelperOptions)}");

            _logger.Info("5FFB497B-BA04-404C-A24F-F94A7A0B688A", $"result.ToHumanizedString() = {result.ToHumanizedString()}");
            _logger.Info("18B0F106-794C-4BA5-B3A3-04DC2F17559C", $"result.ToHumanizedString(debugHelperOptions) = {result.ToHumanizedString(debugHelperOptions)}");

            //var lexer = new Lexer(str, _logger, LexerMode.StrongIdentifier);

            //Token token = null;

            //while ((token = lexer.GetToken()) != null)
            //{
            //    _logger.Info("7F6FCD36-F54B-4285-A26B-84E25A927928", $"token = {token}");
            //}

            _logger.Info("EFB068BA-5671-41C3-A7BD-A720C30B8120", $"End {nameof(Case24)}");
        }

        private void Case23()
        {
            _logger.Info("304DD03B-842C-47B6-BDE2-E8E1F3BB8CCE", $"Begin {nameof(Case23)}");

            var str = "(animal | instrument)::dog";

            _logger.Info("8B3AA3A3-E2DE-4DF7-8B29-631D7911EDE0", $"str = '{str}'");

            var parserContext = new InternalParserCoreContext(str, _logger, LexerMode.StrongIdentifier);

            var parser = new StrongIdentifierValueParser(parserContext);
            parser.Run();

            var result = parser.Result;

            _logger.Info("19F2C080-A816-4159-9576-186C22A430F1", $"result = {result}");
            _logger.Info("43869980-52EA-4F14-B536-60FD513EB7EC", $"result.ToHumanizedLabel() = {result.ToHumanizedLabel()}");

            var debugHelperOptions = DebugHelperOptions.FromHumanizedOptions();
            debugHelperOptions.ShowPrefixesForConceptLikeIdentifier = false;

            _logger.Info("0307EEF4-F8C4-4935-AB69-C8D02AAB430B", $"result.ToHumanizedLabel(debugHelperOptions) = {result.ToHumanizedLabel(debugHelperOptions)}");

            _logger.Info("19980518-2156-42DF-8402-17A1A26443A2", $"result.ToHumanizedString() = {result.ToHumanizedString()}");
            _logger.Info("9BCF2F98-E993-464D-B6FF-5DA735A9BDD3", $"result.ToHumanizedString(debugHelperOptions) = {result.ToHumanizedString(debugHelperOptions)}");

            //var lexer = new Lexer(str, _logger, LexerMode.StrongIdentifier);

            //Token token = null;

            //while ((token = lexer.GetToken()) != null)
            //{
            //    _logger.Info("20556A98-2048-4CBB-AD52-CD068F75CADB", $"token = {token}");
            //}

            _logger.Info("BF0B3233-6E66-4DD0-A364-5F18927CB501", $"End {nameof(Case23)}");
        }

        private void Case22()
        {
            _logger.Info("86BC4BD7-15F4-4C38-925F-844CA835403E", $"Begin {nameof(Case22)}");

            var str = string.Empty;

            _logger.Info("D87EB9E9-C032-4531-86FC-818F96EDE571", $"str = '{str}'");

            var parserContext = new InternalParserCoreContext(str, _logger, LexerMode.StrongIdentifier);

            var parser = new StrongIdentifierValueParser(parserContext);
            parser.Run();

            var result = parser.Result;

            _logger.Info("B31465E6-4E39-44D3-8B0C-CF294F0EBA96", $"result = {result}");
            _logger.Info("410072C1-A845-4449-9F8B-E4AEC2506F8C", $"result.ToHumanizedLabel() = {result.ToHumanizedLabel()}");

            var debugHelperOptions = DebugHelperOptions.FromHumanizedOptions();
            debugHelperOptions.ShowPrefixesForConceptLikeIdentifier = false;

            _logger.Info("44C74FED-8E71-41E0-B062-46BF5765C732", $"result.ToHumanizedLabel(debugHelperOptions) = {result.ToHumanizedLabel(debugHelperOptions)}");

            _logger.Info("027DC12D-F41C-445A-B1FA-3C9186D3311B", $"result.ToHumanizedString() = {result.ToHumanizedString()}");
            _logger.Info("61E849D5-1E71-49EC-AB6F-BD6A62778460", $"result.ToHumanizedString(debugHelperOptions) = {result.ToHumanizedString(debugHelperOptions)}");

            //var lexer = new Lexer(str, _logger, LexerMode.StrongIdentifier);

            //Token token = null;

            //while ((token = lexer.GetToken()) != null)
            //{
            //    _logger.Info("EBC37EA8-26B0-4DD4-A550-C1811AF75712", $"token = {token}");
            //}

            _logger.Info("087F1A66-CCF7-47ED-BDCD-53566B62DBBD", $"End {nameof(Case22)}");
        }

        private void Case21()
        {
            _logger.Info("69318DF4-E5BA-46C4-8B77-F335D7777FC1", $"Begin {nameof(Case21)}");

            var str = "alive::animal::big::dog";

            _logger.Info("35989597-02BF-4FF9-A96A-68A6247FA81C", $"str = '{str}'");

            var parserContext = new InternalParserCoreContext(str, _logger, LexerMode.StrongIdentifier);

            var parser = new StrongIdentifierValueParser(parserContext);
            parser.Run();

            var result = parser.Result;

            _logger.Info("9B4C3F2B-AC1C-46F6-AFE5-C61BEC1E4F86", $"result = {result}");
            _logger.Info("AB136716-9835-433C-A6E4-62E3972AB18D", $"result.ToHumanizedLabel() = {result.ToHumanizedLabel()}");

            var debugHelperOptions = DebugHelperOptions.FromHumanizedOptions();
            debugHelperOptions.ShowPrefixesForConceptLikeIdentifier = false;

            _logger.Info("E8189A5B-709B-4F92-9241-9E9C5BF6D6EC", $"result.ToHumanizedLabel(debugHelperOptions) = {result.ToHumanizedLabel(debugHelperOptions)}");

            _logger.Info("94118B34-9C68-47BC-A36E-C07DA904B743", $"result.ToHumanizedString() = {result.ToHumanizedString()}");
            _logger.Info("3A7E5D50-A804-47A1-B4BA-176020547001", $"result.ToHumanizedString(debugHelperOptions) = {result.ToHumanizedString(debugHelperOptions)}");

            //var lexer = new Lexer(str, _logger, LexerMode.StrongIdentifier);

            //Token token = null;

            //while ((token = lexer.GetToken()) != null)
            //{
            //    _logger.Info("543F604F-6636-48FB-9D40-BC6022CCFE78", $"token = {token}");
            //}

            _logger.Info("67B68601-3458-48EB-9D55-9A444430BE1B", $"End {nameof(Case21)}");
        }

        private void Case20()
        {
            _logger.Info("B254DC94-D94D-4D4A-9960-C9B47863F312", $"Begin {nameof(Case20)}");

            var str = "dog (alive::animal | instrument (big))";

            _logger.Info("61244CFA-EA2E-4763-9DFF-3F7D9867FA15", $"str = '{str}'");

            var parserContext = new InternalParserCoreContext(str, _logger, LexerMode.StrongIdentifier);

            var parser = new StrongIdentifierValueParser(parserContext);
            parser.Run();

            var result = parser.Result;

            _logger.Info("799BCFAC-726C-47DC-BBDB-124BEF912456", $"result = {result}");
            _logger.Info("2EAD75FC-1CFF-4BF0-A077-BFC54AC5FE23", $"result.ToHumanizedLabel() = {result.ToHumanizedLabel()}");

            var debugHelperOptions = DebugHelperOptions.FromHumanizedOptions();
            debugHelperOptions.ShowPrefixesForConceptLikeIdentifier = false;

            _logger.Info("CEBB8003-FCFC-4B0C-A306-4AAEE8F96712", $"result.ToHumanizedLabel(debugHelperOptions) = {result.ToHumanizedLabel(debugHelperOptions)}");

            _logger.Info("562BB8A0-26E6-49B6-93D2-8072C0BA8818", $"result.ToHumanizedString() = {result.ToHumanizedString()}");
            _logger.Info("8CDF39B1-9118-41C2-8A05-0DBA7A33785E", $"result.ToHumanizedString(debugHelperOptions) = {result.ToHumanizedString(debugHelperOptions)}");

            //var lexer = new Lexer(str, _logger, LexerMode.StrongIdentifier);

            //Token token = null;

            //while ((token = lexer.GetToken()) != null)
            //{
            //    _logger.Info("1D8484CB-F8A6-4FF4-A341-053613BCFE1C", $"token = {token}");
            //}

            _logger.Info("566203D2-3F3E-4D35-9195-CAB1CD94C8F7", $"End {nameof(Case20)}");
        }

        private void Case19()
        {
            _logger.Info("6C580D9A-348A-4DF9-92D5-748EC4A2C2BC", $"Begin {nameof(Case19)}");

            var str = "number[∞]";

            _logger.Info("396F028A-2866-49CC-89F3-9B088F11B1ED", $"str = '{str}'");

            var parserContext = new InternalParserCoreContext(str, _logger, LexerMode.StrongIdentifier);

            var parser = new StrongIdentifierValueParser(parserContext);
            parser.Run();

            var result = parser.Result;

            _logger.Info("2C61516A-7AAF-47BE-84EB-BAFFC0211171", $"result = {result}");
            _logger.Info("86FE1940-1E03-4E23-A4A3-7C59E8728224", $"result.ToHumanizedLabel() = {result.ToHumanizedLabel()}");

            var debugHelperOptions = DebugHelperOptions.FromHumanizedOptions();
            debugHelperOptions.ShowPrefixesForConceptLikeIdentifier = false;

            _logger.Info("8876F77A-8147-48BB-A80A-7B60034DACFE", $"result.ToHumanizedLabel(debugHelperOptions) = {result.ToHumanizedLabel(debugHelperOptions)}");

            _logger.Info("22BA2FEC-9DE7-4128-B4AB-C83918FBFE80", $"result.ToHumanizedString() = {result.ToHumanizedString()}");
            _logger.Info("D948B30C-A1AD-4382-9EFE-72EE0CB4F7E1", $"result.ToHumanizedString(debugHelperOptions) = {result.ToHumanizedString(debugHelperOptions)}");

            //var lexer = new Lexer(str, _logger, LexerMode.StrongIdentifier);

            //Token token = null;

            //while ((token = lexer.GetToken()) != null)
            //{
            //    _logger.Info("FA56E3E9-E9B1-4DE3-8E8F-808F516FDEA1", $"token = {token}");
            //}

            _logger.Info("9CCBA3AD-4D9E-4773-BCB6-F44DECF6F4AC", $"End {nameof(Case19)}");
        }

        private void Case18()
        {
            _logger.Info("3C6C0269-A462-418B-B44F-2486E7EE1564", $"Begin {nameof(Case18)}");

            var str = "number[*]";

            _logger.Info("2408E45E-6E97-4FC3-8683-89EFC3FC4C59", $"str = '{str}'");

            var parserContext = new InternalParserCoreContext(str, _logger, LexerMode.StrongIdentifier);

            var parser = new StrongIdentifierValueParser(parserContext);
            parser.Run();

            var result = parser.Result;

            _logger.Info("94B50052-56AE-4F7C-B359-2796455AA64B", $"result = {result}");
            _logger.Info("DDC6F23F-A54A-460C-9CF6-42BCBD920ADA", $"result.ToHumanizedLabel() = {result.ToHumanizedLabel()}");

            var debugHelperOptions = DebugHelperOptions.FromHumanizedOptions();
            debugHelperOptions.ShowPrefixesForConceptLikeIdentifier = false;

            _logger.Info("BB3AE72D-A0EB-4ED1-87A8-D67352EE616B", $"result.ToHumanizedLabel(debugHelperOptions) = {result.ToHumanizedLabel(debugHelperOptions)}");

            _logger.Info("F55EAE9A-A338-4375-BC9C-0BAC34936F5E", $"result.ToHumanizedString() = {result.ToHumanizedString()}");
            _logger.Info("3159ADEE-1E53-4EEA-8945-C0E031B61A46", $"result.ToHumanizedString(debugHelperOptions) = {result.ToHumanizedString(debugHelperOptions)}");

            //var lexer = new Lexer(str, _logger, LexerMode.StrongIdentifier);

            //Token token = null;

            //while ((token = lexer.GetToken()) != null)
            //{
            //    _logger.Info("2F410076-8A01-4B50-A836-0554D74CFA81", $"token = {token}");
            //}

            _logger.Info("EEA93D23-E4AF-4B8F-8411-2F96A3637974", $"End {nameof(Case18)}");
        }

        private void Case17()
        {
            _logger.Info("E4900041-D5F2-4CDA-AD1C-54FEAC848A50", $"Begin {nameof(Case17)}");

            var str = "number[]";

            _logger.Info("2B5EADF7-293B-456E-A703-E9DC03F256F5", $"str = '{str}'");

            var parserContext = new InternalParserCoreContext(str, _logger, LexerMode.StrongIdentifier);

            var parser = new StrongIdentifierValueParser(parserContext);
            parser.Run();

            var result = parser.Result;

            _logger.Info("AFFB83E0-A5FF-4EEF-90B7-15D710211BC8", $"result = {result}");
            _logger.Info("A828FA55-1F22-4BAC-82BD-FEA871EDBF9A", $"result.ToHumanizedLabel() = {result.ToHumanizedLabel()}");

            var debugHelperOptions = DebugHelperOptions.FromHumanizedOptions();
            debugHelperOptions.ShowPrefixesForConceptLikeIdentifier = false;

            _logger.Info("1978916E-9A87-4F37-87A9-07B7800CD52A", $"result.ToHumanizedLabel(debugHelperOptions) = {result.ToHumanizedLabel(debugHelperOptions)}");

            _logger.Info("9704D94D-A2A3-4A1B-BF5D-664499D3EAE9", $"result.ToHumanizedString() = {result.ToHumanizedString()}");
            _logger.Info("BCE3B72A-0B65-46FB-924B-F38B9F54800F", $"result.ToHumanizedString(debugHelperOptions) = {result.ToHumanizedString(debugHelperOptions)}");

            //var lexer = new Lexer(str, _logger, LexerMode.StrongIdentifier);

            //Token token = null;

            //while ((token = lexer.GetToken()) != null)
            //{
            //    _logger.Info("9165610D-5CEA-48D0-860E-8AA26A20EB6B", $"token = {token}");
            //}

            _logger.Info("BCB00EDD-5FB6-4B98-B212-85CDE49A6F01", $"End {nameof(Case17)}");
        }

        private void Case16()
        {
            _logger.Info("5BD069A3-E35D-46E0-A374-EE6FB5E93581", $"Begin {nameof(Case16)}");

            var str = "number[5]";

            _logger.Info("797C8BA7-79F1-4DB4-BC02-AA59B6AD5AB3", $"str = '{str}'");

            var parserContext = new InternalParserCoreContext(str, _logger, LexerMode.StrongIdentifier);

            var parser = new StrongIdentifierValueParser(parserContext);
            parser.Run();

            var result = parser.Result;

            _logger.Info("0B9D7CE5-265C-401E-8237-AFECFDE73D11", $"result = {result}");
            _logger.Info("D9B8B4B6-D4F7-4065-BD29-732AF846E299", $"result.ToHumanizedLabel() = {result.ToHumanizedLabel()}");

            var debugHelperOptions = DebugHelperOptions.FromHumanizedOptions();
            debugHelperOptions.ShowPrefixesForConceptLikeIdentifier = false;

            _logger.Info("D1EEE8EB-2E46-4311-9058-AC7ACD4B21F7", $"result.ToHumanizedLabel(debugHelperOptions) = {result.ToHumanizedLabel(debugHelperOptions)}");

            _logger.Info("FE41C9C8-7562-4D15-818E-BFC890D0C46D", $"result.ToHumanizedString() = {result.ToHumanizedString()}");
            _logger.Info("8F72F0ED-2B2B-4106-A3EA-2C54E44F5460", $"result.ToHumanizedString(debugHelperOptions) = {result.ToHumanizedString(debugHelperOptions)}");

            //var lexer = new Lexer(str, _logger, LexerMode.StrongIdentifier);

            //Token token = null;

            //while ((token = lexer.GetToken()) != null)
            //{
            //    _logger.Info("8C0FE1D8-D29D-40E1-86FD-ECF3593FE044", $"token = {token}");
            //}

            _logger.Info("B9133ABA-DD9B-4DB9-BD8F-D6320582913C", $"End {nameof(Case16)}");
        }

        private void Case15()
        {
            _logger.Info("27BE6F69-E5DA-4566-A13C-DCD5AA54EF25", $"Begin {nameof(Case15)}");

            var str = "global(politics)::dog (animal (alive))";

            _logger.Info("B95CA8C8-7921-4C3B-BAF9-3A9AE6C7C5F1", $"str = '{str}'");

            var parserContext = new InternalParserCoreContext(str, _logger, LexerMode.StrongIdentifier);

            var parser = new StrongIdentifierValueParser(parserContext);
            parser.Run();

            var result = parser.Result;

            _logger.Info("F397E863-CF82-4E16-9C35-7B0C968BAC93", $"result = {result}");
            _logger.Info("F12A1488-B85D-4ACB-BEA0-4A329925C7E3", $"result.ToHumanizedLabel() = {result.ToHumanizedLabel()}");

            var debugHelperOptions = DebugHelperOptions.FromHumanizedOptions();
            debugHelperOptions.ShowPrefixesForConceptLikeIdentifier = false;

            _logger.Info("DDDD592C-1002-4D0C-A345-05F36131500F", $"result.ToHumanizedLabel(debugHelperOptions) = {result.ToHumanizedLabel(debugHelperOptions)}");

            _logger.Info("EAF8792C-C0F9-4E8A-AA1D-343063A5C77F", $"result.ToHumanizedString() = {result.ToHumanizedString()}");
            _logger.Info("F9541FF4-F2EE-452C-A161-4E1837D115D6", $"result.ToHumanizedString(debugHelperOptions) = {result.ToHumanizedString(debugHelperOptions)}");

            //var lexer = new Lexer(str, _logger, LexerMode.StrongIdentifier);

            //Token token = null;

            //while ((token = lexer.GetToken()) != null)
            //{
            //    _logger.Info("D3999E58-C7DF-4BE6-8F1A-8CB30198111F", $"token = {token}");
            //}

            _logger.Info("794E090B-849C-4585-BCEF-3B56DF188631", $"End {nameof(Case15)}");
        }

        private void Case14()
        {
            _logger.Info("FBC8B0C9-C7F3-4DA2-B2D5-58F2249AEAFD", $"Begin {nameof(Case14)}");

            var str = "global::##Prop1";

            _logger.Info("E47B42AA-7EF4-4F10-A8DB-AAF232414610", $"str = '{str}'");

            var parserContext = new InternalParserCoreContext(str, _logger, LexerMode.StrongIdentifier);

            var parser = new StrongIdentifierValueParser(parserContext);
            parser.Run();

            var result = parser.Result;

            _logger.Info("7CDA1037-1000-4B8A-9B2B-D3000FBE9F57", $"result = {result}");
            _logger.Info("8AE917F3-E0DE-4785-8215-328108554674", $"result.ToHumanizedLabel() = {result.ToHumanizedLabel()}");

            var debugHelperOptions = DebugHelperOptions.FromHumanizedOptions();
            debugHelperOptions.ShowPrefixesForConceptLikeIdentifier = false;

            _logger.Info("4C51C220-5378-4A48-8682-665785C665AA", $"result.ToHumanizedLabel(debugHelperOptions) = {result.ToHumanizedLabel(debugHelperOptions)}");

            _logger.Info("60FF3A52-977E-4568-B5ED-A9B53A2E0357", $"result.ToHumanizedString() = {result.ToHumanizedString()}");
            _logger.Info("C97B40D1-2DE5-4DE3-8019-DEB3FC676153", $"result.ToHumanizedString(debugHelperOptions) = {result.ToHumanizedString(debugHelperOptions)}");

            //var lexer = new Lexer(str, _logger, LexerMode.StrongIdentifier);

            //Token token = null;

            //while ((token = lexer.GetToken()) != null)
            //{
            //    _logger.Info("8689EB54-64DA-4462-B9B7-4BA51CEC4E10", $"token = {token}");
            //}

            _logger.Info("F5CF20FD-63DF-4A2E-B839-9FB9A166D875", $"End {nameof(Case14)}");
        }

        private void Case13()
        {
            _logger.Info("B95B9C29-097D-4919-B94E-CAFEDEE62DB1", $"Begin {nameof(Case13)}");

            var str = "global::Prop1";

            _logger.Info("9900B991-57DD-4963-A2BD-23BC257BA431", $"str = '{str}'");

            var parserContext = new InternalParserCoreContext(str, _logger, LexerMode.StrongIdentifier);

            var parser = new StrongIdentifierValueParser(parserContext);
            parser.Run();

            var result = parser.Result;

            _logger.Info("BB7E2D65-5947-4545-9A58-9B710D1AEB78", $"result = {result}");
            _logger.Info("D25A1045-84C8-475A-8CCB-624146A2DF62", $"result.ToHumanizedLabel() = {result.ToHumanizedLabel()}");

            var debugHelperOptions = DebugHelperOptions.FromHumanizedOptions();
            debugHelperOptions.ShowPrefixesForConceptLikeIdentifier = false;

            _logger.Info("B9639017-FD7D-460E-9AC1-F678CD2597AA", $"result.ToHumanizedLabel(debugHelperOptions) = {result.ToHumanizedLabel(debugHelperOptions)}");

            _logger.Info("382DED27-D987-4B0B-8C38-1EBF27199F0D", $"result.ToHumanizedString() = {result.ToHumanizedString()}");
            _logger.Info("737524E6-B4B7-4E99-96C8-10D844CA879C", $"result.ToHumanizedString(debugHelperOptions) = {result.ToHumanizedString(debugHelperOptions)}");

            //var lexer = new Lexer(str, _logger, LexerMode.StrongIdentifier);

            //Token token = null;

            //while ((token = lexer.GetToken()) != null)
            //{
            //    _logger.Info("3E42C8BF-FA63-43E5-B58F-4EC15D10B479", $"token = {token}");
            //}

            _logger.Info("32D42830-E329-4970-A744-3463B8638AC3", $"End {nameof(Case13)}");
        }

        private void Case12()
        {
            _logger.Info("B6D7C0EE-FF1C-4632-8F73-43BAA509046C", $"Begin {nameof(Case12)}");

            var str = "@:`small`";

            _logger.Info("00BFEA04-11BC-4D7D-A734-67F00DBF274C", $"str = '{str}'");

            var oldIdentifier = NameHelper.CreateName(str);

            _logger.Info("CF1AFD01-3EB5-4FA0-AA6F-8660BDD03C7B", $"oldIdentifier = {oldIdentifier}");

            var parserContext = new InternalParserCoreContext(str, _logger, LexerMode.StrongIdentifier);

            var parser = new StrongIdentifierValueParser(parserContext);
            parser.Run();

            var result = parser.Result;

            _logger.Info("E982B6A6-8D9B-4ACB-A8BF-25A416F6CB20", $"result = {result}");
            _logger.Info("2A6B6313-4C9C-4658-B79E-A13EF1D92C1D", $"result.ToHumanizedLabel() = {result.ToHumanizedLabel()}");

            var debugHelperOptions = DebugHelperOptions.FromHumanizedOptions();
            debugHelperOptions.ShowPrefixesForConceptLikeIdentifier = false;

            _logger.Info("530D9221-AB82-4483-8A35-762A6B8E6C74", $"result.ToHumanizedLabel(debugHelperOptions) = {result.ToHumanizedLabel(debugHelperOptions)}");

            _logger.Info("60300145-F7EB-4B95-9FE0-BF67C6AB2307", $"result.ToHumanizedString() = {result.ToHumanizedString()}");
            _logger.Info("768F1E4E-6A37-4031-B3CA-E463E0357285", $"result.ToHumanizedString(debugHelperOptions) = {result.ToHumanizedString(debugHelperOptions)}");

            //var lexer = new Lexer(str, _logger, LexerMode.StrongIdentifier);

            //Token token = null;

            //while ((token = lexer.GetToken()) != null)
            //{
            //    _logger.Info("F06803C2-0BA4-4786-BC40-D65B88194898", $"token = {token}");
            //}

            _logger.Info("CBE33CB3-38CC-4A35-816C-19F8CE90B1D9", $"End {nameof(Case12)}");
        }

        private void Case12_a()
        {
            _logger.Info("4823C893-F715-4BCC-AFAD-51746D138BDC", $"Begin {nameof(Case12_a)}");

            var str = "@:`small dog`";

            _logger.Info("F89FDC9A-422F-488B-9B12-4217050718C0", $"str = '{str}'");

            var oldIdentifier = NameHelper.CreateName(str);

            _logger.Info("9B0C0273-477C-4013-9570-634012B18B57", $"oldIdentifier = {oldIdentifier}");

            var parserContext = new InternalParserCoreContext(str, _logger, LexerMode.StrongIdentifier);

            var parser = new StrongIdentifierValueParser(parserContext);
            parser.Run();

            var result = parser.Result;

            _logger.Info("43A60565-9579-412D-8D8C-13793E430D30", $"result = {result}");
            _logger.Info("1D327D44-1B0E-4B45-8C0F-F50C61DE5E2B", $"result.ToHumanizedLabel() = {result.ToHumanizedLabel()}");

            var debugHelperOptions = DebugHelperOptions.FromHumanizedOptions();
            debugHelperOptions.ShowPrefixesForConceptLikeIdentifier = false;

            _logger.Info("693D36F9-D2B6-4FBB-A893-09412D286F58", $"result.ToHumanizedLabel(debugHelperOptions) = {result.ToHumanizedLabel(debugHelperOptions)}");

            _logger.Info("40E38B00-5796-4D57-AF3A-134A32BCDCC6", $"result.ToHumanizedString() = {result.ToHumanizedString()}");
            _logger.Info("9775A99C-3528-4CE3-8A37-DE7F79F295E8", $"result.ToHumanizedString(debugHelperOptions) = {result.ToHumanizedString(debugHelperOptions)}");

            //var lexer = new Lexer(str, _logger, LexerMode.StrongIdentifier);

            //Token token = null;

            //while ((token = lexer.GetToken()) != null)
            //{
            //    _logger.Info("6B4D33BA-4A82-45FE-ABC1-B87F6DAE21EF", $"token = {token}");
            //}

            _logger.Info("F6911407-5FBB-454F-877E-08F268227DCD", $"End {nameof(Case12_a)}");
        }

        private void Case11()
        {
            _logger.Info("DE8FAA93-39B2-4C34-AF72-EC87950B5207", $"Begin {nameof(Case11)}");

            var str = "@>`dog`";

            _logger.Info("DE752BF9-5F41-44B3-8665-16C103EF299C", $"str = '{str}'");

            var parserContext = new InternalParserCoreContext(str, _logger, LexerMode.StrongIdentifier);

            var parser = new StrongIdentifierValueParser(parserContext);
            parser.Run();

            var result = parser.Result;

            _logger.Info("529490E8-2E41-45B5-8F52-BC4302CD4C97", $"result = {result}");
            _logger.Info("3FC750F9-0FF9-4D14-8788-106AF7C8A5EE", $"result.ToHumanizedLabel() = {result.ToHumanizedLabel()}");

            var debugHelperOptions = DebugHelperOptions.FromHumanizedOptions();
            debugHelperOptions.ShowPrefixesForConceptLikeIdentifier = false;

            _logger.Info("E87CD547-17A8-4EC9-B608-B6C2B6DE8FCA", $"result.ToHumanizedLabel(debugHelperOptions) = {result.ToHumanizedLabel(debugHelperOptions)}");

            _logger.Info("FED7ACCE-7834-49C3-AA5D-4F54E04A09F5", $"result.ToHumanizedString() = {result.ToHumanizedString()}");
            _logger.Info("183B679B-50D0-42BF-B6A4-BD2B2A79B892", $"result.ToHumanizedString(debugHelperOptions) = {result.ToHumanizedString(debugHelperOptions)}");

            //var lexer = new Lexer(str, _logger, LexerMode.StrongIdentifier);

            //Token token = null;

            //while ((token = lexer.GetToken()) != null)
            //{
            //    _logger.Info("0DB9E800-39A1-4C76-ADB2-684C4F2E6D55", $"token = {token}");
            //}

            _logger.Info("45268496-F73B-45BB-A996-88F55FC4F16A", $"End {nameof(Case11)}");
        }

        private void Case10()
        {
            _logger.Info("F3B108D6-8EBA-4B28-B07F-D5A8D701DD9E", $"Begin {nameof(Case10)}");

            var str = "@@`dog`";

            _logger.Info("74820A91-066D-4C66-9D71-7053103F4CA1", $"str = '{str}'");

            var parserContext = new InternalParserCoreContext(str, _logger, LexerMode.StrongIdentifier);

            var parser = new StrongIdentifierValueParser(parserContext);
            parser.Run();

            var result = parser.Result;

            _logger.Info("65FB645A-BD92-4A4A-9266-17B0277941B0", $"result = {result}");
            _logger.Info("CCD5CD7A-4C9F-47F8-BB9F-2C18D805E4F1", $"result.ToHumanizedLabel() = {result.ToHumanizedLabel()}");

            var debugHelperOptions = DebugHelperOptions.FromHumanizedOptions();
            debugHelperOptions.ShowPrefixesForConceptLikeIdentifier = false;

            _logger.Info("54A19545-A31C-40BB-817A-EA6BB7B4B06A", $"result.ToHumanizedLabel(debugHelperOptions) = {result.ToHumanizedLabel(debugHelperOptions)}");

            _logger.Info("00CE50BC-8276-4053-B91C-737A9E164B59", $"result.ToHumanizedString() = {result.ToHumanizedString()}");
            _logger.Info("9999DF83-F3A4-414D-8A3E-0FA55CE8A9D7", $"result.ToHumanizedString(debugHelperOptions) = {result.ToHumanizedString(debugHelperOptions)}");

            //var lexer = new Lexer(str, _logger, LexerMode.StrongIdentifier);

            //Token token = null;

            //while ((token = lexer.GetToken()) != null)
            //{
            //    _logger.Info("9E1D43CC-68C1-48C9-B558-C00BBC0A0AA3", $"token = {token}");
            //}

            _logger.Info("6F7CCF66-5168-4CDB-AD43-AEBBA4D14715", $"End {nameof(Case10)}");
        }

        private void Case9()
        {
            _logger.Info("4D2DE69D-5CA0-405C-9323-2ABF3D4D3A63", $"Begin {nameof(Case9)}");

            var str = "@`dog`";

            _logger.Info("5979A9D4-1E06-42C3-B56E-A575D18DD1B2", $"str = '{str}'");

            var parserContext = new InternalParserCoreContext(str, _logger, LexerMode.StrongIdentifier);

            var parser = new StrongIdentifierValueParser(parserContext);
            parser.Run();

            var result = parser.Result;

            _logger.Info("F795C123-5CAB-409E-958F-5121A5237BEA", $"result = {result}");
            _logger.Info("768AB150-4F97-4093-8284-B9D1257B9A7E", $"result.ToHumanizedLabel() = {result.ToHumanizedLabel()}");

            var debugHelperOptions = DebugHelperOptions.FromHumanizedOptions();
            debugHelperOptions.ShowPrefixesForConceptLikeIdentifier = false;

            _logger.Info("3C5B1918-5AE1-435C-8D4F-AFB52E7A31B3", $"result.ToHumanizedLabel(debugHelperOptions) = {result.ToHumanizedLabel(debugHelperOptions)}");

            _logger.Info("00B4E0AF-E92B-41ED-B73F-AAABF67D3B0A", $"result.ToHumanizedString() = {result.ToHumanizedString()}");
            _logger.Info("82C5082E-2EF8-4131-A823-5FB9CB5949DD", $"result.ToHumanizedString(debugHelperOptions) = {result.ToHumanizedString(debugHelperOptions)}");

            //var lexer = new Lexer(str, _logger, LexerMode.StrongIdentifier);

            //Token token = null;

            //while ((token = lexer.GetToken()) != null)
            //{
            //    _logger.Info("34FDEE2C-989D-433C-95BB-9A948853B373", $"token = {token}");
            //}

            _logger.Info("CA4DFB26-7C7F-4EFD-ACB7-7B0FF4ECFAC8", $"End {nameof(Case9)}");
        }

        private void Case8()
        {
            _logger.Info("56193D5E-11B8-4832-969A-A839AE22E622", $"Begin {nameof(Case8)}");

            var str = "$`dog`";

            _logger.Info("5FD716CD-6A41-44EA-A40C-FCFFF266BBF3", $"str = '{str}'");

            var parserContext = new InternalParserCoreContext(str, _logger, LexerMode.StrongIdentifier);

            var parser = new StrongIdentifierValueParser(parserContext);
            parser.Run();

            var result = parser.Result;

            _logger.Info("68CFACF1-E364-4D34-AACF-C69EF4C3C20A", $"result = {result}");
            _logger.Info("5B71D156-4357-48FA-8A41-5A42927C67CF", $"result.ToHumanizedLabel() = {result.ToHumanizedLabel()}");

            var debugHelperOptions = DebugHelperOptions.FromHumanizedOptions();
            debugHelperOptions.ShowPrefixesForConceptLikeIdentifier = false;

            _logger.Info("821B6383-D676-4FB3-9E49-645282599020", $"result.ToHumanizedLabel(debugHelperOptions) = {result.ToHumanizedLabel(debugHelperOptions)}");

            _logger.Info("6092F7B0-D147-4050-BFA8-4BAB98B1B502", $"result.ToHumanizedString() = {result.ToHumanizedString()}");
            _logger.Info("7129E251-FA49-4DA9-A69B-EEBDCFC35302", $"result.ToHumanizedString(debugHelperOptions) = {result.ToHumanizedString(debugHelperOptions)}");

            //var lexer = new Lexer(str, _logger, LexerMode.StrongIdentifier);

            //Token token = null;

            //while ((token = lexer.GetToken()) != null)
            //{
            //    _logger.Info("315947C3-DFA6-453F-8B29-B793F4B02E9B", $"token = {token}");
            //}

            _logger.Info("84B14F5F-DAA0-4F06-919D-192A51644FDF", $"End {nameof(Case8)}");
        }

        private void Case8_a()
        {
            _logger.Info("81CF2E2D-F886-4D38-9391-282D3B8F1D37", $"Begin {nameof(Case8_a)}");

            var str = "$_";

            _logger.Info("3A755EC1-2FF2-4C7F-A4B9-31BC544ED3DA", $"str = '{str}'");

            var parserContext = new InternalParserCoreContext(str, _logger, LexerMode.StrongIdentifier);

            var parser = new StrongIdentifierValueParser(parserContext);
            parser.Run();

            var result = parser.Result;

            _logger.Info("2917AD2E-906B-411B-A283-087FE6F6B7EA", $"result = {result}");
            _logger.Info("E3AA932D-DAAF-4704-90C8-F86A574E5E3B", $"result.ToHumanizedLabel() = {result.ToHumanizedLabel()}");

            var debugHelperOptions = DebugHelperOptions.FromHumanizedOptions();
            debugHelperOptions.ShowPrefixesForConceptLikeIdentifier = false;

            _logger.Info("76F3C5F1-DBC0-46CE-8E7F-F9E300917D06", $"result.ToHumanizedLabel(debugHelperOptions) = {result.ToHumanizedLabel(debugHelperOptions)}");

            _logger.Info("A872810F-4A07-4829-8E7D-2834581B9FEC", $"result.ToHumanizedString() = {result.ToHumanizedString()}");
            _logger.Info("9A9783AD-251A-4D73-A456-021D9F344783", $"result.ToHumanizedString(debugHelperOptions) = {result.ToHumanizedString(debugHelperOptions)}");

            //var lexer = new Lexer(str, _logger, LexerMode.StrongIdentifier);

            //Token token = null;

            //while ((token = lexer.GetToken()) != null)
            //{
            //    _logger.Info("916C228C-8AA3-4139-9BBC-10001250D352", $"token = {token}");
            //}

            _logger.Info("FE54DC9D-47B2-452B-BF71-6DB40D988221", $"End {nameof(Case8_a)}");
        }

        private void Case7()
        {
            _logger.Info("EE2D7A8A-E889-4580-8AED-7AF73AB5098A", $"Begin {nameof(Case7)}");

            var str = "#|`dog`";

            _logger.Info("E3486139-EBBC-4885-ABD3-AE7D8AED38BE", $"str = '{str}'");

            var parserContext = new InternalParserCoreContext(str, _logger, LexerMode.StrongIdentifier);

            var parser = new StrongIdentifierValueParser(parserContext);
            parser.Run();

            var result = parser.Result;

            _logger.Info("3D1AF7C2-616F-47F4-87AE-A03C712D73E7", $"result = {result}");
            _logger.Info("EF3F6581-3830-4EEF-A83F-B9B8A7E4D2AA", $"result.ToHumanizedLabel() = {result.ToHumanizedLabel()}");

            var debugHelperOptions = DebugHelperOptions.FromHumanizedOptions();
            debugHelperOptions.ShowPrefixesForConceptLikeIdentifier = false;

            _logger.Info("77BA601D-466E-44A4-A683-B542451AFDC6", $"result.ToHumanizedLabel(debugHelperOptions) = {result.ToHumanizedLabel(debugHelperOptions)}");

            _logger.Info("F3A65A73-D94C-4FB8-A656-6AD0E242205C", $"result.ToHumanizedString() = {result.ToHumanizedString()}");
            _logger.Info("AD433838-59DB-4075-8D6E-5349493DDCE5", $"result.ToHumanizedString(debugHelperOptions) = {result.ToHumanizedString(debugHelperOptions)}");

            //var lexer = new Lexer(str, _logger, LexerMode.StrongIdentifier);

            //Token token = null;

            //while ((token = lexer.GetToken()) != null)
            //{
            //    _logger.Info("2C700970-0D98-4B14-B0BA-E1983EA8F6F3", $"token = {token}");
            //}

            _logger.Info("EFD2B007-25B3-421F-A391-08F89DBE940A", $"End {nameof(Case7)}");
        }

        private void Case6()
        {
            _logger.Info("68E1C069-DDAE-4B10-94B5-38116905510F", $"Begin {nameof(Case6)}");

            var str = "#^`dog`";

            _logger.Info("A8C58289-A166-4FBC-B94F-0ABC6DBFEBDA", $"str = '{str}'");

            var parserContext = new InternalParserCoreContext(str, _logger, LexerMode.StrongIdentifier);

            var parser = new StrongIdentifierValueParser(parserContext);
            parser.Run();

            var result = parser.Result;

            _logger.Info("FBD2486C-7F70-4C38-801B-ABC6CAB1E071", $"result = {result}");
            _logger.Info("E72644B1-20E2-4773-9776-C728AC53F6A5", $"result.ToHumanizedLabel() = {result.ToHumanizedLabel()}");

            var debugHelperOptions = DebugHelperOptions.FromHumanizedOptions();
            debugHelperOptions.ShowPrefixesForConceptLikeIdentifier = false;

            _logger.Info("F1F3D599-AFC9-4B3A-B00C-47F605625CED", $"result.ToHumanizedLabel(debugHelperOptions) = {result.ToHumanizedLabel(debugHelperOptions)}");

            _logger.Info("8FD0EAF4-1AA3-4C4E-950E-A099E448A58D", $"result.ToHumanizedString() = {result.ToHumanizedString()}");
            _logger.Info("0AE1CC62-DFF7-405B-9393-563F1FA9F80A", $"result.ToHumanizedString(debugHelperOptions) = {result.ToHumanizedString(debugHelperOptions)}");

            //var lexer = new Lexer(str, _logger, LexerMode.StrongIdentifier);

            //Token token = null;

            //while ((token = lexer.GetToken()) != null)
            //{
            //    _logger.Info("E7C6D716-0E31-49A8-B39B-A9687D567ECE", $"token = {token}");
            //}

            _logger.Info("10BE6164-1563-472D-BD32-A2AD2FF84CF9", $"End {nameof(Case6)}");
        }

        private void Case5()
        {
            _logger.Info("123337F1-DB4E-4076-8509-815AB3CE7BBB", $"Begin {nameof(Case5)}");

            var str = "##@`dog`";

            _logger.Info("9463EB7D-D873-4FAF-A560-396D285CB8A2", $"str = '{str}'");

            var parserContext = new InternalParserCoreContext(str, _logger, LexerMode.StrongIdentifier);

            var parser = new StrongIdentifierValueParser(parserContext);
            parser.Run();

            var result = parser.Result;

            _logger.Info("3D6FE328-9611-43E4-8FCD-6AAD23AB352D", $"result = {result}");
            _logger.Info("26216FBC-9F06-4E57-B336-27E7EE0F910D", $"result.ToHumanizedLabel() = {result.ToHumanizedLabel()}");

            var debugHelperOptions = DebugHelperOptions.FromHumanizedOptions();
            debugHelperOptions.ShowPrefixesForConceptLikeIdentifier = false;

            _logger.Info("CC273B13-1993-409B-AE57-DAB9B5CB2445", $"result.ToHumanizedLabel(debugHelperOptions) = {result.ToHumanizedLabel(debugHelperOptions)}");

            _logger.Info("47047024-E91C-482D-BBA3-97BE6948B251", $"result.ToHumanizedString() = {result.ToHumanizedString()}");
            _logger.Info("1ADA4958-9CBF-4C65-B99F-C206F5D7E666", $"result.ToHumanizedString(debugHelperOptions) = {result.ToHumanizedString(debugHelperOptions)}");

            //var lexer = new Lexer(str, _logger, LexerMode.StrongIdentifier);

            //Token token = null;

            //while ((token = lexer.GetToken()) != null)
            //{
            //    _logger.Info("E00149A2-1E95-45E6-9C95-4CC1007ADD7D", $"token = {token}");
            //}

            _logger.Info("AEFADC7F-F08F-419E-9F0D-090070C11533", $"End {nameof(Case5)}");
        }

        private void Case5_a()
        {
            _logger.Info("7D8616C1-EC72-40C9-828F-AC4894FD16E2", $"Begin {nameof(Case5_a)}");

            var str = "##@";

            _logger.Info("215EA535-7195-448A-8165-D666A064D1F0", $"str = '{str}'");

            var parserContext = new InternalParserCoreContext(str, _logger, LexerMode.StrongIdentifier);

            var parser = new StrongIdentifierValueParser(parserContext);
            parser.Run();

            var result = parser.Result;

            _logger.Info("9D0EC83A-9032-412A-A920-73C180F02822", $"result = {result}");
            _logger.Info("4922D53B-0585-4BCF-BDFA-DA93C4627209", $"result.ToHumanizedLabel() = {result.ToHumanizedLabel()}");

            var debugHelperOptions = DebugHelperOptions.FromHumanizedOptions();
            debugHelperOptions.ShowPrefixesForConceptLikeIdentifier = false;

            _logger.Info("7278C59E-CC57-46B9-A990-737C3ED924F7", $"result.ToHumanizedLabel(debugHelperOptions) = {result.ToHumanizedLabel(debugHelperOptions)}");

            _logger.Info("4C1A5336-2EB1-4387-89AE-5C87FDB3880B", $"result.ToHumanizedString() = {result.ToHumanizedString()}");
            _logger.Info("46F3CB77-B828-46D0-99D0-A2DD72EDE78A", $"result.ToHumanizedString(debugHelperOptions) = {result.ToHumanizedString(debugHelperOptions)}");

            //var lexer = new Lexer(str, _logger, LexerMode.StrongIdentifier);

            //Token token = null;

            //while ((token = lexer.GetToken()) != null)
            //{
            //    _logger.Info("9D4D4091-770F-4B4F-A31A-C9B1F740039A", $"token = {token}");
            //}

            _logger.Info("50B68948-B095-4D25-933B-28310AE310A4", $"End {nameof(Case5_a)}");
        }

        private void Case4()
        {
            _logger.Info("694F8127-65F8-48D1-9796-37B6AD9D14CE", $"Begin {nameof(Case4)}");

            var str = "##`dog`";

            _logger.Info("43718E7B-15FB-4B5D-B8E8-2E20498FF78B", $"str = '{str}'");

            var parserContext = new InternalParserCoreContext(str, _logger, LexerMode.StrongIdentifier);

            var parser = new StrongIdentifierValueParser(parserContext);
            parser.Run();

            var result = parser.Result;

            _logger.Info("3BDB92B7-82CB-4964-800F-1E695FBE2B17", $"result = {result}");
            _logger.Info("2FD545A5-D78A-49BD-A894-29B249E50572", $"result.ToHumanizedLabel() = {result.ToHumanizedLabel()}");

            var debugHelperOptions = DebugHelperOptions.FromHumanizedOptions();
            debugHelperOptions.ShowPrefixesForConceptLikeIdentifier = false;

            _logger.Info("0A9AB3AB-AAE8-44E1-950E-7926F0D7465F", $"result.ToHumanizedLabel(debugHelperOptions) = {result.ToHumanizedLabel(debugHelperOptions)}");

            _logger.Info("85E90469-EE08-4D93-AC97-3938CCD657EC", $"result.ToHumanizedString() = {result.ToHumanizedString()}");
            _logger.Info("0C8488EA-F1AC-4D03-ADD8-52976AA63BB6", $"result.ToHumanizedString(debugHelperOptions) = {result.ToHumanizedString(debugHelperOptions)}");

            //var lexer = new Lexer(str, _logger, LexerMode.StrongIdentifier);

            //Token token = null;

            //while ((token = lexer.GetToken()) != null)
            //{
            //    _logger.Info("31425952-058B-4275-BB11-4D38ACAAAFA1", $"token = {token}");
            //}

            _logger.Info("3986889A-0F5C-42EB-BCC8-51FA085B39CB", $"End {nameof(Case4)}");
        }

        private void Case3()
        {
            _logger.Info("BBB3FFBF-7022-4FBE-88DD-AA642C114E64", $"Begin {nameof(Case3)}");

            var str = "#@`dog`";

            _logger.Info("61FF1248-2508-4927-8E78-CA3D0BB88AA1", $"str = '{str}'");

            var parserContext = new InternalParserCoreContext(str, _logger, LexerMode.StrongIdentifier);

            var parser = new StrongIdentifierValueParser(parserContext);
            parser.Run();

            var result = parser.Result;

            _logger.Info("F7C77FE6-F709-44E9-9537-5F402C00E697", $"result = {result}");
            _logger.Info("1C600AA2-C588-4066-9E95-0F12C9E65E25", $"result.ToHumanizedLabel() = {result.ToHumanizedLabel()}");

            var debugHelperOptions = DebugHelperOptions.FromHumanizedOptions();
            debugHelperOptions.ShowPrefixesForConceptLikeIdentifier = false;

            _logger.Info("3681B256-A3AD-48A4-AA53-7E6516C5F6C5", $"result.ToHumanizedLabel(debugHelperOptions) = {result.ToHumanizedLabel(debugHelperOptions)}");

            _logger.Info("3AE38765-4DD5-4C8C-8B89-9337A0506B22", $"result.ToHumanizedString() = {result.ToHumanizedString()}");
            _logger.Info("778C95D3-29A5-45B4-9F4E-B0AF8EE45774", $"result.ToHumanizedString(debugHelperOptions) = {result.ToHumanizedString(debugHelperOptions)}");

            //var lexer = new Lexer(str, _logger, LexerMode.StrongIdentifier);

            //Token token = null;

            //while ((token = lexer.GetToken()) != null)
            //{
            //    _logger.Info("51C31E42-96F7-4BE7-82A9-07647470CDF8", $"token = {token}");
            //}

            _logger.Info("9DED6582-F5AE-4CC9-9141-7FBC8474BC26", $"End {nameof(Case3)}");
        }

        private void Case3_a()
        {
            _logger.Info("E007C0C1-6711-4310-81BE-563B162B2CD5", $"Begin {nameof(Case3_a)}");

            var str = "#@";

            _logger.Info("7CDEA6FC-FF47-4653-9C22-336E741BF03F", $"str = '{str}'");

            var parserContext = new InternalParserCoreContext(str, _logger, LexerMode.StrongIdentifier);

            var parser = new StrongIdentifierValueParser(parserContext);
            parser.Run();

            var result = parser.Result;

            _logger.Info("DB2C14A3-0987-4B2A-8648-86EE0BB78508", $"result = {result}");
            _logger.Info("A91A0409-4B52-48A4-A085-DC9E233C2ADF", $"result.ToHumanizedLabel() = {result.ToHumanizedLabel()}");

            var debugHelperOptions = DebugHelperOptions.FromHumanizedOptions();
            debugHelperOptions.ShowPrefixesForConceptLikeIdentifier = false;

            _logger.Info("A72C9469-61ED-4599-A17B-416D60097F01", $"result.ToHumanizedLabel(debugHelperOptions) = {result.ToHumanizedLabel(debugHelperOptions)}");

            _logger.Info("6096EDA0-64D7-4370-8934-0C0DC5FB9CC0", $"result.ToHumanizedString() = {result.ToHumanizedString()}");
            _logger.Info("025C51F5-A778-4DFB-8818-620CA09FEC1D", $"result.ToHumanizedString(debugHelperOptions) = {result.ToHumanizedString(debugHelperOptions)}");

            //var lexer = new Lexer(str, _logger, LexerMode.StrongIdentifier);

            //Token token = null;

            //while ((token = lexer.GetToken()) != null)
            //{
            //    _logger.Info("F5B99A69-C957-42C0-800A-41371BAA063A", $"token = {token}");
            //}

            _logger.Info("AD89A6B2-1FBC-4C28-829E-C482FFF50FF6", $"End {nameof(Case3_a)}");
        }

        private void Case2()
        {
            _logger.Info("D2FF1F10-613F-4928-AFB5-AB32A53A79BA", $"Begin {nameof(Case2)}");

            var str = "#`dog`";

            _logger.Info("3197872C-C2FC-4B9C-8BE8-A6C64CE913F3", $"str = '{str}'");

            var parserContext = new InternalParserCoreContext(str, _logger, LexerMode.StrongIdentifier);

            var parser = new StrongIdentifierValueParser(parserContext);
            parser.Run();

            var result = parser.Result;

            _logger.Info("36EF1006-0E34-4133-9A47-AFFBFD0FC1B1", $"result = {result}");
            _logger.Info("E3AE0C81-06FF-4736-B912-91F1E445DD54", $"result.ToHumanizedLabel() = {result.ToHumanizedLabel()}");

            var debugHelperOptions = DebugHelperOptions.FromHumanizedOptions();
            debugHelperOptions.ShowPrefixesForConceptLikeIdentifier = false;

            _logger.Info("22FCC6AA-67C3-4013-A03E-CEA6E948BEA6", $"result.ToHumanizedLabel(debugHelperOptions) = {result.ToHumanizedLabel(debugHelperOptions)}");

            _logger.Info("007150D7-834B-4BFF-9F03-A6F8001CEC08", $"result.ToHumanizedString() = {result.ToHumanizedString()}");
            _logger.Info("73216B78-D527-4A75-83D7-3AF32A5D904A", $"result.ToHumanizedString(debugHelperOptions) = {result.ToHumanizedString(debugHelperOptions)}");

            //var lexer = new Lexer(str, _logger, LexerMode.StrongIdentifier);

            //Token token = null;

            //while((token = lexer.GetToken()) != null)
            //{
            //    _logger.Info("4E050FF3-E1CB-4362-B626-B5EAF07C9F4F", $"token = {token}");
            //}

            _logger.Info("0190AB27-C056-45D1-93D3-D492DC5E7160", $"End {nameof(Case2)}");
        }

        private void Case2_a()
        {
            _logger.Info("A87F96AD-22DD-4F1E-BE38-4048E8303136", $"Begin {nameof(Case2_a)}");

            var str = "#020ED339-6313-459A-900D-92F809CEBDC5";

            _logger.Info("4414D48C-078A-4E04-840D-F0DCA14F8DA2", $"str = '{str}'");

            var parserContext = new InternalParserCoreContext(str, _logger, LexerMode.StrongIdentifier);

            var parser = new StrongIdentifierValueParser(parserContext);
            parser.Run();

            var result = parser.Result;

            _logger.Info("4F20518C-2E6D-4F77-9940-1D1158124E5E", $"result = {result}");
            _logger.Info("CF17F9F8-84B7-481D-AC8F-BE867FBF9384", $"result.ToHumanizedLabel() = {result.ToHumanizedLabel()}");

            var debugHelperOptions = DebugHelperOptions.FromHumanizedOptions();
            debugHelperOptions.ShowPrefixesForConceptLikeIdentifier = false;

            _logger.Info("E79D8C40-F65C-49AE-B08C-78868E9C61AD", $"result.ToHumanizedLabel(debugHelperOptions) = {result.ToHumanizedLabel(debugHelperOptions)}");

            _logger.Info("1290373A-107E-4A87-8945-F144672D4822", $"result.ToHumanizedString() = {result.ToHumanizedString()}");
            _logger.Info("732BDCFD-E370-4563-B286-0FB51CDB56CE", $"result.ToHumanizedString(debugHelperOptions) = {result.ToHumanizedString(debugHelperOptions)}");

            //var lexer = new Lexer(str, _logger, LexerMode.StrongIdentifier);

            //Token token = null;

            //while((token = lexer.GetToken()) != null)
            //{
            //    _logger.Info("B0F82B2D-78A2-48D4-8DAA-D4310566DB8C", $"token = {token}");
            //}

            _logger.Info("77B9B0C2-3634-4101-AF04-2B556E1635FA", $"End {nameof(Case2_a)}");
        }

        private void Case2_b()
        {
            _logger.Info("E7EFAB4E-8EED-4754-96AB-3EE979D56628", $"Begin {nameof(Case2_b)}");

            var str = "#^91e029e7-6a4c-454b-b15b-323d2b5ff0a9";

            _logger.Info("46C1D3F8-75B7-4A0F-B75D-3F94040CB4D8", $"str = '{str}'");

            var parserContext = new InternalParserCoreContext(str, _logger, LexerMode.StrongIdentifier);

            var parser = new StrongIdentifierValueParser(parserContext);
            parser.Run();

            var result = parser.Result;

            _logger.Info("1F61F194-0901-4962-A215-FFE7399077D1", $"result = {result}");
            _logger.Info("FDF0A42E-2E72-450E-8476-74DBC0C6159E", $"result = {result.ToHumanizedLabel()}");

            var debugHelperOptions = DebugHelperOptions.FromHumanizedOptions();
            debugHelperOptions.ShowPrefixesForConceptLikeIdentifier = false;

            _logger.Info("440E65E4-0B13-488A-B509-63533B9C2545", $"result.ToHumanizedLabel(debugHelperOptions) = {result.ToHumanizedLabel(debugHelperOptions)}");

            _logger.Info("E8516898-16F0-4023-9B9E-0F852DC8C561", $"result.ToHumanizedString() = {result.ToHumanizedString()}");
            _logger.Info("B909DD2A-3F21-4723-9984-82E316130FF3", $"result.ToHumanizedString(debugHelperOptions) = {result.ToHumanizedString(debugHelperOptions)}");

            //var lexer = new Lexer(str, _logger, LexerMode.StrongIdentifier);

            //Token token = null;

            //while((token = lexer.GetToken()) != null)
            //{
            //    _logger.Info("ABEFD016-F6F6-44B4-81E9-96B685B5D92C", $"token = {token}");
            //}

            _logger.Info("23B882CC-D5B4-4385-8A37-A23C058BC63A", $"End {nameof(Case2_b)}");
        }

        private void Case1()
        {
            _logger.Info("8E57C361-E44A-4222-94FD-3A91B81350F7", $"Begin {nameof(Case1)}");

            var str = "`dog`";

            _logger.Info("D00EF65D-519F-4C02-A61F-E082D198EE8F", $"str = '{str}'");

            var parserContext = new InternalParserCoreContext(str, _logger, LexerMode.StrongIdentifier);

            var parser = new StrongIdentifierValueParser(parserContext);
            parser.Run();

            var result = parser.Result;

            _logger.Info("2B79E29D-4CFB-49DD-811C-4D338BE2BF6E", $"result = {result}");
            _logger.Info("9D7AE6E5-0462-4258-B89D-AC42312E3733", $"result.ToHumanizedLabel() = {result.ToHumanizedLabel()}");

            var debugHelperOptions = DebugHelperOptions.FromHumanizedOptions();
            debugHelperOptions.ShowPrefixesForConceptLikeIdentifier = false;

            _logger.Info("99AF80C0-10B4-4ED3-A8E0-AA4F98FF1281", $"result.ToHumanizedLabel(debugHelperOptions) = {result.ToHumanizedLabel(debugHelperOptions)}");

            _logger.Info("6C80DA77-2D54-4985-95A0-96757957A1CF", $"result.ToHumanizedString() = {result.ToHumanizedString()}");
            _logger.Info("92434CE1-72AA-4A81-B61D-D6609DF78DD1", $"result.ToHumanizedString(debugHelperOptions) = {result.ToHumanizedString(debugHelperOptions)}");

            //var lexer = new Lexer(str, _logger, LexerMode.StrongIdentifier);

            //Token token = null;

            //while ((token = lexer.GetToken()) != null)
            //{
            //    _logger.Info("4E050FF3-E1CB-4362-B626-B5EAF07C9F4F", $"token = {token}");
            //}

            _logger.Info("AA429DE7-8128-475B-B900-C1CFA3C5D178", $"End {nameof(Case1)}");
        }
    }
}
