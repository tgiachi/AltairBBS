namespace Altair.BBS.Api.Ansi;

public static class TelnetOpCodes
{
    public enum Commands
    {
        Iac = 0xFF,
        Se = 0xF0,
        Nop = 0xF1,
        Dm = 0xF2,
        Break = 0xF3,
        Ip = 0xF4,
        Ao = 0xF5,
        Ayt = 0xF6,
        EraseCharacter = 0xF7,
        EraseLine = 0xF8,
        GoAhead = 0xF9,
        SubBegin = 0xFA,
        Will = 0xFB,
        Wont = 0xFC,
        Do = 0xFD,
        Dont = 0xFE,
    }

    public enum SubNegotiationCommand
    {
        Is = 0x00,
        Send = 0x01,
        Info = 0x02,
        Var = 0x00,
        Value = 0x01,
        Esc = 0x02,
        UserVar = 0x3
    }

    public enum Options
    {
        TransmitBinary = 0x00,
        Echo = 0x01,
        SuppressGoAhead = 0x03,
        Status = 0x05,
        TimingMark = 0x06,
        TerminalType = 0x18,
        EndOrRecord = 0x19,
        NegotiateAboutWindowSize = 0x1F
    }

    public static string Se => "\xF0";
    public static string Nop => "\xF1";
    public static string Dm => "\xF2";
    public static string Break => "\xF3";
    public static string Ip => "\xF4";
    public static string Ao => "\xF5";
    public static string Ayt => "\x246";
    public static byte Will => 0xFB;
    public static byte Iac => 0xFF;
    public static byte Do => 0xFD;


    public static byte TerminalType => 0x18;

    public static byte Echo = 0x01;
}