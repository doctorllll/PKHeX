using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using PKHeX.Application.Abstractions;
using PKHeX.Core;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Application.Services;

/// <inheritdoc cref="IGen4ChineseCharTableService"/>
public sealed class Gen4ChineseCharTableService : IGen4ChineseCharTableService
{
    private const ushort Terminator = StringConverter4.Terminator;
    private const string ResourceName = "PKHeX.Application.Text.Gen4ChineseFanTranslation.CharTable.txt";

    private static readonly Lazy<IReadOnlyDictionary<ushort, char>> DecodeTableLazy = new(LoadDecodeTable);
    private static readonly Lazy<IReadOnlyDictionary<char, ushort>> EncodeTableLazy = new(LoadEncodeTable);

    public bool IsSupported(PKM pk) => pk is G4PKM;

    public string DecodeNickname(PKM pk) => Decode(pk.NicknameTrash);
    public string DecodeOriginalTrainerName(PKM pk) => Decode(pk.OriginalTrainerTrash);

    public void SetNickname(PKM pk, string value) => Encode(pk.NicknameTrash, value, pk.MaxStringLengthNickname);
    public void SetOriginalTrainerName(PKM pk, string value) => Encode(pk.OriginalTrainerTrash, value, pk.MaxStringLengthTrainer);

    private static string Decode(ReadOnlySpan<byte> trash)
    {
        var table = DecodeTableLazy.Value;
        Span<char> result = stackalloc char[trash.Length / 2];
        int ctr = 0;
        for (int i = 0; i + 1 < trash.Length; i += 2)
        {
            ushort value = (ushort)(trash[i] | (trash[i + 1] << 8));
            if (value == Terminator)
                break;
            char chr = table.TryGetValue(value, out var mapped) ? mapped : (char)StringConverter4Util.ConvertValue2CharG4(value);
            result[ctr++] = StringConverter4Util.NormalizeGenderSymbol(chr);
        }
        return new string(result[..ctr]);
    }

    private static void Encode(Span<byte> destBuffer, ReadOnlySpan<char> value, int maxLength)
    {
        var table = EncodeTableLazy.Value;
        if (value.Length > maxLength)
            value = value[..maxLength];

        for (int i = 0; i < value.Length; i++)
        {
            char chr = value[i];
            ushort code = table.TryGetValue(chr, out var mapped) ? mapped : StringConverter4Util.ConvertChar2ValueG4(chr);
            WriteUInt16LittleEndian(destBuffer[(i * 2)..], code);
        }

        int count = value.Length * 2;
        if (count != destBuffer.Length)
            WriteUInt16LittleEndian(destBuffer[count..], Terminator);
    }

    private static IReadOnlyDictionary<ushort, char> LoadDecodeTable()
    {
        var table = new Dictionary<ushort, char>();
        foreach (var (code, chr) in ReadResourceEntries())
            table[code] = chr;
        return table;
    }

    private static IReadOnlyDictionary<char, ushort> LoadEncodeTable()
    {
        var table = new Dictionary<char, ushort>();
        foreach (var (code, chr) in ReadResourceEntries())
            table.TryAdd(chr, code); // first (lowest-code) occurrence wins for reverse lookup
        return table;
    }

    private static IEnumerable<(ushort Code, char Char)> ReadResourceEntries()
    {
        var assembly = typeof(Gen4ChineseCharTableService).Assembly;
        using var stream = assembly.GetManifestResourceStream(ResourceName)
            ?? throw new InvalidOperationException($"Embedded resource '{ResourceName}' not found.");
        using var reader = new StreamReader(stream);
        string? line;
        while ((line = reader.ReadLine()) is not null)
        {
            if (line.Length == 0)
                continue;
            int tab = line.IndexOf('\t');
            if (tab < 0)
                continue;
            var code = ushort.Parse(line.AsSpan(0, tab), System.Globalization.NumberStyles.HexNumber);
            var chr = line[tab + 1];
            yield return (code, chr);
        }
    }
}
