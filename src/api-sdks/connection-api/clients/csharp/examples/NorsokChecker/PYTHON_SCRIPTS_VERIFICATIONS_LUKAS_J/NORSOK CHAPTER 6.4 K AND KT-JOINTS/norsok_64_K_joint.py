# =============================================================================
#  NORSOK N-004 Rev.3 (Feb 2013)  -  Section 6.4.3  "Strength of simple joints"
#  K JOINT  (brace whose axial force is balanced by another brace on the same
#            side of the chord; gap g between the brace footprints)
#
#  Flat script (top to bottom, no functions): edit the INPUT block, run, read
#  every intermediate value and the final 6.4.3.6 interaction check.
#
#  Units (keep consistent):
#     length / thickness / diameter ....... mm
#     force ................................ N
#     stress / yield strength .............. MPa  (= N/mm2)
#     bending moment ....................... N*mm
#
#  Differences vs. the T/Y scripts:
#    * K axial Qu uses the gap factor Qg (depends on g/D) and a min{} pair.
#    * Table 6-4 row for K is (C1,C2,C3) = (0.2, 0.2, 0.3) - note C2 = 0.2,
#      the only joint type with a non-zero C2 (chord in-plane bending term).
#    * This evaluates ONE brace classified as K. The "balancing" brace on the
#      same side is represented only through the gap g (its force balance is a
#      classification input, handled outside this single-brace check).
#
#  Reference: equations 6.52, 6.53, 6.54, 6.55, 6.57 and Tables 6-3, 6-4.
# =============================================================================

import math

# -----------------------------------------------------------------------------
#  INPUT  -  the only block you normally edit
# -----------------------------------------------------------------------------

# --- Chord (the "through" member the braces land on) ------------------------
D   = 457.0      # chord outer diameter            [mm]
T   = 16.0       # chord wall thickness            [mm]
fy  = 355.0      # chord yield strength            [MPa]

# --- Brace being checked ----------------------------------------------------
d   = 273.0      # brace outer diameter            [mm]
t   = 12.0       # brace wall thickness            [mm]
fyb = 355.0      # brace yield strength            [MPa]  (used in Qg overlap branch)

# --- Geometry ---------------------------------------------------------------
theta_deg = 45.0   # angle between brace axis and chord axis   [deg]
g         = 257   # gap between adjacent brace footprints, along chord [mm]
                   #   g > 0 = gap; NORSOK K-joint: 50 mm < g < D.
                   #   g < 0 = overlap -> OUT OF SCOPE of this script.

# --- Design forces in the BRACE at the joint --------------------------------
#     Axial sign: tension positive, compression negative.
N_Sd   =  250.0e3    # brace axial force                 [N]
My_Sd  =    40.0e6    # brace in-plane bending moment     [N*mm]
Mz_Sd  =    20.0e6    # brace out-of-plane bending moment [N*mm]

# --- Design forces in the CHORD at the joint (for Qf, clause 6.4.3.4) --------
#     Use the AVERAGE of the chord forces/moments on either side of the brace
#     intersection. Axial sign: tension positive.
N_chord   =  123e3   # chord axial force                 [N]
My_chord  =  88.8e6   # chord in-plane bending moment     [N*mm]
Mz_chord  =  -1.8e6   # chord out-of-plane bending moment [N*mm]

# --- Partial safety factor --------------------------------------------------
gamma_M = 1.15         # clause 6.4.3.2

# =============================================================================
#  CALCULATION
# =============================================================================
print("=" * 70)
print("NORSOK N-004  6.4.3  -  Simple K joint check")
print("=" * 70)

# --- STEP 0: geometry (Figure 6-5) and validity range (6.4.3.1) -------------
beta  = d / D
gamma = D / (2.0 * T)
tau   = t / T
theta = math.radians(theta_deg)
sin_theta = math.sin(theta)
gD = g / D

print("\n--- STEP 0: geometry ---")
print(f"  beta  = d/D    = {beta:.4f}")
print(f"  gamma = D/(2T) = {gamma:.4f}")
print(f"  tau   = t/T    = {tau:.4f}")
print(f"  theta          = {theta_deg:.1f} deg   (sin = {sin_theta:.4f})")
print(f"  g              = {g:.1f} mm   (g/D = {gD:.4f})")

