# Znaménka momentů chordu — proč se `M_op,chord` liší od `Mz` v IDEA

Odpověď na dotaz v `NORSOK CHAPTER 6.4 T_Y CONNECTION/NORSOK_TY_CONNECTIONS_UNIT_TESTS/TY_JOINT_NORSOK_ISSUES.docx`:

> *Interakce komprese jako kombinace – nedává mi smysl, proč je dosazeno Mz s mínusovým
> znaménkem a ne s kladným? Ostatní výpočty chordu pro N, My jsou správné, ale pro Mz
> dosazuje záporné.*

**Krátká odpověď: je to správně a na výsledku posudku to nic nemění.** Znaménko nevzniká
výpočtem, ale tím, do jakého souřadného rámce se moment promítá. A σ_mz vstupuje do Qf pouze
v druhé mocnině, takže jeho znaménko je pro výsledek bezpředmětné.

## `M_ip` a `M_op` nejsou `My` a `Mz` chordu

To je jádro nedorozumění. V tabulce **nejsou** lokální momenty chordu, ale jeho momentový vektor
promítnutý do rámce, který určuje **posuzovaný brace**:

```
M_ip = M · n_b              n_b = normála sub-roviny daného brace
M_op = M · (n_b × e_x)      e_x = osa chordu
```

`n_b` je táž normála, kterou používá výpočet sil v brace (`brace_subplane_normal`), aby „ohyb
chordu v rovině" znamenal tutéž rovinu, ve které leží brace. To je požadavek NORSOK: Qf se
vyhodnocuje v rovině daného styčníku, ne v lokálních osách profilu.

**Důsledek:** znaménko `M_ip` / `M_op` závisí na orientaci `n_b`, tedy na tom, kterým směrem
brace ze chordu vychází a na které jeho straně sedí. Nemá tedy pevný vztah ke znaménkům `My`,
`Mz`, jak je zobrazuje IDEA.

## Ověřeno na `TY_CONNECTION_UNIT_TEST.ideaCon`

Zatěžovací stav `INTERACTION_COMPRESSION`, hodnoty načtené z API:

| | hodnota |
|---|---|
| osa chordu `e_x` | `[1, 0, 0]` |
| osa brace | `[0,5 ; 0 ; 0,866]` → brace leží v rovině **XZ** |
| momenty chordu (lokálně) | `Mx = −2,15` · `My = −5,00` · `Mz = +1,25` kNm |
| moment jako globální vektor | `[−2,15 ; −5,00 ; +1,25]` kNm |

Protože brace leží v rovině XZ, je normála té roviny globální osa **±Y**. Odtud:

- `M_ip = M · n_b = +5,00 kNm` — číselně `−My`, a to výhradně proto, že `n_b` míří do **−Y**
- `M_op = M · (n_b × e_x) = +1,25 kNm`

Kdyby brace vycházel na druhou stranu, obě znaménka se obrátí, přičemž fyzikální situace
i výsledek posudku zůstanou totožné.

## Dva úmyslné obraty znaménka ve výpočtu napětí

Oba jsou v dokumentaci funkce `chord_stress_at_brace` v `norsok/extract.py`:

**1. σ_my se obrací záměrně.** Mechanika dává vlákno v tahu jako kladné, NORSOK ale požaduje
σ_my **kladné v tlaku** v místě paty brace:

```
sigma_my = −(M_ip · z_ip / I)        z_ip = side · R
```

`side` (+1/−1) říká, na které straně chordu pata brace leží — tedy které vlákno se vyhodnocuje.

**2. σ_mz se neobrací a nemusí** — do posudku vstupuje pouze v druhé mocnině.

Rovnice 6.54 a 6.55 (implementace ve funkci `Qf` v `norsok/n64.py`):

```
A²  = (σ_a/f_y)² + (σ_my² + σ_mz²) / (1,62·f_y²)
Q_f = 1 + C1·(σ_a/f_y) − C2·(σ_my/(1,62·f_y)) − C3·A²
```

**σ_mz se vyskytuje jedině v `A²`, a to jako `σ_mz²`.** Lineární člen pro něj v 6.54 není — má
ho jen σ_a a σ_my. Znaménko σ_mz tedy nemá jak výsledek ovlivnit; funkčně je to totéž, jako
kdyby se vzala absolutní hodnota. Stejně tak rovnice 6.57 pracuje s `|M_op|`.

Ověřeno výpočtem: `Qf` vrací pro `σ_mz = +0,74 MPa` i `−0,74 MPa` bitově shodnou hodnotu, ve
dvou různých sadách koeficientů C1/C2/C3.

**Pozor — u σ_a a σ_my znaménko naopak rozhoduje**, protože vstupují i lineárně. Na téže sadě
vstupů:

| změna | Q_f |
|---|---|
| výchozí stav | 1,0141 |
| obrácené σ_a | 1,0078 |
| obrácené σ_my | 0,9879 |
| obrácené σ_mz | 1,0141 — **bez rozdílu** |

Nelze tedy zobecnit „na znaménkách nezáleží". Platí to výhradně pro σ_mz.

## Co je tedy třeba ověřovat

Při kontrole ručním výpočtem má smysl porovnávat:

- `N_chord` — má přímý vztah k `N` chordu v IDEA (osová složka, znaménko se zachovává)
- **velikosti** `M_ip` a `M_op` — mají odpovídat velikostem `My` a `Mz`, ovšem pouze pokud
  rovina brace splývá s lokálními osami chordu; jinak jde o projekce a shoda s jednotlivými
  složkami nastat nemusí
- σ_a a σ_my **včetně znaménka** — vstupují do 6.54 lineárně, takže tam znaménko rozhoduje;
  u σ_my platí NORSOK konvence „kladné v tlaku"
- σ_mz **pouze ve velikosti** — do posudku jde jen jako σ_mz², viz výše

Nesouhlas znaménka u `M_op` / σ_mz sám o sobě není chyba. Chybou by bylo, kdyby nesouhlasila
**velikost**, nebo kdyby se lišilo znaménko u σ_a či σ_my.

## Poznámka k `M_ip`

Ze stejného důvodu vychází u tohoto souboru `M_ip = +5,00`, zatímco `My = −5,00`. Jde o týž
mechanismus — obrácená orientace `n_b`. Na σ_my to nemá vliv, protože ta se stejně obrací
záměrně, aby platila konvence „kladné v tlaku".

---

*Sestaveno 25. 8. 2026 měřením na `TY_CONNECTION_UNIT_TEST.ideaCon` proti běžící službě
IDEA StatiCa 26.1.0.2007. Souvisí s otevřeným bodem pro Lukáše v
[`../../PYTHON_STOPGAP.md`](../../PYTHON_STOPGAP.md) a v `UNIFICATION.md`.*
