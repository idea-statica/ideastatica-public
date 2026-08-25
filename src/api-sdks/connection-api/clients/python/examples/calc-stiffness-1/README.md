# Stiffness analysis calculation

## What the script does

`calc-stiffness-1.py` opens the sample project `../projects/beam-to-cont-column.ideaCon`, selects the analyzed member, and runs a stiffness CBFEM analysis on the first connection.

Steps performed (real method names from the script):

1. Attach to a running Connection API service via `ConnectionApiServiceAttacher("http://localhost:5000").create_api_client()`.
2. `api_client.project.open_project_from_filepath(project_file_path)` — open the `.ideaCon` project.
3. `api_client.project.get_project_data(...)` and `api_client.connection.get_connections(...)` — print project data and pick the first connection.
4. `api_client.member.set_bearing_member(...)` — set member id 2 as the analyzed member.
5. Build `ConCalculationParameter()` with `connection_ids` and `analysis_type = "stiffness"`, then `api_client.calculation.get_raw_json_results(...)` — run the stiffness analysis and read the raw JSON results (the script parses the `stiffnesess` section).

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
   python calc-stiffness-1.py
   ```
