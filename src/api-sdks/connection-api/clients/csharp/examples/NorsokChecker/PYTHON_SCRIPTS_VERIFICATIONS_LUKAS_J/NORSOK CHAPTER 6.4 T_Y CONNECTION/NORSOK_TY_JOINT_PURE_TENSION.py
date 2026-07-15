# =============================================================================
#  NORSOK N-004 Rev.3 (Feb 2013)  -  Section 6.4.3  "Strength of simple joints"
#  STEP 1:  T / Y joints only
#
#  This script is written FLAT (top to bottom, no functions) so it is easy to
#  read and to tune. You change the numbers in the INPUT block, run the file,
#  and it prints every intermediate value plus the final 6.4.3.6 check.
#
#  Units used everywhere (keep them consistent!):
#     length / thickness / diameter ....... mm
#     force ................................ N
#     stress / yield strength .............. MPa  (= N/mm2)
#     bending moment ....................... N*mm
#
#  Reference: equations 6.52, 6.53, 6.54, 6.55, 6.57 and Tables 6-3, 6-4.
# =============================================================================

import math

# -----------------------------------------------------------------------------
#  INPUT  -  this is the only block you normally edit
# -----------------------------------------------------------------------------

# --- Chord (the "through" member that the brace lands on) --------------------
D   = 168.3      # chord outer diameter            [mm]
T   = 8       # chord wall thickness            [mm]
fy  = 355.0      # chord yield strength            [MPa]

# --- Brace (the member framing into the chord) -------------------------------
d   = 114.3      # brace outer diameter            [mm]
# (brace thickness t is only needed for the validity check, not the resistance)
t   = 6.3       # brace wall thickness            [mm]

# --- Geometry ----------------------------------------------------------------
theta_deg = 60.0   # angle between brace axis and chord axis   [degrees]

# --- Design forces in the BRACE at the joint (from your analysis) ------------
#     Sign convention for axial: tension positive, compression negative.
N_Sd   =  250.0e3     # brace axial force                 [N]
My_Sd  =   0     # brace in-plane bending moment     [N*mm]
Mz_Sd  =   0     # brace out-of-plane bending moment [N*mm]

# --- Design FORCES in the CHORD at the joint (for the Qf chord factor) -------
#     Clause 6.4.3.4: use the AVERAGE of the chord forces/moments on either
#     side of the brace intersection. Enter that average here.
#     The stresses sigma_a, sigma_my, sigma_mz are computed from these forces
#     and the chord section properties (A, W_el) further below.
#     Axial sign convention: tension positive.
N_chord   = -62.5e3    # chord axial force               [N]   (+tension)
My_chord  = 0    # chord in-plane bending moment     [N*mm]
Mz_chord  = 0    # chord out-of-plane bending moment [N*mm]

# --- Partial safety factor ---------------------------------------------------
gamma_M = 1.15         # clause 6.4.3.2

# =============================================================================
#  CALCULATION  -  you do not normally edit below this line while tuning
# =============================================================================

print("=" * 70)
print("NORSOK N-004  6.4.3  -  Simple T/Y joint check")
print("=" * 70)

# -----------------------------------------------------------------------------
#  STEP 0 - geometric parameters (Figure 6-3) and validity range (6.4.3.1)
# -----------------------------------------------------------------------------
beta  = d / D            # brace dia / chord dia
gamma = D / (2.0 * T)    # chord radius / chord thickness
tau   = t / T            # brace thickness / chord thickness
theta = math.radians(theta_deg)
sin_theta = math.sin(theta)

print("\n--- STEP 0: geometry ---")
print(f"  beta  = d/D      = {beta:.4f}")
print(f"  gamma = D/(2T)   = {gamma:.4f}")
print(f"  tau   = t/T      = {tau:.4f}")
print(f"  theta            = {theta_deg:.1f} deg   (sin = {sin_theta:.4f})")

# Validity range. We only WARN; the code allows out-of-range use with the
# lesser of actual vs. limiting parameters, which we will add later if needed.
print("\n  Validity range (6.4.3.1):")
print(f"    0.2 <= beta <= 1.0   : {'OK' if 0.2 <= beta <= 1.0 else 'OUT OF RANGE'}")
print(f"    10  <= gamma <= 50   : {'OK' if 10.0 <= gamma <= 50.0 else 'OUT OF RANGE'}")
print(f"    30  <= theta <= 90   : {'OK' if 30.0 <= theta_deg <= 90.0 else 'OUT OF RANGE'}")

