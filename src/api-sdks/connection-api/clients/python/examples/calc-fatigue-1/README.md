# Fatigue analysis calculation

## What the script does

`calc-fatigue-1.py` opens the sample project `../projects/fatigue-check.ideaCon`, runs a stress/strain CBFEM analysis on its first connection, then switches the connection's analysis type to fatigue and recalculates.

Steps performed (real method names from the script):

1. Attach to a running Connection API service via `ConnectionApiServiceAttacher("http://localhost:5000").create_api_client()`.
2. `api_client.project.open_project_from_filepath(project_file_path)` — open the `.ideaCon` project.
3. `api_client.project.get_project_data(...)` and `api_client.connection.get_connections(...)` — print project data and pick the first connection.
4. `api_client.calculation.calculate(...)` — run the stress/strain analysis; print the brief results, then `api_client.calculation.get_results(...)` for detailed results.
5. Set `connection1.analysis_type = "fatigues"` and apply it with `api_client.connection.update_connection(...)` (the analysis type is taken from the connection).
6. `api_client.calculation.calculate(...)` again — now runs the fatigue analysis; print brief results, `api_client.calculation.get_results(...)` detailed results, and `api_client.calculation.get_raw_json_results(...)` raw JSON results.

## How to run

1. Install the client package (the package version must match your installed IDEA StatiCa version):

   ```console
   pip install ideastatica-connection-api
   ```

2. Start the Connection API service from your IDEA StatiCa installation folder (adjust to your installed version):

   ```console
   "C:\Program Files\IDEA StatiCa\StatiCa 26.0\IdeaStatiCa.ConnectionRestApi.exe"
   ```

   The service listens on port 5000 by default, which is the `baseUrl` the script expects. Alternatively, start the service from Python with `ConnectionApiServiceRunner` (see the `calculate_project_using_runner` example).

3. Run the script from this folder (it locates the sample project via the relative path `../projects`):

   ```console
   python calc-fatigue-1.py
   ```
