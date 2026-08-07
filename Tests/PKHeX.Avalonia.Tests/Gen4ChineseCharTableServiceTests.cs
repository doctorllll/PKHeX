using PKHeX.Application.Services;
using PKHeX.Core;

namespace PKHeX.Avalonia.Tests;

/// <summary>
/// Covers <see cref="Gen4ChineseCharTableService"/>: the opt-in alternate character table for
/// Generation 4 saves from Chinese fan-translation ROM patches (e.g. HGSS "官译") that repurpose
/// character codes PKHeX's own Generation 4 tables don't otherwise assign meaningfully.
/// </summary>
public sealed class Gen4ChineseCharTableServiceTests
{
    private readonly Gen4ChineseCharTableService _service = new();

    [Fact]
    public void IsSupported_TrueForGen4_FalseOtherwise()
    {
        Assert.True(_service.IsSupported(new PK4()));
        Assert.False(_service.IsSupported(new PK9()));
    }

    [Fact]
    public void DecodesCustomCharacterCodesOutsideCoreTables()
    {
        var pk = new PK4();
        // 0x74A is outside Core's TableINT and inside the Korean Hangul block Core would otherwise
        // (mis)decode as a Hangul syllable; the fan-translation table repurposes it for Chinese.
        WriteCode(pk.NicknameTrash, 0x74A);

        // Vanilla Core decode produces a Hangul syllable, not the intended Chinese character.
        Assert.NotEqual('月', pk.Nickname[0]);

        Assert.Equal("月", _service.DecodeNickname(pk));
    }

    [Fact]
    public void SetNickname_RoundTripsThroughCustomTable()
    {
        var pk = new PK4();
        _service.SetNickname(pk, "月桂叶");

        Assert.Equal("月桂叶", _service.DecodeNickname(pk));
        // The bytes are genuinely custom-encoded: Core's own decode must not agree.
        Assert.NotEqual("月桂叶", pk.Nickname);
    }

    [Fact]
    public void SetOriginalTrainerName_RoundTripsThroughCustomTable()
    {
        var pk = new PK4();
        _service.SetOriginalTrainerName(pk, "近田");

        Assert.Equal("近田", _service.DecodeOriginalTrainerName(pk));
    }

    [Fact]
    public void OrdinaryAsciiNicknamesStillRoundTripViaCoreFallback()
    {
        var pk = new PK4();
        _service.SetNickname(pk, "PIKACHU");

        Assert.Equal("PIKACHU", _service.DecodeNickname(pk));
        // Plain ASCII isn't in the custom table, so Core's own decode must agree too.
        Assert.Equal("PIKACHU", pk.Nickname);
    }

    private static void WriteCode(Span<byte> trash, ushort code)
    {
        trash[0] = (byte)(code & 0xFF);
        trash[1] = (byte)(code >> 8);
        trash[2] = 0xFF; // terminator
        trash[3] = 0xFF;
    }
}