# -----------------------------------------------------------------------------
#  STEP 0b - chord section properties and chord stresses
#            Properties are computed for a plain circular hollow section (CHS)
#            from the chord outer diameter D and wall thickness T.
#            d_in = inner diameter = D - 2T.
#            A     = cross-sectional area
#            I     = second moment of area
#            W_el  = elastic section modulus  = I / (D/2)   -> used for STRESS
#            W_pl  = plastic section modulus               -> carried for later,
#                                                             NOT used in 6.4.3
# -----------------------------------------------------------------------------
d_in = D - 2.0 * T
A    = math.pi / 4.0 * (D ** 2 - d_in ** 2)
I    = math.pi / 64.0 * (D ** 4 - d_in ** 4)
W_el = I / (D / 2.0)
W_pl = (D ** 3 - d_in ** 3) / 6.0
R = D/2

print("\n--- STEP 0b: chord section properties (CHS from D, T) ---")
print(f"  A    = {A:12.1f} mm^2")
print(f"  I    = {I:12.1f} mm^4")
print(f"  W_el = {W_el:12.1f} mm^3   (used for stress)")
print(f"  W_pl = {W_pl:12.1f} mm^3   (not used in 6.4.3)")
print(f"  R = {R:12.1f} mm")

# Chord stresses from the average chord forces (clause 6.4.3.4).
# Stress = Force / Area for axial,  Moment / W_el for bending (elastic).
sigma_a_Sd  = N_chord  / A        # +tension
sigma_my_Sd = My_chord / W_el     # in-plane bending stress
sigma_mz_Sd = Mz_chord / W_el     # out-of-plane bending stress

print("\n  chord stresses (from forces / section properties):")
print(f"  sigma_a_Sd  = N_chord/A     = {sigma_a_Sd:8.3f} MPa")
print(f"  sigma_my_Sd = My_chord/W_el = {sigma_my_Sd:8.3f} MPa")
print(f"  sigma_mz_Sd = Mz_chord/W_el = {sigma_mz_Sd:8.3f} MPa")

# -----------------------------------------------------------------------------
#  STEP 1 - strength factor Qu  (Table 6-3, Y / T-Y joint row)
#           One Qu per action type: axial, in-plane bending, out-of-plane.
# -----------------------------------------------------------------------------
print("\n--- STEP 1: Qu strength factors (Table 6-3, Y joint) ---")

# Axial Qu depends on whether the brace axial force is tension or compression.
if N_Sd >= 0.0:
    # Axial tension
    Qu_axial = 30.0 * beta
    print(f"  brace axial = TENSION")
    print(f"  Qu_axial = 30*beta = {Qu_axial:.4f}")
else:
    # Axial compression: take the minimum of the two expressions
    expr1 = 2.8 + (20.0 + 0.8 * gamma) * beta ** 1.6
    expr2 = 2.8 + 36.0 * beta ** 1.6
    Qu_axial = min(expr1, expr2)
    print(f"  brace axial = COMPRESSION")
    print(f"  expr1 = 2.8+(20+0.8*gamma)*beta^1.6 = {expr1:.4f}")
    print(f"  expr2 = 2.8+36*beta^1.6             = {expr2:.4f}")
    print(f"  Qu_axial = min(expr1, expr2)        = {Qu_axial:.4f}")

# In-plane bending Qu
Qu_ipb = (5.0 + 0.7 * gamma) * beta ** 1.2
print(f"  Qu_ipb (in-plane bend)  = (5+0.7*gamma)*beta^1.2     = {Qu_ipb:.4f}")

# Out-of-plane bending Qu
Qu_opb = 2.5 + (4.5 + 0.2 * gamma) * beta ** 2.6
print(f"  Qu_opb (out-of-plane)   = 2.5+(4.5+0.2*gamma)*beta^2.6 = {Qu_opb:.4f}")