print("\n  Validity range (6.4.3.1):")
print(f"    0.2 <= beta <= 1.0   : {'OK' if 0.2 <= beta <= 1.0 else 'OUT OF RANGE'}")
print(f"    10  <= gamma <= 50   : {'OK' if 10.0 <= gamma <= 50.0 else 'OUT OF RANGE'}")
print(f"    30  <= theta <= 90   : {'OK' if 30.0 <= theta_deg <= 90.0 else 'OUT OF RANGE'}")
print(f"    g/D > -0.6 (K)       : {'OK' if gD > -0.6 else 'OUT OF RANGE'}")
if g < 0.0:
    print("    *** WARNING: g < 0 = OVERLAP joint -> OUT OF SCOPE of this script ***")

# --- STEP 0b: chord section properties and chord stresses -------------------
d_in = D - 2.0 * T
A    = math.pi / 4.0 * (D ** 2 - d_in ** 2)
I    = math.pi / 64.0 * (D ** 4 - d_in ** 4)
W_el = I / (D / 2.0)

print("\n--- STEP 0b: chord section properties (CHS from D, T) ---")
print(f"  A    = {A:12.1f} mm^2")
print(f"  I    = {I:12.1f} mm^4")
print(f"  W_el = {W_el:12.1f} mm^3")

sigma_a_Sd  = N_chord  / A         # +tension
sigma_my_Sd = My_chord / W_el
sigma_mz_Sd = Mz_chord / W_el

print("\n  chord stresses:")
print(f"  sigma_a_Sd  = N_chord/A     = {sigma_a_Sd:8.3f} MPa")
print(f"  sigma_my_Sd = My_chord/W_el = {sigma_my_Sd:8.3f} MPa")
print(f"  sigma_mz_Sd = Mz_chord/W_el = {sigma_mz_Sd:8.3f} MPa")

# --- STEP 1: gap factor Qg and geometric factor Qbeta (notes under Table 6-3)
print("\n--- STEP 1a: Qg gap factor and Qbeta (Table 6-3 notes a, b) ---")
# Qg gap factor (note b)
if gD >= 0.05:
    Qg = max(1.0 + 0.2 * (1.0 - 2.8 * gD) ** 3, 1.0)
    print(f"  g/D >= 0.05 : Qg = 1+0.2*(1-2.8*g/D)^3 = {Qg:.4f}")
elif gD <= -0.05:
    phi = (t * fyb) / (T * fy)
    Qg = 0.13 + 0.65 * phi * gamma ** 0.5
    print(f"  g/D <= -0.05 (overlap): phi = t*fyb/(T*fyc) = {phi:.4f}")
    print(f"  Qg = 0.13+0.65*phi*gamma^0.5 = {Qg:.4f}")
else:
    phi = (t * fyb) / (T * fy)
    Qg_pos = max(1.0 + 0.2 * (1.0 - 2.8 * 0.05) ** 3, 1.0)
    Qg_neg = 0.13 + 0.65 * phi * gamma ** 0.5
    w = (gD - (-0.05)) / 0.10
    Qg = Qg_neg + (Qg_pos - Qg_neg) * w
    print(f"  -0.05 < g/D < 0.05 : linear interpolation -> Qg = {Qg:.4f}")

# Qbeta geometric factor (note a) - used by some Qu rows; for K axial it is not
# in the formula, but compute & show it for completeness.
Qbeta = 0.3 / (beta * (1.0 - 0.833 * beta)) if beta > 0.6 else 1.0
print(f"  Qbeta = 0.3/(beta*(1-0.833*beta)) if beta>0.6 else 1.0 = {Qbeta:.4f}")

# --- STEP 1b: strength factor Qu (Table 6-3, K row) ------------------------
print("\n--- STEP 1b: Qu strength factors (Table 6-3, K row) ---")
# K axial: same expression for tension and compression, with the gap factor Qg
expr1 = (16.0 + 1.2 * gamma) * beta ** 1.2 * Qg
expr2 = 40.0 * beta ** 1.2 * Qg
Qu_axial = min(expr1, expr2)
print(f"  brace axial = {'TENSION' if N_Sd >= 0 else 'COMPRESSION'} (same K formula)")
print(f"  expr1 = (16+1.2*gamma)*beta^1.2*Qg = {expr1:.4f}")
print(f"  expr2 = 40*beta^1.2*Qg             = {expr2:.4f}")
print(f"  Qu_axial = min(expr1, expr2)       = {Qu_axial:.4f}")

