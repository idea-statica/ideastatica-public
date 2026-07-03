# =============================================================================
# NORSOK N-004 Rev.3 - Section 6.4.3
# T / Y joints only
#
# Console version:
# - Loops over theta increments
# - Stores all results for every increment in a list of dictionaries
# - Prints detailed output for every increment
# - Prints one final summary table at the end
# =============================================================================

import math

# -----------------------------------------------------------------------------
# INPUT
# -----------------------------------------------------------------------------

# --- Chord ----------------------------------------------------
D = 457.0       # chord outer diameter [mm]
T = 16.0        # chord wall thickness [mm]
fy = 355.0      # chord yield strength [MPa]

# --- Brace ----------------------------------------------------
d = 273.0       # brace outer diameter [mm]
t = 12.0        # brace wall thickness [mm]

# --- Design forces in BRACE ----------------------------------
N_Sd = -1000.0e3     # axial force [N], tension positive
My_Sd = 40.0e6      # in-plane bending moment [N*mm]
Mz_Sd = 20.0e6      # out-of-plane bending moment [N*mm]

# --- Design forces in CHORD ----------------------------------
N_chord = 129e3
My_chord = -20e6
Mz_chord = -2.6e6

# --- Partial safety factor -----------------------------------
gamma_M = 1.15

# --- Angle loop ----------------------------------------------
theta_start = 30
theta_end = 90
theta_step = 5

# Store output from every increment here
results = []

# =============================================================================
# CALCULATION
# =============================================================================

print("=" * 90)
print("NORSOK N-004 6.4.3 - Simple T/Y joint check")
print("=" * 90)

