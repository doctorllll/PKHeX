# Vendored data: Gen4 Chinese fan-translation character table

`CharTable.txt` in this folder is a trimmed extract of the character table maintained by the
**PokemonChineseTranslationRevise** project, which standardizes the character encoding used by
Chinese fan-translation ("汉化") patches of Generation 4 Pokémon games (Diamond/Pearl/Platinum/
HeartGold/SoulSilver) — including "官译" (official-name-style) revisions such as the one that
produced the save file this feature was built to read.

## Source

| | |
|---|---|
| Upstream repo | https://github.com/Xzonn/PokemonChineseTranslationRevise |
| File | `files/CharTable.txt` |
| Commit (SHA) fetched at | `bac148542b9456fd7902ead3b7a382ba4b0251ec` |
| Fetch date | 2026-08-07 |
| License (text/data assets) | CC BY-NC-SA 3.0 (see upstream `LICENSE.texts.txt`) |
| License (this repo, code) | GPL-3.0 |

## What was kept

These Gen4 games encode text as a sequence of 16-bit character codes. Codes `0x000`-`0x1EC` are
Nintendo's own official table (kana, full-width alphanumerics, symbols) — already implemented
identically in `PKHeX.Core`'s `StringConverter4Util.TableINT`, so **not** duplicated here. Fan
patches add Chinese characters by repurposing code ranges PKHeX doesn't otherwise use meaningfully
on a Japanese-region cart, including the Korean Hangul block (`0x400`-`0xD65`) and further ranges
beyond it. `CharTable.txt` here contains only the trimmed subset of the upstream table with
`code >= 0x1ED` (6814 entries) — the actual custom/Chinese extension — one `HEXCODE<TAB>character`
pair per line.

## Why this license mix is fine

The upstream data license (CC BY-NC-SA 3.0) requires attribution, share-alike, and non-commercial
use. PKHeX-Avalonia is a free, non-commercial, GPL-3.0 hobby project; this document provides the
required attribution, and the data file is kept distinct from — not merged into — the GPL-3.0
licensed code that reads it (`Gen4ChineseCharTableService.cs`). If this table is ever updated,
re-fetch `files/CharTable.txt` from the upstream repo, re-run the `code >= 0x1ED` filter, and update
the SHA/date above.

## Credits

Character table maintained by **Xzonn** and the **PokemonChineseTranslationRevise** project,
building on translation work by the original ACG Chinese translation group.
