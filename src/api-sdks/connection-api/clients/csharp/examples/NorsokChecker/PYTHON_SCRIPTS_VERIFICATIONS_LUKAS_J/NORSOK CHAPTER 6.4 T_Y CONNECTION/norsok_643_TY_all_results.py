import json, pandas as pd

with open(r"C:\Users\LukasJuricek\Claude\NORSOKpythonProject1\NORSOK CHAPTER 6.4 T_Y CONNECTION\norsok_results_only.json") as fh:
    rows = json.load(fh)

df = pd.DataFrame(rows)  # JSON list -> DataFrame, columns auto-named
print(df.head())
print(df.columns.tolist())  # ['increment', 'theta_deg', 'N_Rd_kN', 'My_Rd_kNm', 'Mz_Rd_kNm']

k = [0.1, 0.2, 0.3, 0.4, 0.5, 0.6, 0.7, 0.8, 0.9, 1]

# -----------------------------------------------------------------------------
#  Build the applied-load set for every angle.
#  angles[30] -> list of 10 dicts (one per fraction in k)
# -----------------------------------------------------------------------------
angles = {}
for row in range(len(df)):
    theta = int(df["theta_deg"][row])
    angles[theta] = [{
        "fraction": i,
        "N_Sd":  df["N_Rd_kN"][row]   * i,   # applied brace axial   [kN]
        "My_Sd": df["My_Rd_kNm"][row] * i,   # applied in-plane M    [kN*m]
        "Mz_Sd": df["Mz_Rd_kNm"][row] * i,   # applied out-of-plane M[kN*m]
        # carry the resistances so the check below needs nothing else
        "N_Rd":  df["N_Rd_kN"][row],
        "My_Rd": df["My_Rd_kNm"][row],
        "Mz_Rd": df["Mz_Rd_kNm"][row],
    } for i in k]

# -----------------------------------------------------------------------------
#  Run the 6.4.3.6 interaction check (eq 6.57) on every (angle, fraction).
#     utilisation = |N_Sd|/N_Rd + (|My_Sd|/My_Rd)^2 + |Mz_Sd|/Mz_Rd
#  Collect everything into one flat dataset.
# -----------------------------------------------------------------------------
dataset = []
for theta, loadcases in angles.items():
    for lc in loadcases:
        term_N  = abs(lc["N_Sd"])  / lc["N_Rd"]
        term_My = (abs(lc["My_Sd"]) / lc["My_Rd"]) ** 2
        term_Mz = abs(lc["Mz_Sd"]) / lc["Mz_Rd"]
        util = term_N + term_My + term_Mz

        dataset.append({
            "theta_deg":   theta,
            "fraction":    lc["fraction"],
            "N_Sd_kN":     lc["N_Sd"],
            "My_Sd_kNm":   lc["My_Sd"],
            "Mz_Sd_kNm":   lc["Mz_Sd"],
            "utilisation": util,
            "verdict":     "PASS" if util <= 1.0 else "FAIL",
        })

# dataset as a DataFrame -> easy to view / pivot / export
ds = pd.DataFrame(dataset)
print("\nDataset:", len(ds), "rows  (", len(angles), "angles x", len(k), "fractions )")

# pivot: rows = angle, columns = fraction, cell = utilisation
pivot = ds.pivot(index="theta_deg", columns="fraction", values="utilisation")
print("\nUtilisation table (rows = angle, cols = load fraction):")
print(pivot.round(3))

# save the full dataset
ds.to_csv(r"C:\Users\LukasJuricek\Claude\NORSOKpythonProject1\norsok_dataset_all_angles.csv", index=False)
print("\nSaved -> norsok_dataset_all_angles.csv")






# =============================================================================
#  NORSOK N-004 Rev.3 (Feb 2013)  -  Section 6.4.3  "Strength of simple joints"
#  STEP 1:  T / Y joints only
#
#  This part runs the full 6.4.3 check, but instead of ONE hardcoded brace
#  load it loops over every angle-variant built from the JSON above and uses
#  each one (every angle x every load fraction) as the bracing load N_Sd /
#  My_Sd / Mz_Sd.
#
#  Units inside the check:  force = N, moment = N*mm, length = mm, stress = MPa.
#  The JSON loads are in kN / kN*m, so they are converted on the way in.
#
#  Reference: equations 6.52, 6.53, 6.54, 6.55, 6.57 and Tables 6-3, 6-4.
# =============================================================================

import math

# -----------------------------------------------------------------------------
#  FIXED JOINT GEOMETRY / MATERIAL  -  same as before, shared by every variant
# -----------------------------------------------------------------------------

# --- Chord (the "through" member that the brace lands on) --------------------
D   = 457.0      # chord outer diameter            [mm]
T   = 16.0       # chord wall thickness            [mm]
fy  = 355.0      # chord yield strength            [MPa]