for increment, theta_deg in enumerate(range(theta_start, theta_end + 1, theta_step), start=1):

    print("\n")
    print("#" * 90)
    print(f"INCREMENT {increment} | theta = {theta_deg} deg")
    print("#" * 90)

    # -------------------------------------------------------------------------
    # STEP 0 - Geometry
    # -------------------------------------------------------------------------

    beta = d / D
    gamma = D / (2.0 * T)
    tau = t / T

    theta_rad = math.radians(theta_deg)
    sin_theta = math.sin(theta_rad)

    beta_status = "OK" if 0.2 <= beta <= 1.0 else "OUT OF RANGE"
    gamma_status = "OK" if 10.0 <= gamma <= 50.0 else "OUT OF RANGE"
    theta_status = "OK" if 30.0 <= theta_deg <= 90.0 else "OUT OF RANGE"

    print("\n--- STEP 0: Geometry ---")
    print(f"beta  = d/D    = {beta:.4f}   -> {beta_status}")
    print(f"gamma = D/(2T) = {gamma:.4f}  -> {gamma_status}")
    print(f"tau   = t/T    = {tau:.4f}")
    print(f"theta          = {theta_deg:.1f} deg")
    print(f"sin(theta)     = {sin_theta:.4f} -> {theta_status}")

    # -------------------------------------------------------------------------
    # STEP 0b - Chord section properties
    # -------------------------------------------------------------------------

    d_in = D - 2.0 * T

    A = math.pi / 4.0 * (D ** 2 - d_in ** 2)
    I = math.pi / 64.0 * (D ** 4 - d_in ** 4)
    W_el = I / (D / 2.0)
    W_pl = (D ** 3 - d_in ** 3) / 6.0

    sigma_a_Sd = N_chord / A
    sigma_my_Sd = My_chord / W_el
    sigma_mz_Sd = Mz_chord / W_el

    print("\n--- STEP 0b: Chord section properties ---")
    print(f"A    = {A:12.2f} mm2")
    print(f"I    = {I:12.2f} mm4")
    print(f"W_el = {W_el:12.2f} mm3")
    print(f"W_pl = {W_pl:12.2f} mm3")

    print("\nChord stresses:")
    print(f"sigma_a_Sd  = {sigma_a_Sd:8.3f} MPa")
    print(f"sigma_my_Sd = {sigma_my_Sd:8.3f} MPa")
    print(f"sigma_mz_Sd = {sigma_mz_Sd:8.3f} MPa")

    # -------------------------------------------------------------------------
    # STEP 1 - Qu strength factors
    # -------------------------------------------------------------------------

    print("\n--- STEP 1: Qu strength factors ---")

    if N_Sd >= 0.0:
        brace_axial_type = "TENSION"
        Qu_axial = 30.0 * beta

        expr1 = None
        expr2 = None

        print("Brace axial force = TENSION")
        print(f"Qu_axial = 30 * beta = {Qu_axial:.4f}")

    else:
        brace_axial_type = "COMPRESSION"

        expr1 = 2.8 + (20.0 + 0.8 * gamma) * beta ** 1.6
        expr2 = 2.8 + 36.0 * beta ** 1.6
        Qu_axial = min(expr1, expr2)

        print("Brace axial force = COMPRESSION")
        print(f"expr1 = {expr1:.4f}")
        print(f"expr2 = {expr2:.4f}")
        print(f"Qu_axial = min(expr1, expr2) = {Qu_axial:.4f}")

    Qu_ipb = (5.0 + 0.7 * gamma) * beta ** 1.2
    Qu_opb = 2.5 + (4.5 + 0.2 * gamma) * beta ** 2.6

    print(f"Qu_ipb = {Qu_ipb:.4f}")
    print(f"Qu_opb = {Qu_opb:.4f}")

    # -------------------------------------------------------------------------
    # STEP 2 - Chord action factor Qf
    # -------------------------------------------------------------------------

    print("\n--- STEP 2: Chord action factor Qf ---")

    A2 = (
        (sigma_a_Sd / fy) ** 2
        + (sigma_my_Sd ** 2 + sigma_mz_Sd ** 2) / (1.62 * fy ** 2)
    )

    C1_ax = 0.3
    C2_ax = 0.0
    C3_ax = 0.8

    C1_m = 0.2
    C2_m = 0.0
    C3_m = 0.4

    Qf_axial = (
        1.0
        + C1_ax * (sigma_a_Sd / fy)
        - C2_ax * (sigma_my_Sd / (1.62 * fy))
        - C3_ax * A2
    )

    Qf_moment = (
        1.0
        + C1_m * (sigma_a_Sd / fy)
        - C2_m * (sigma_my_Sd / (1.62 * fy))
        - C3_m * A2
    )

    print(f"A2        = {A2:.5f}")
    print(f"Qf_axial  = {Qf_axial:.4f}")
    print(f"Qf_moment = {Qf_moment:.4f}")

    # -------------------------------------------------------------------------
    # STEP 3 - Design resistances
    # -------------------------------------------------------------------------

    print("\n--- STEP 3: Design resistances ---")

    common = fy * T ** 2 / (gamma_M * sin_theta)

    N_Rd = common * Qu_axial * Qf_axial
    My_Rd = common * d * Qu_ipb * Qf_moment
    Mz_Rd = common * d * Qu_opb * Qf_moment

    print(f"common = {common:.2f} N")
    print(f"N_Rd   = {N_Rd / 1e3:10.2f} kN")
    print(f"My_Rd  = {My_Rd / 1e6:10.2f} kN*m")
    print(f"Mz_Rd  = {Mz_Rd / 1e6:10.2f} kN*m")

    # -------------------------------------------------------------------------
    # STEP 4 - Interaction check
    # -------------------------------------------------------------------------

    print("\n--- STEP 4: Interaction check ---")

    term_N = abs(N_Sd) / N_Rd
    term_My = (abs(My_Sd) / My_Rd) ** 2
    term_Mz = abs(Mz_Sd) / Mz_Rd

    utilisation = term_N + term_My + term_Mz

    verdict = "PASS" if utilisation <= 1.0 else "FAIL"

    print(f"term_N  = {term_N:.4f}")
    print(f"term_My = {term_My:.4f}")
    print(f"term_Mz = {term_Mz:.4f}")
    print(f"UTILISATION = {utilisation:.4f}")
    print(f"VERDICT     = {verdict}")

    # -------------------------------------------------------------------------
    # STORE ALL OUTPUTS FROM THIS INCREMENT
    # -------------------------------------------------------------------------

    results.append({
        "increment": increment,
        "theta_deg": theta_deg,
        "sin_theta": sin_theta,

        "beta": beta,
        "gamma": gamma,
        "tau": tau,

        "beta_status": beta_status,
        "gamma_status": gamma_status,
        "theta_status": theta_status,

        "A": A,
        "I": I,
        "W_el": W_el,
        "W_pl": W_pl,

        "sigma_a_Sd": sigma_a_Sd,
        "sigma_my_Sd": sigma_my_Sd,
        "sigma_mz_Sd": sigma_mz_Sd,

        "brace_axial_type": brace_axial_type,
        "Qu_axial": Qu_axial,
        "Qu_ipb": Qu_ipb,
        "Qu_opb": Qu_opb,

        "A2": A2,
        "Qf_axial": Qf_axial,
        "Qf_moment": Qf_moment,

        "common": common,
        "N_Rd": N_Rd,
        "My_Rd": My_Rd,
        "Mz_Rd": Mz_Rd,

        "term_N": term_N,
        "term_My": term_My,
        "term_Mz": term_Mz,
        "utilisation": utilisation,
        "verdict": verdict
    })


# =============================================================================
# FINAL SUMMARY OUTPUT
# =============================================================================

print("\n")
print("=" * 120)
print("FINAL SUMMARY - ALL INCREMENTS")
print("=" * 120)

