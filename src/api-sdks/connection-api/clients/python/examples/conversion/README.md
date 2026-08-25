# Design-code conversion (batch)

## What the script does

`main.py` converts every `.ideaCon` project in its folder to the American design code (AISC) and saves each converted project as `<name>_AISC.ideaCon`. Files that already end in `_AISC.ideaCon` are skipped.

Steps performed per project (real method names from the script):

1. Attach to a running Connection API service via `ConnectionApiServiceAttacher` and `api_client.project.open_project_from_filepath(...)`.
2. `api_client.conversion.get_conversion_mapping(project_id, CountryCode.AMERICAN)` — read the default conversion mapping for the target code.
3. Adjust the mapping — the script keeps the original cross-sections by setting each `target_value = source_value`.
4. `api_client.conversion.change_code(project_id, default_mapping)` — convert the project.
5. `api_client.project.download_project(project_id, output_file_path)` — save the converted project, then `api_client.project.close_project(project_id)`.

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

3. Put the `.ideaCon` projects you want to convert next to `main.py` and run:

   ```console
   python main.py
   ```