# --- Brace (the member framing into the chord) -------------------------------
d   = 273.0      # brace outer diameter            [mm]
t   = 12.0       # brace wall thickness            [mm]

# --- Design FORCES in the CHORD at the joint (for the Qf chord factor) -------
N_chord   =  0   # chord axial force                 [N]   (+tension)
My_chord  =  0   # chord in-plane bending moment     [N*mm]
Mz_chord  =  0   # chord out-of-plane bending moment [N*mm]

# --- Partial safety factor ---------------------------------------------------
gamma_M = 1.15         # clause 6.4.3.2

# -----------------------------------------------------------------------------
#  6.4.3 check, wrapped in a function so it can run once per load variant.
#  Inputs: theta_deg and the brace loads (N_Sd [N], My_Sd / Mz_Sd [N*mm]).
#  Returns the utilisation (eq 6.57).  verbose=True prints the full breakdown.
# -----------------------------------------------------------------------------
def norsok_643_check(theta_deg, N_Sd, My_Sd, Mz_Sd, verbose=False):

    # ---- STEP 0: geometry (Figure 6-3) and validity range (6.4.3.1) ----
    beta  = d / D
    gamma = D / (2.0 * T)
    tau   = t / T
    theta = math.radians(theta_deg)
    sin_theta = math.sin(theta)

    # ---- STEP 0b: chord section properties + chord stresses ----
    d_in = D - 2.0 * T
    A    = math.pi / 4.0 * (D ** 2 - d_in ** 2)
    I    = math.pi / 64.0 * (D ** 4 - d_in ** 4)
    W_el = I / (D / 2.0)

    sigma_a_Sd  = N_chord  / A
    sigma_my_Sd = My_chord / W_el
    sigma_mz_Sd = Mz_chord / W_el

    # ---- STEP 1: Qu strength factors (Table 6-3, Y joint) ----
    if N_Sd >= 0.0:
        Qu_axial = 30.0 * beta                                   # tension
    else:
        expr1 = 2.8 + (20.0 + 0.8 * gamma) * beta ** 1.6         # compression
        expr2 = 2.8 + 36.0 * beta ** 1.6
        Qu_axial = min(expr1, expr2)
    Qu_ipb = (5.0 + 0.7 * gamma) * beta ** 1.2
    Qu_opb = 2.5 + (4.5 + 0.2 * gamma) * beta ** 2.6

    # ---- STEP 2: chord action factor Qf (eq 6.54 / 6.55, Table 6-4) ----
    A2 = (sigma_a_Sd / fy) ** 2 \
         + (sigma_my_Sd ** 2 + sigma_mz_Sd ** 2) / (1.62 * fy ** 2)
    C1_ax, C2_ax, C3_ax = 0.3, 0.0, 0.8      # T/Y axial
    C1_m,  C2_m,  C3_m  = 0.2, 0.0, 0.4      # all joints, moment
    Qf_axial = 1.0 + C1_ax * (sigma_a_Sd / fy) \
                   - C2_ax * (sigma_my_Sd / (1.62 * fy)) - C3_ax * A2
    Qf_moment = 1.0 + C1_m * (sigma_a_Sd / fy) \
                    - C2_m * (sigma_my_Sd / (1.62 * fy)) - C3_m * A2

    # ---- STEP 3: design resistances (eq 6.52 / 6.53) ----
    common = fy * T ** 2 / (gamma_M * sin_theta)
    N_Rd  = common * Qu_axial * Qf_axial
    My_Rd = common * d * Qu_ipb * Qf_moment
    Mz_Rd = common * d * Qu_opb * Qf_moment

    # ---- STEP 4: interaction check (eq 6.57) ----
    term_N  = abs(N_Sd)  / N_Rd
    term_My = (abs(My_Sd) / My_Rd) ** 2
    term_Mz = abs(Mz_Sd) / Mz_Rd
    utilisation = term_N + term_My + term_Mz

    if verbose:
        print(f"\n  theta = {theta_deg:.1f} deg | "
              f"N_Sd = {N_Sd/1e3:8.1f} kN  My_Sd = {My_Sd/1e6:7.2f} kN*m  "
              f"Mz_Sd = {Mz_Sd/1e6:7.2f} kN*m")
        print(f"    N_Rd = {N_Rd/1e3:8.1f} kN  My_Rd = {My_Rd/1e6:7.2f} kN*m  "
              f"Mz_Rd = {Mz_Rd/1e6:7.2f} kN*m")
        print(f"    terms: N={term_N:.3f}  My={term_My:.3f}  Mz={term_Mz:.3f}  "
              f"-> util = {utilisation:.4f}  "
              f"[{'PASS' if utilisation <= 1.0 else 'FAIL'}]")

    return utilisation

