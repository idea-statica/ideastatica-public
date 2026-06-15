# Generate a PDF report

## What the script does

`generate-report.py` opens the sample project `../projects/HSS_norm_cond.ideaCon`, calculates it, and saves a PDF report of the first connection.

Steps performed (real method names from the script):

1. Attach to a running Connection API service via `ConnectionApiServiceAttacher("http://localhost:5000").create_api_client()`.
2. `api_client.project.open_project_from_filepath(project_file_path)` — open the `.ideaCon` project.
3. `api_client.calculation.calculate(...)` — run the CBFEM analysis.
4. `api_client.report.generate_pdf(project_id, connection_id)` — generate the report and write the returned bytes to `output.pdf`.

## How to run

1. Install the client package (the package version must match your installed IDEA StatiCa version):

   ```console
   pip install ideastatica-connection-api
   ```

2. Start the Connection API service from your IDEA StatiCa installation folder (adjust to your installed version):

   ```console
   "C:\Program Files\IDEA StatiCa\StatiCa 26.0\IdeaStatiCa.ConnectionRestApi.exe"
   ```

   The service listens on port 5000 by default, which is the `baseUrl` the script expects.

3. Run the script from this folder (it locates the sample project via the relative path `../projects`):

   ```console
   python generate-report.py
   ```
