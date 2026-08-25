# Joint design resistance calculation

## What the script does

`calc-design-resistance-1.py` opens the sample project `../projects/beam-to-cont-column.ideaCon`, runs the joint design resistance CBFEM analysis on its first connection, and prints the key result values.

Steps performed (real method names from the script):

1. Attach to a running Connection API service via `ConnectionApiServiceAttacher("http://localhost:5000").create_api_client()`.
2. `api_client.project.open_project_from_filepath(project_file_path)` — open the `.ideaCon` project.
3. `api_client.project.get_project_data(...)` and `api_client.connection.get_connections(...)` — print project data and pick the first connection.
4. Build a `ConCalculationParameter` with `analysis_type = "total_Design"` for that connection.
5. `api_client.calculation.get_raw_json_results(...)` — run the analysis and get the raw JSON results.
6. Parse the `totalCapacity` section of the results and print `appliedLoadPercentage`, `maxPlateEps`, and `maxWeldEps`.

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
   python calc-design-resistance-1.py
   ```