# =============================================================================
#  FULL CROSS: every increment from every angle is used as the BRACE LOAD,
#  and each one is checked against EVERY target-angle's joint.
#
#    source (theta, fraction)  ->  N_Sd / My_Sd / Mz_Sd  (the applied load)
#    target  theta             ->  the joint whose resistance it is checked on
#
#  13 source angles x 10 fractions = 130 load increments
#  x 13 target angles              = 1690 utilisation values.
#
#  `angles` (from the JSON section) holds the load increments per source angle,
#  in kN / kN*m, so convert to N / N*mm before the check.
# =============================================================================
print("=" * 70)
print("NORSOK N-004  6.4.3  -  every increment (all angles) as brace load")
print("=" * 70)

target_angles = sorted(angles.keys())     # the 13 joints to check against

check_results = []
for src_theta, loadcases in angles.items():            # where the load comes from
    for lc in loadcases:                               # each fraction increment
        N_Sd  = lc["N_Sd"]  * 1e3                       # kN   -> N
        My_Sd = lc["My_Sd"] * 1e6                       # kN*m -> N*mm
        Mz_Sd = lc["Mz_Sd"] * 1e6
        for tgt_theta in target_angles:                # check on every joint
            util = norsok_643_check(tgt_theta, N_Sd, My_Sd, Mz_Sd)
            check_results.append({
                "source_theta": src_theta,
                "fraction":     lc["fraction"],
                "target_theta": tgt_theta,
                "N_Sd_kN":      lc["N_Sd"],
                "My_Sd_kNm":    lc["My_Sd"],
                "Mz_Sd_kNm":    lc["Mz_Sd"],
                "utilisation":  util,
                "verdict":      "PASS" if util <= 1.0 else "FAIL",
            })

res = pd.DataFrame(check_results)
print(f"\nComputed {len(res)} utilisation values "
      f"({len(angles)} source angles x {len(k)} fractions x {len(target_angles)} target angles).")

# -----------------------------------------------------------------------------
#  Export the full dataset for the interactive dashboard.
#  Written as a JS file (window.NORSOK_DATA = [...]) so the HTML dashboard can
#  load it with a plain <script> tag and open by double-click (no web server).
# -----------------------------------------------------------------------------
import json as _json
_data_js = r"C:\Users\LukasJuricek\Claude\NORSOKpythonProject1\norsok_dashboard_data.js"
with open(_data_js, "w", encoding="utf-8") as _fh:
    _fh.write("window.NORSOK_DATA = ")
    _json.dump(check_results, _fh)
    _fh.write(";")
print("Saved data -> norsok_dashboard_data.js")

# -----------------------------------------------------------------------------
#  GRAPH: one SEPARATE figure per source angle (saved as its own PNG).
#    In each figure:  x = target joint angle, y = utilisation,
#    one line per load increment (fraction k=0.1...1.0), plus a legend.
# -----------------------------------------------------------------------------
try:
    import matplotlib
    matplotlib.use("Agg")
    import matplotlib.pyplot as plt
    import os

    out_dir = r"C:\Users\LukasJuricek\Claude\NORSOKpythonProject1\graphs_by_angle"
    os.makedirs(out_dir, exist_ok=True)

    source_angles    = sorted(angles.keys())           # one figure each
    fractions_sorted = sorted(set(res["fraction"]))     # one line each
    cmap = plt.get_cmap("viridis", len(fractions_sorted))
    frac_color = {fr: cmap(i) for i, fr in enumerate(fractions_sorted)}

    for src in source_angles:
        sub = res[res["source_theta"] == src]

        fig, ax = plt.subplots(figsize=(9, 6))
        for fr in fractions_sorted:
            line = sub[sub["fraction"] == fr].sort_values("target_theta")
            ax.plot(line["target_theta"], line["utilisation"],
                    marker="o", markersize=5, linewidth=1.6,
                    color=frac_color[fr], label=f"k = {fr:.1f}")

        ax.axhline(1.0, color="red", linestyle="--", linewidth=1.4, label="limit = 1.0")
        ax.set_xlabel("Target joint angle  theta  [deg]")
        ax.set_ylabel("Utilisation  (eq 6.57)")
        ax.set_title(f"NORSOK 6.4.3 - Utilisation vs angle\n"
                     f"source angle = {src} deg  (each line = load increment k)")
        ax.grid(True, alpha=0.3)
        ax.legend(title="load increment", loc="upper left",
                  ncol=2, fontsize=9)

        fig.tight_layout()
        png = os.path.join(out_dir, f"util_vs_angle_source_{src:02d}deg.png")
        fig.savefig(png, dpi=130)
        plt.close(fig)
        print(f"Saved graph -> graphs_by_angle\\util_vs_angle_source_{src:02d}deg.png")

    print(f"\n{len(source_angles)} separate graphs saved in: graphs_by_angle\\")
except ImportError:
    print("matplotlib not installed - graph skipped.")
    print("Install with:  python -m pip install matplotlib")

print("=" * 70)
