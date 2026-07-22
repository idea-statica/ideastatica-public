# =============================================================================
#  NORSOK N-004 Rev.3 (Feb 2013)  -  Section 6.4.3  "Strength of simple joints"
#  X JOINT  (two coaxial braces on OPPOSITE sides of the chord; the axial force
#            of one brace is transferred straight through the chord to the other)
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
#  Differences vs. the K and T/Y scripts:
#    * X axial TENSION      Qu = 6.4 * gamma^(0.6*beta^2)        (Table 6-3, X)
#    * X axial COMPRESSION  Qu = (2.8 + (12 + 0.1*gamma)*beta) * Qbeta
#                           -> uses the GEOMETRIC factor Qbeta (NOT the gap Qg;
#                              X joints have no gap).
#    * In-plane / out-of-plane bending Qu are the SAME for all classes.
#    * Table 6-4 Qf coefficients for X depend on BOTH the force sign (tension /
#      compression) AND on beta, with LINEAR interpolation between beta = 0.9
#      and beta = 1.0 (note 1 under Table 6-4). This is the extra wrinkle X has
#      that K and T/Y do not.
#
#  Reference: equations 6.52, 6.53, 6.54, 6.55, 6.57 and Tables 6-3, 6-4.
#  (X rows of Tables 6-3 and 6-4 verified directly against the NORSOK PDF.)
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
t   = 12.0       # brace wall thickness            [mm]  (validity check only)

# --- Geometry ---------------------------------------------------------------
theta_deg = 60  # angle between brace axis and chord axis   [deg]
                   #   X joint: opposite braces, typically near 90 deg.

# --- Design forces in the BRACE at the joint --------------------------------
#     Axial sign: tension positive, compression negative.
N_Sd   = 1355.0e3    # brace axial force                 [N]
My_Sd  = 0    # brace in-plane bending moment     [N*mm]
Mz_Sd  = 0    # brace out-of-plane bending moment [N*mm]

# --- Design forces in the CHORD at the joint (for Qf, clause 6.4.3.4) --------
#     Use the AVERAGE of the chord forces/moments on either side of the brace
#     intersection. Axial sign: tension positive.
N_chord   =  0   # chord axial force                 [N]
My_chord  =  0   # chord in-plane bending moment     [N*mm]
Mz_chord  =  -0   # chord out-of-plane bending moment [N*mm]

# --- Partial safety factor --------------------------------------------------
gamma_M = 1.15         # clause 6.4.3.2

# =============================================================================
#  CALCULATION
# =============================================================================
print("=" * 70)
print("NORSOK N-004  6.4.3  -  Simple X joint check")
print("=" * 70)

# --- STEP 0: geometry (Figure 6-4) and validity range (6.4.3.1) -------------
beta  = d / D
gamma = D / (2.0 * T)
tau   = t / T
theta = math.radians(theta_deg)
sin_theta = math.sin(theta)

print("\n--- STEP 0: geometry ---")
print(f"  beta  = d/D    = {beta:.4f}")
print(f"  gamma = D/(2T) = {gamma:.4f}")
print(f"  tau   = t/T    = {tau:.4f}")
print(f"  theta          = {theta_deg:.1f} deg   (sin = {sin_theta:.4f})")

print("\n  Validity range (6.4.3.1):")
print(f"    0.2 <= beta <= 1.0   : {'OK' if 0.2 <= beta <= 1.0 else 'OUT OF RANGE'}")
print(f"    10  <= gamma <= 50   : {'OK' if 10.0 <= gamma <= 50.0 else 'OUT OF RANGE'}")
print(f"    30 deg  <= theta <= 90 deg   : {'OK' if 30.0 <= theta_deg <= 90.0 else 'OUT OF RANGE'}")

# --- STEP 0b: chord section properties and chord stresses -------------------
d_in = D - 2.0 * T
A    = math.pi / 4.0 * (D ** 2 - d_in ** 2)
I    = math.pi / 64.0 * (D ** 4 - d_in ** 4)
W_el = I / (D / 2.0)               # elastic section modulus, used for stresses

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

# --- STEP 1a: geometric factor Qbeta (Table 6-3, note a) -------------------
print("\n--- STEP 1a: Qbeta geometric factor (Table 6-3, note a) ---")
# X axial COMPRESSION uses Qbeta. (X has no gap, so Qg does not apply.)
Qbeta = 0.3 / (beta * (1.0 - 0.833 * beta)) if beta > 0.6 else 1.0
print(f"  Qbeta = 0.3/(beta*(1-0.833*beta)) if beta>0.6 else 1.0 = {Qbeta:.4f}")

# --- STEP 1b: strength factor Qu (Table 6-3, X row) ------------------------
print("\n--- STEP 1b: Qu strength factors (Table 6-3, X row) ---")
if N_Sd >= 0.0:
    # X axial tension
    Qu_axial = 6.4 * gamma ** (0.6 * beta ** 2)
    print(f"  brace axial = TENSION")
    print(f"  Qu_axial = 6.4*gamma^(0.6*beta^2) = {Qu_axial:.4f}")
else:
    # X axial compression
    Qu_axial = (2.8 + (12.0 + 0.1 * gamma) * beta) * Qbeta
    print(f"  brace axial = COMPRESSION")
    print(f"  Qu_axial = (2.8+(12+0.1*gamma)*beta)*Qbeta = {Qu_axial:.4f}")

# In-plane / out-of-plane bending: identical to K and T/Y.
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

# Table 6-4 for X joints (note 1: linear interpolation between beta=0.9 and 1.0).
#   X tension     : beta<=0.9 -> (C1,C2,C3)=(0.0, 0.0, 0.4)
#                   beta=1.0  -> (C1,C2,C3)=(0.2, 0.0, 0.2)
#   X compression : beta<=0.9 -> (C1,C2,C3)=(0.2, 0.0, 0.5)
#                   beta=1.0  -> (C1,C2,C3)=(-0.2, 0.0, 0.2)
def _interp_beta(c_low, c_high, b):
    # c_low at beta<=0.9, c_high at beta=1.0, linear in between, clamped.
    if b <= 0.9:
        return c_low
    if b >= 1.0:
        return c_high
    w = (b - 0.9) / 0.10
    return c_low + (c_high - c_low) * w

if N_Sd >= 0.0:
    C1_ax = _interp_beta(0.0, 0.2, beta)
    C2_ax = _interp_beta(0.0, 0.0, beta)
    C3_ax = _interp_beta(0.4, 0.2, beta)
    print(f"  X axial coeffs = TENSION, interpolated for beta={beta:.4f}")
else:
    C1_ax = _interp_beta(0.2, -0.2, beta)
    C2_ax = _interp_beta(0.0,  0.0, beta)
    C3_ax = _interp_beta(0.5,  0.2, beta)
    print(f"  X axial coeffs = COMPRESSION, interpolated for beta={beta:.4f}")

# All joints under brace MOMENT loading
C1_m, C2_m, C3_m = 0.2, 0.0, 0.4

Qf_axial  = 1.0 + C1_ax * (sigma_a_Sd / fy) - C2_ax * (sigma_my_Sd / (1.62 * fy)) - C3_ax * A2
Qf_moment = 1.0 + C1_m  * (sigma_a_Sd / fy) - C2_m  * (sigma_my_Sd / (1.62 * fy)) - C3_m  * A2
print(f"  Qf axial  (C1,C2,C3 = {C1_ax:.3f},{C2_ax:.3f},{C3_ax:.3f}) = {Qf_axial:.4f}")
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