header = (
    f"{'Inc':>4} | "
    f"{'Theta':>7} | "
    f"{'sin':>7} | "
    f"{'N_Rd [kN]':>12} | "
    f"{'My_Rd [kNm]':>14} | "
    f"{'Mz_Rd [kNm]':>14} | "
    f"{'term_N':>8} | "
    f"{'term_My':>8} | "
    f"{'term_Mz':>8} | "
    f"{'Util.':>8} | "
    f"{'Verdict':>8}"
)

print(header)
print("-" * 120)

for row in results:
    print(
        f"{row['increment']:>4} | "
        f"{row['theta_deg']:>7.1f} | "
        f"{row['sin_theta']:>7.4f} | "
        f"{row['N_Rd'] / 1e3:>12.2f} | "
        f"{row['My_Rd'] / 1e6:>14.2f} | "
        f"{row['Mz_Rd'] / 1e6:>14.2f} | "
        f"{row['term_N']:>8.4f} | "
        f"{row['term_My']:>8.4f} | "
        f"{row['term_Mz']:>8.4f} | "
        f"{row['utilisation']:>8.4f} | "
        f"{row['verdict']:>8}"
    )

print("=" * 120)

# =============================================================================
# OPTIONAL: ACCESS STORED RESULTS MANUALLY
# =============================================================================

print("\nExample: first increment stored output:")
print(results[0])

print("\nExample: last increment stored output:")
print(results[-1])



# =============================================================================
# PLOTS - SUBPLOTS FROM STORED RESULTS
# =============================================================================

import matplotlib.pyplot as plt

# Extract data from results
theta_values = [row["theta_deg"] for row in results]

N_Rd_values = [row["N_Rd"] / 1e3 for row in results]       # N -> kN
My_Rd_values = [row["My_Rd"] / 1e6 for row in results]     # Nmm -> kNm
Mz_Rd_values = [row["Mz_Rd"] / 1e6 for row in results]     # Nmm -> kNm
utilisation_values = [row["utilisation"] for row in results]

# Create subplots
fig, axs = plt.subplots(2, 2, figsize=(14, 10))

# -----------------------------------------------------------------------------
# Subplot 1 - N_Rd
# -----------------------------------------------------------------------------
axs[0, 0].plot(theta_values, N_Rd_values, marker="o",color = "green")
axs[0, 0].set_title("Axial resistance N_Rd vs Theta")
axs[0, 0].set_xlabel("Theta [deg]")
axs[0, 0].set_ylabel("N_Rd [kN]")
axs[0, 0].grid(True)


# -----------------------------------------------------------------------------
# Subplot 2 - My_Rd
# -----------------------------------------------------------------------------
axs[0, 1].plot(theta_values, My_Rd_values, marker="o",color = "blue")
axs[0, 1].set_title("In-plane bending resistance My_Rd vs Theta")
axs[0, 1].set_xlabel("Theta [deg]")
axs[0, 1].set_ylabel("My_Rd [kNm]")
axs[0, 1].grid(True)


# -----------------------------------------------------------------------------
# Subplot 3 - Mz_Rd
# -----------------------------------------------------------------------------
axs[1, 0].plot(theta_values, Mz_Rd_values, marker="o",color = "orange")
axs[1, 0].set_title("Out-of-plane bending resistance Mz_Rd vs Theta")
axs[1, 0].set_xlabel("Theta [deg]")
axs[1, 0].set_ylabel("Mz_Rd [kNm]")
axs[1, 0].grid(True)


# -----------------------------------------------------------------------------
# Subplot 4 - Utilisation
# -----------------------------------------------------------------------------
axs[1, 1].plot(theta_values, utilisation_values, marker="o",color = "red")
axs[1, 1].axhline(y=1.0, linestyle="--", label="Limit = 1.0")
axs[1, 1].set_title("Utilisation vs Theta")
axs[1, 1].set_xlabel("Theta [deg]")
axs[1, 1].set_ylabel("Utilisation [-]")
axs[1, 1].grid(True)
axs[1, 1].legend()

# Improve layout
plt.tight_layout()

# Show plot
plt.show()


# =============================================================================
# SAVE ONLY FINAL RESULT VALUES TO JSON
# =============================================================================

import json

results_only = []

for row in results:
    results_only.append({
        "increment": row["increment"],
        "theta_deg": row["theta_deg"],
        "N_Rd_kN": row["N_Rd"] / 1e3,
        "My_Rd_kNm": row["My_Rd"] / 1e6,
        "Mz_Rd_kNm": row["Mz_Rd"] / 1e6,
    })

with open("norsok_results_only.json", "w", encoding="utf-8") as json_file:
    json.dump(results_only, json_file, indent=4)

print("Results saved to norsok_results_only.json")