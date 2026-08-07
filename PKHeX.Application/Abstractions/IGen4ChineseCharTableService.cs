using PKHeX.Core;

namespace PKHeX.Application.Abstractions;

/// <summary>
/// Decodes and encodes Nickname / OriginalTrainerName text for Generation 4 entities using the
/// extended character table published by the PokemonChineseTranslationRevise project for Chinese
/// fan-translation ROM patches (e.g. HGSS "官译") that repurpose character codes PKHeX's own
/// Generation 4 tables don't otherwise assign meaningfully — including the Korean Hangul block,
/// unused on Japanese-region carts — for Chinese glyphs.
/// </summary>
public interface IGen4ChineseCharTableService
{
    /// <summary>True if <paramref name="pk"/> uses the Generation 4 character encoding this table extends.</summary>
    bool IsSupported(PKM pk);

    string DecodeNickname(PKM pk);
    string DecodeOriginalTrainerName(PKM pk);

    void SetNickname(PKM pk, string value);
    void SetOriginalTrainerName(PKM pk, string value);
}