# -----------------------------------------------------------------------------
#  STEP 2 - chord action factor Qf  (6.4.3.4, eq 6.54 and 6.55, Table 6-4)
#           Qf is computed twice: once with the AXIAL coefficients and once
#           with the MOMENT coefficients, because Table 6-4 differs by action.
# -----------------------------------------------------------------------------
print("\n--- STEP 2: chord action factor Qf (6.54 / 6.55) ---")

# A^2 (eq 6.55) - the chord utilisation parameter (same for both actions)
A2 = (sigma_a_Sd / fy) ** 2 \
     + (sigma_my_Sd ** 2 + sigma_mz_Sd ** 2) / (1.62 * fy ** 2)
print(f"  A^2 (eq 6.55) = {A2:.5f}")

# Table 6-4 coefficients
# T/Y joints under brace AXIAL loading:
C1_ax, C2_ax, C3_ax = 0.3, 0.0, 0.8
# All joints under brace MOMENT loading:
C1_m,  C2_m,  C3_m  = 0.2, 0.0, 0.4

# eq 6.54:  Qf = 1.0 + C1*(sig_a/fy) - C2*(sig_my/(1.62*fy)) - C3*A^2
Qf_axial = 1.0 + C1_ax * (sigma_a_Sd / fy) \
               - C2_ax * (sigma_my_Sd / (1.62 * fy)) \
               - C3_ax * A2
Qf_moment = 1.0 + C1_m * (sigma_a_Sd / fy) \
                - C2_m * (sigma_my_Sd / (1.62 * fy)) \
                - C3_m * A2

print(f"  Qf for axial action  (C1,C2,C3 = {C1_ax},{C2_ax},{C3_ax}) = {Qf_axial:.4f}")
print(f"  Qf for moment action (C1,C2,C3 = {C1_m},{C2_m},{C3_m}) = {Qf_moment:.4f}")

# -----------------------------------------------------------------------------
#  STEP 3 - design resistances  (eq 6.52 axial, eq 6.53 bending)
#           common factor = fy * T^2 / (gamma_M * sin(theta))
# -----------------------------------------------------------------------------
print("\n--- STEP 3: design resistances (6.52 / 6.53) ---")

common = fy * T ** 2 / (gamma_M * sin_theta)   # [N] for axial, *d -> [N*mm]
# Axial resistance (uses axial Qu and axial Qf)
N_Rd = common * Qu_axial * Qf_axial
# In-plane bending resistance (uses ipb Qu and moment Qf, and *d per 6.53)
My_Rd = common * d * Qu_ipb * Qf_moment
# Out-of-plane bending resistance (uses opb Qu and moment Qf, and *d)
Mz_Rd = common * d * Qu_opb * Qf_moment

print(f"  common factor fy*T^2/(gamma_M*sin) = {common:.1f} N")
print(f"  N_Rd  = {N_Rd/1e3:10.2f} kN")
print(f"  My_Rd = {My_Rd/1e6:10.2f} kN*m")
print(f"  Mz_Rd = {Mz_Rd/1e6:10.2f} kN*m")

# -----------------------------------------------------------------------------
#  STEP 4 - strength check  (eq 6.57)
#     N_Sd/N_Rd + (My_Sd/My_Rd)^2 + Mz_Sd/Mz_Rd  <= 1.0
#     (axial term uses the magnitude of the force)
# -----------------------------------------------------------------------------
print("\n--- STEP 4: interaction check (6.57) ---")

term_N  = (abs(N_Sd) / N_Rd)
term_My = (abs(My_Sd) / My_Rd)**2
term_Mz = (abs(Mz_Sd) / Mz_Rd)
utilisation = term_N + term_My + term_Mz

print(f"  axial term   N_Sd/N_Rd          = {term_N:.4f}")
print(f"  in-plane     (My_Sd/My_Rd)^2    = {term_My:.4f}")
print(f"  out-of-plane Mz_Sd/Mz_Rd        = {term_Mz:.4f}")
print(f"  --------------------------------------------")
print(f"  UTILISATION (sum)               = {utilisation:.4f}")
print(f"  VERDICT                         = {'PASS' if utilisation <= 1.0 else 'FAIL'}")
print("=" * 70)






