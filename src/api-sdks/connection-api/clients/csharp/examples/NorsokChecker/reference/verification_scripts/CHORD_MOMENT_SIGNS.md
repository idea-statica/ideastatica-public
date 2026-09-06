# Chord moment signs — why `M_op,chord` differs from IDEA's `Mz`

An answer to a question raised during the hand verification of the T/Y unit-test joint:

> *Compression interaction as a combination — I do not see why Mz is substituted with a minus
> sign and not a plus. The other chord values, N and My, are right, but for Mz a negative value
> is substituted.*

**Short answer: it is correct, and it changes nothing in the check.** The sign does not come from
the arithmetic but from the frame the moment is projected into. And σ_mz enters Q_f only squared,
so its sign cannot reach the result.

## `M_ip` and `M_op` are not the chord's `My` and `Mz`

That is the heart of the misunderstanding. The table does **not** hold the chord's local moments;
it holds the chord's moment vector projected into the frame defined by the **brace under check**:

```
M_ip = M · n_b              n_b = normal of that brace's sub-plane
M_op = M · (n_b × e_x)      e_x = chord axis
```

`n_b` is the same normal the brace-force resolution uses (`brace_subplane_normal`), so that "chord
bending in plane" means the plane the brace lies in. That is what NORSOK asks for: Q_f is evaluated
in the plane of the joint, not in the section's local axes.

**Consequence:** the sign of `M_ip` / `M_op` depends on the orientation of `n_b` — on which way the
brace leaves the chord and which side it sits on. It has no fixed relation to the signs of `My` and
`Mz` as IDEA displays them.

## Verified on `TY_CONNECTION_UNIT_TEST.ideaCon`

Load effect `INTERACTION_COMPRESSION`, values read from the API:

| | value |
|---|---|
| chord axis `e_x` | `[1, 0, 0]` |
| brace axis | `[0.5, 0, 0.866]` → the brace lies in the **XZ** plane |
| chord moments (local) | `Mx = −2.15` · `My = −5.00` · `Mz = +1.25` kN·m |
| moment as a global vector | `[−2.15, −5.00, +1.25]` kN·m |

Because the brace lies in the XZ plane, the normal of that plane is the global **±Y** axis. Hence:

- `M_ip = M · n_b = +5.00 kN·m` — numerically `−My`, and only because `n_b` points to **−Y**
- `M_op = M · (n_b × e_x) = +1.25 kN·m`

Had the brace left on the other side, both signs would reverse, with the physical situation and the
check result unchanged.

## Two deliberate sign reversals in the stress derivation

Both are documented in `chord_stress_at_brace` in `norsok/extract.py`:

**1. σ_my is reversed on purpose.** Mechanics gives the fibre in tension as positive; NORSOK wants
σ_my **positive in compression** at the brace footprint:

```
sigma_my = −(M_ip · z_ip / I)        z_ip = side · R
```

`side` (+1/−1) says which side of the chord the footprint is on — that is, which fibre is evaluated.

**2. σ_mz is not reversed and need not be** — it enters the check only squared.

Equations 6.54 and 6.55 (implemented in `Qf` in `norsok/n64.py`):

```
A²  = (σ_a/f_y)² + (σ_my² + σ_mz²) / (1.62·f_y²)
Q_f = 1 + C1·(σ_a/f_y) − C2·(σ_my/(1.62·f_y)) − C3·A²
```

**σ_mz occurs only in `A²`, and there as `σ_mz²`.** Eq 6.54 has no linear term for it — only σ_a
and σ_my have one. Its sign therefore has no way to affect the result; functionally it is the same
as taking the absolute value. Likewise eq 6.57 works with `|M_op|`.

Verified by computation: `Qf` returns a bit-identical value for `σ_mz = +0.74 MPa` and `−0.74 MPa`,
under two different sets of C1/C2/C3 coefficients.

**Note — for σ_a and σ_my the sign does decide**, because they also enter linearly. On the same
inputs:

| change | Q_f |
|---|---|
| as read | 1.0141 |
| σ_a reversed | 1.0078 |
| σ_my reversed | 0.9879 |
| σ_mz reversed | 1.0141 — **no difference** |

So "signs do not matter" cannot be generalised. It holds for σ_mz only.

## What a hand check should therefore compare

- `N_chord` — relates directly to the chord's `N` in IDEA (axial component, sign preserved)
- the **magnitudes** of `M_ip` and `M_op` — they should match the magnitudes of `My` and `Mz`, but
  only if the brace plane coincides with the chord's local axes; otherwise they are projections and
  need not match the individual components
- σ_a and σ_my **including sign** — they enter 6.54 linearly, so the sign decides there; σ_my follows
  the NORSOK convention "positive in compression"
- σ_mz **by magnitude only** — it enters the check only as σ_mz², see above

A sign mismatch on `M_op` / σ_mz is not an error in itself. An error would be a mismatch in
**magnitude**, or a differing sign on σ_a or σ_my.

## A note on `M_ip`

For the same reason this file gives `M_ip = +5.00` while `My = −5.00`. Same mechanism — the reversed
orientation of `n_b`. It has no effect on σ_my, which is reversed deliberately anyway so that the
"positive in compression" convention holds.

---

*Compiled 2026-08-25 by measurement on `TY_CONNECTION_UNIT_TEST.ideaCon` against a running
Connection REST service. Closes the σ_mz sign question listed under open items in `UNIFICATION.md`.*
