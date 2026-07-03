# =============================================================================
#  NORSOK N-004 Rev.3 (Feb 2013)  -  Section 6.4.3
#  PARAMETRIC STUDY:  how does the chord action factor Qf behave as the chord
#  stress level changes?
#
#  Qf is computed twice per load state (eq 6.54 / 6.55, Table 6-4):
#     Qf_axial  -> uses the AXIAL  coefficients (C1,C2,C3 = 0.3, 0.0, 0.8)
#     Qf_moment -> uses the MOMENT coefficients (C1,C2,C3 = 0.2, 0.0, 0.4)
#
#  We run a TRIPLE-NESTED loop over the chord actions:
#     N_chord  from -1000e3 .. +1000e3  step 100e3   [N]
#     My_chord from  -25e6  ..  +25e6   step   5e6   [N*mm]
#     Mz_chord from  -25e6  ..  +25e6   step   5e6   [N*mm]
#  For every (N, My, Mz) combination we store Qf_axial and Qf_moment.
#
#  Output: two figures, each with three scatter panels.
#     Figure 1 -> Qf_axial  vs N_chord, vs My_chord, vs Mz_chord
#     Figure 2 -> Qf_moment vs N_chord, vs My_chord, vs Mz_chord
#  Because it is a full nested loop, each x-value carries MANY Qf points
#  (one per combination of the other two actions) -> a vertical spread that
#  shows the influence of the remaining stresses.
#
#  Units:  length/thickness/dia = mm | force = N | stress/fy = MPa | moment = N*mm
# =============================================================================

import math
import numpy as np
import matplotlib.pyplot as plt

# -----------------------------------------------------------------------------
#  INPUT  -  geometry and material (same section as test_QU_QF.py)
# -----------------------------------------------------------------------------
D  = 457.0     # chord outer diameter   [mm]
T  = 16.0      # chord wall thickness   [mm]
fy = 355.0     # chord yield strength   [MPa]

# -----------------------------------------------------------------------------
#  Chord section properties (plain CHS from D, T)  -  needed for the stresses
# -----------------------------------------------------------------------------
d_in = D - 2.0 * T
A    = math.pi / 4.0 * (D ** 2 - d_in ** 2)     # area                [mm^2]
I    = math.pi / 64.0 * (D ** 4 - d_in ** 4)    # second moment       [mm^4]
W_el = I / (D / 2.0)                            # elastic modulus     [mm^3]

# -----------------------------------------------------------------------------
#  Table 6-4 coefficients
# -----------------------------------------------------------------------------
C1_ax, C2_ax, C3_ax = 0.3, 0.0, 0.8     # T/Y joints, brace AXIAL loading
C1_m,  C2_m,  C3_m  = 0.2, 0.0, 0.4     # all joints, brace MOMENT loading


def qf(sigma_a, sigma_my, sigma_mz, C1, C2, C3):
    """Chord action factor Qf (eq 6.54) using the utilisation A^2 (eq 6.55).

    sigma_* are chord stresses in MPa; C1..C3 are the Table 6-4 coefficients.
    """
    A2 = (sigma_a / fy) ** 2 \
         + (sigma_my ** 2 + sigma_mz ** 2) / (1.62 * fy ** 2)      # eq 6.55
    return 1.0 + C1 * (sigma_a / fy) \
               - C2 * (sigma_my / (1.62 * fy)) \
               - C3 * A2                                            # eq 6.54


# -----------------------------------------------------------------------------
#  Sweep grids  -  inclusive of both end points (np.arange + tiny pad on stop)
# -----------------------------------------------------------------------------
N_range  = np.arange(-10000e3, 10000e3 + 1.0, 1000e3)   # [N]
My_range = np.arange( -250e6,   250e6  + 1.0,   50e6)   # [N*mm]
Mz_range = np.arange( -250e6,   250e6  + 1.0,   5e6)   # [N*mm]

print("Sweep sizes:")
print(f"  N_chord  : {len(N_range):3d} values  {N_range[0]/1e3:+.0f} .. {N_range[-1]/1e3:+.0f} kN")
print(f"  My_chord : {len(My_range):3d} values  {My_range[0]/1e6:+.0f} .. {My_range[-1]/1e6:+.0f} kNm")
print(f"  Mz_chord : {len(Mz_range):3d} values  {Mz_range[0]/1e6:+.0f} .. {Mz_range[-1]/1e6:+.0f} kNm")
print(f"  total combinations = {len(N_range)*len(My_range)*len(Mz_range)}")

# storage: one row per (N, My, Mz) combination
Ns, Mys, Mzs        = [], [], []
Qf_ax_all, Qf_m_all = [], []

# -----------------------------------------------------------------------------
#  TRIPLE-NESTED LOOP
# -----------------------------------------------------------------------------
for N_chord in N_range:
    for My_chord in My_range:
        for Mz_chord in Mz_range:
            # chord stresses from the current chord actions (clause 6.4.3.4)
            sigma_a  = N_chord  / A        # +tension   [MPa]
            sigma_my = My_chord / W_el     # in-plane   [MPa]
            sigma_mz = Mz_chord / W_el     # out-of-pl. [MPa]

            Qf_axial  = qf(sigma_a, sigma_my, sigma_mz, C1_ax, C2_ax, C3_ax)
            Qf_moment = qf(sigma_a, sigma_my, sigma_mz, C1_m,  C2_m,  C3_m)

            Ns.append(N_chord)
            Mys.append(My_chord)
            Mzs.append(Mz_chord)
            Qf_ax_all.append(Qf_axial)
            Qf_m_all.append(Qf_moment)

# to numpy for plotting / scaling
Ns        = np.array(Ns)  / 1e3    # -> kN
Mys       = np.array(Mys) / 1e6    # -> kNm
Mzs       = np.array(Mzs) / 1e6    # -> kNm
Qf_ax_all = np.array(Qf_ax_all)
Qf_m_all  = np.array(Qf_m_all)

# -----------------------------------------------------------------------------
#  PLOT  -  a small helper so the two figures are identical in style
# -----------------------------------------------------------------------------
def make_figure(qf_values, title, ylabel):
    """3-panel scatter: qf_values vs N_chord, vs My_chord, vs Mz_chord."""
    fig, axes = plt.subplots(1, 3, figsize=(15, 5), sharey=True)
    fig.suptitle(title, fontsize=13, fontweight="bold")

    panels = [
        (Ns,  "N_chord [kN]",   axes[0]),
        (Mys, "My_chord [kNm]", axes[1]),
        (Mzs, "Mz_chord [kNm]", axes[2]),
    ]
    for xdata, xlabel, ax in panels:
        ax.scatter(xdata, qf_values, s=8, alpha=0.25, edgecolors="none")
        ax.set_xlabel(xlabel)
        ax.grid(True, alpha=0.3)
    axes[0].set_ylabel(ylabel)
    fig.tight_layout(rect=(0, 0, 1, 0.95))
    return fig


make_figure(Qf_ax_all,
            "Qf_axial  vs chord actions (full nested sweep)",
            "Qf_axial  (eq 6.54, axial coeffs)")

make_figure(Qf_m_all,
            "Qf_moment vs chord actions (full nested sweep)",
            "Qf_moment (eq 6.54, moment coeffs)")

# quick numeric summary
print("\nQf_axial :  min = %.4f   max = %.4f" % (Qf_ax_all.min(), Qf_ax_all.max()))
print("Qf_moment:  min = %.4f   max = %.4f" % (Qf_m_all.min(),  Qf_m_all.max()))

plt.show()