Qu_ipb = (5.0 + 0.7 * gamma) * beta ** 1.2
Qu_opb = 2.5 + (4.5 + 0.2 * gamma) * beta ** 2.6
print(f"  Qu_ipb (in-plane)     = (5+0.7*gamma)*beta^1.2       = {Qu_ipb:.4f}")
print(f"  Qu_opb (out-of-plane) = 2.5+(4.5+0.2*gamma)*beta^2.6 = {Qu_opb:.4f}")

# --- STEP 2: chord action factor Qf (6.54 / 6.55, Table 6-4) ---------------
print("\n--- STEP 2: chord action factor Qf (6.54 / 6.55) ---")
# eq 6.55: 2nd-term denominator is 1.62*fy^2  (NOT (1.62*fy)^2). In Python
# 1.62*fy**2 = 1.62*(fy^2) because ** binds tighter than *.
A2 = (sigma_a_Sd / fy) ** 2 \
     + (sigma_my_Sd ** 2 + sigma_mz_Sd ** 2) / (1.62 * fy ** 2)
print(f"  A^2 (eq 6.55) = {A2:.5f}")

# Table 6-4: K joints under balanced axial loading; all joints under MOMENT
C1_ax, C2_ax, C3_ax = 0.2, 0.2, 0.3   # note C2 = 0.2 for K (non-zero!)
C1_m,  C2_m,  C3_m  = 0.2, 0.0, 0.4
Qf_axial = 1.0 + C1_ax * (sigma_a_Sd / fy) - C2_ax * (sigma_my_Sd / (1.62 * fy)) - C3_ax * A2
Qf_moment = 1.0 + C1_m * (sigma_a_Sd / fy) - C2_m * (sigma_my_Sd / (1.62 * fy)) - C3_m * A2
print(f"  Qf axial  (C1,C2,C3 = {C1_ax},{C2_ax},{C3_ax}) = {Qf_axial:.4f}")
print(f"  Qf moment (C1,C2,C3 = {C1_m},{C2_m},{C3_m}) = {Qf_moment:.4f}")
print("  (Qf < 1.0 under chord stress is correct - it is the chord-load reduction.)")

# --- STEP 3: design resistances (6.52 axial, 6.53 bending) -----------------
print("\n--- STEP 3: design resistances (6.52 / 6.53) ---")
common = fy * T ** 2 / (gamma_M * sin_theta)
N_Rd  = common * Qu_axial * Qf_axial
My_Rd = common * d * Qu_ipb * Qf_moment
Mz_Rd = common * d * Qu_opb * Qf_moment
print(f"  common = fy*T^2/(gamma_M*sin) = {common:.1f} N")
print(f"  N_Rd  = {N_Rd/1e3:10.2f} kN")
print(f"  My_Rd = {My_Rd/1e6:10.2f} kN*m")
print(f"  Mz_Rd = {Mz_Rd/1e6:10.2f} kN*m")

# --- STEP 4: interaction check (6.57) --------------------------------------
print("\n--- STEP 4: interaction check (6.57) ---")
term_N  = abs(N_Sd) / N_Rd
term_My = (abs(My_Sd) / My_Rd) ** 2
term_Mz = abs(Mz_Sd) / Mz_Rd
utilisation = term_N + term_My + term_Mz
print(f"  axial term   N_Sd/N_Rd       = {term_N:.4f}")
print(f"  in-plane     (My_Sd/My_Rd)^2 = {term_My:.4f}")
print(f"  out-of-plane Mz_Sd/Mz_Rd     = {term_Mz:.4f}")
print(f"  --------------------------------------------")
print(f"  UTILISATION (sum)            = {utilisation:.4f}")
print(f"  VERDICT                      = {'PASS' if utilisation <= 1.0 else 'FAIL'}")
print("=" * 70)


